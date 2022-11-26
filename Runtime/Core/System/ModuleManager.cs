using System;
using System.Collections.Generic;
using System.Reflection;

namespace GiraffeStar
{
    public class ModuleManager
    {
        readonly Dictionary<Type, IModule> modulesByType;
        readonly List<(IModule, MethodInfo)> updates;
        readonly List<(IModule, MethodInfo)> lateUpdates;
        readonly List<(IModule, MethodInfo)> fixedUpdates;

        public ModuleManager()
        {
            modulesByType = new Dictionary<Type, IModule>();
            updates = new List<(IModule, MethodInfo)>();
            lateUpdates = new List<(IModule, MethodInfo)>();
            fixedUpdates = new List<(IModule, MethodInfo)>();
        }

        public void Register(IModule module)
        {
            if (module == null)
            {
                throw new FrameworkException($"Tried to register non-IModule");
            }

            var moduleType = module.GetType();

            if(modulesByType.ContainsKey(moduleType))
            {
                throw new FrameworkException($"Tried to register already registered Module {moduleType}");
            }

            modulesByType.Add(moduleType, module);
            RegisterIfExists(module, updates, "Update");
            RegisterIfExists(module, updates, "LateUpdate");
            RegisterIfExists(module, updates, "FixedUpdate");

            var registerMethod = GetKeyMethod(module, "OnRegister");
            registerMethod?.Invoke(module, null);
        }

        public bool Unregister(IModule module)
        {
            var moduleType = module.GetType();

            if (!modulesByType.ContainsKey(moduleType))
            {
                return false;
            }

            updates.RemoveAll(x => x.Item1 == module);
            lateUpdates.RemoveAll(x => x.Item1 == module);
            fixedUpdates.RemoveAll(x => x.Item1 == module);
            modulesByType.Remove(moduleType);

            var unregisterMethod = GetKeyMethod(module, "OnUnregister");
            unregisterMethod?.Invoke(module, null);

            return true;
        }

        public T FindModule<T>()
            where T : IModule
        {
            if (modulesByType.TryGetValue(typeof(T), out var module))
            {
                return (T)module;
            }

            return default(T);
        }

        public void Update()
        {
            foreach (var (module, methodInfo) in updates)
            {
                methodInfo.Invoke(module, null);
            }
        }

        public void LateUpdate()
        {
            foreach (var (module, methodInfo) in lateUpdates)
            {
                methodInfo.Invoke(module, null);
            }
        }

        public void FixedUpdate()
        {
            foreach (var (module, methodInfo) in fixedUpdates)
            {
                methodInfo.Invoke(module, null);
            }
        }

        public void CleanUp()
        {
            var modules = new IModule[modulesByType.Count];
            modulesByType.Values.CopyTo(modules, 0);
            
            foreach (var module in modules)
            {
                Unregister(module);
            }
        }

        static MethodInfo GetKeyMethod(IModule module, string methodName)
        {
            var type = module.GetType();
            var methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            // 파라메터 개수가 0인 것만 취급
            if (methodInfo?.GetParameters().Length != 0)
            {
                methodInfo = null;
            }

            return methodInfo;
        }

        static void RegisterIfExists(IModule module, List<(IModule, MethodInfo)> list, string methodName)
        {
            var targetMethod = GetKeyMethod(module, methodName);
            if (targetMethod != null)
            {
                list.Add((module, targetMethod));
            }
        }
    }
}
