using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GiraffeStar
{
    public class ObjectManager
    {
        readonly HashSet<object> instances;
        readonly List<(object, MethodInfo)> updates;
        readonly List<(object, MethodInfo)> lateUpdates;
        readonly List<(object, MethodInfo)> fixedUpdates;

        public ObjectManager()
        {
            instances = new HashSet<object>();
            updates = new List<(object, MethodInfo)>();
            lateUpdates = new List<(object, MethodInfo)>();
            fixedUpdates = new List<(object, MethodInfo)>();
        }

        public void Register(object obj)
        {
            if (obj == null)
            {
                throw new FrameworkException($"Tried to register null object");
            }

            var type = obj.GetType();
            if (type.IsValueType)
            {
                throw new FrameworkException($"Value type objects cannot be registered : {obj}");
            }

            if (instances.Contains(obj))
            {
                throw new FrameworkException($"Tried to registered already registered object : {obj}");
            }

            instances.Add(obj);
            RegisterIfExists(obj, updates, "Update");
            RegisterIfExists(obj, updates, "LateUpdate");
            RegisterIfExists(obj, updates, "FixedUpdate");

            var registerMethod = GetKeyMethod(obj, "OnRegister");
            registerMethod?.Invoke(obj, null);
        }

        public bool Unregister(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!instances.Contains(obj))
            {
                return false;
            }

            updates.RemoveAll(x => Object.ReferenceEquals(x.Item1, obj));
            lateUpdates.RemoveAll(x => Object.ReferenceEquals(x.Item1, obj));
            fixedUpdates.RemoveAll(x => Object.ReferenceEquals(x.Item1, obj));
            instances.Remove(obj);

            var unregisterMethod = GetKeyMethod(obj, "OnUnregister");
            unregisterMethod?.Invoke(obj, null);

            return true;
        }

        public void Update()
        {
            foreach (var (obj, methodInfo) in updates)
            {
                methodInfo.Invoke(obj, null);
            }
        }

        public void LateUpdate()
        {
            foreach (var (obj, methodInfo) in lateUpdates)
            {
                methodInfo.Invoke(obj, null);
            }
        }

        public void FixedUpdate()
        {
            foreach (var (obj, methodInfo) in fixedUpdates)
            {
                methodInfo.Invoke(obj, null);
            }
        }

        public void Cleanup()
        {
            var instanceList = instances.ToList();
            foreach (var obj in instanceList)
            {
                Unregister(obj);
            }

            instances.Clear();
            updates.Clear();
            lateUpdates.Clear();
            fixedUpdates.Clear();
        }

        static MethodInfo GetKeyMethod(object obj, string methodName)
        {
            var type = obj.GetType();
            var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            // 파라메터 개수가 0인 것만 취급
            if (methodInfo?.GetParameters().Length != 0)
            {
                methodInfo = null;
            }

            return methodInfo;
        }

        static void RegisterIfExists(object obj, List<(object, MethodInfo)> list, string methodName)
        {
            var targetMethod = GetKeyMethod(obj, methodName);
            if (targetMethod != null)
            {
                list.Add((obj, targetMethod));
            }
        }
    }
}