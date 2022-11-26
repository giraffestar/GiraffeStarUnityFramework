using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GiraffeStar
{
    public class MessageHandler
    {
        readonly ComponentManager componentManager;
        readonly MissionHandler missionHandler;
        readonly Dictionary<string, Dictionary<Type, List<(MethodInfo, object)>>> subscriptionsByServices;

        public MessageHandler(ComponentManager componentManager, MissionHandler missionHandler)
        {
            this.componentManager = componentManager;
            this.missionHandler = missionHandler;
            subscriptionsByServices = new Dictionary<string, Dictionary<Type, List<(MethodInfo, object)>>>();
        }

        public void Register(object obj)
        {
            var type = obj.GetType();
            var subscriberMethods = GetSubscriberMethods(type);

            foreach (var (methodInfo, subscriberAttribute) in subscriberMethods)
            {
                foreach (var domain in subscriberAttribute.Filters)
                {
                    if (!subscriptionsByServices.ContainsKey(domain))
                    {
                        subscriptionsByServices.Add(domain, new Dictionary<Type, List<(MethodInfo, object)>>());
                    }

                    var parameterType = methodInfo.GetParameters()[0].ParameterType;
                    if (!subscriptionsByServices[domain].ContainsKey(parameterType))
                    {
                        subscriptionsByServices[domain].Add(parameterType, new List<(MethodInfo, object)>());
                    }

                    subscriptionsByServices[domain][parameterType].Add((methodInfo, obj));
                }
            }
        }

        public void Unregister(object obj)
        {
            var type = obj.GetType();
            var subscriberMethods = GetSubscriberMethods(type);

            foreach (var (methodInfo, subscriberAttribute) in subscriberMethods)
            {
                foreach (var service in subscriberAttribute.Filters)
                {
                    var parameterType = methodInfo.GetParameters()[0].ParameterType;

                    subscriptionsByServices[service][parameterType].RemoveAll(x => x.Item2 == obj);
                }
            }
        }

        public void ProcessMessage(MessageCore message)
        {
            var subscriberCount = 0;
            var reserved = new List<(MethodInfo, object)>();
            var targetMessageTypes = new List<Type>();
            
            // dispatches messages even to parent classes
            var messageType = message.GetType();
            while (messageType != null && messageType != typeof(MessageCore))
            {
                targetMessageTypes.Add(messageType);
                messageType = messageType.BaseType;
            }
            
            var filter = message.Filter;
            if(subscriptionsByServices.TryGetValue(filter, out var defaultSubscription))
            {
                foreach (var targetMessageType in targetMessageTypes)
                {
                    if (defaultSubscription.TryGetValue(targetMessageType, out var methodList))
                    {
                        foreach (var (methodInfo, obj) in methodList)
                        {
                            // 핸들링 중에 콜렉션을 변경시키는 작업이 있을 수 있다.
                            reserved.Add((methodInfo, obj));
                        }
                    }
                }
            }

            foreach (var (methodInfo, obj) in reserved)
            {
                if (obj == null)
                {
                    // 없어진 컴포넌트 처리
                    continue;
                }

                if (obj is Component comp)
                {
                    if (comp == null || comp.gameObject == null)
                    {
                        // 없어진 컴포넌트 처리
                        componentManager.Unregister(comp);
                        continue;
                    }
                }

                methodInfo.Invoke(obj, new object[] { message });
                subscriberCount++;
            }

            subscriberCount += missionHandler.DispatchMessage(message);

            if (subscriberCount == 0)
            {
                // TODO add log warnings
            }
        }

        static List<(MethodInfo, SubscriberAttribute)> GetSubscriberMethods(Type type)
        {
            var result = new List<(MethodInfo, SubscriberAttribute)>();

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var methodInfo in methods)
            {
                var attributes = methodInfo.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is SubscriberAttribute subscriberAttribute)
                    {
                        var parameters = methodInfo.GetParameters();
                        if (parameters.Length != 1)
                        {
                            throw new FrameworkException($"{type}'s {methodInfo.Name} has zero or more than one parameters. Subscriber system only supports one parameter");
                        }

                        result.Add((methodInfo, subscriberAttribute));
                    }
                }
            }
            
            return result;
        }
    }
}
