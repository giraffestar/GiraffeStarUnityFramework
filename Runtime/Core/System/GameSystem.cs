using UnityEngine;

namespace GiraffeStar
{
    public partial class GameSystem
    {
        readonly MissionHandler missionHandler;
        readonly MessageHandler messageHandler;
        readonly ModuleManager moduleManager;
        readonly ComponentManager componentManager;
        readonly ObjectManager objectManager;

        public GameSystem()
        {
            moduleManager = new ModuleManager();
            componentManager = new ComponentManager();
            objectManager = new ObjectManager();
            missionHandler = new MissionHandler();
            messageHandler = new MessageHandler(componentManager, missionHandler);
        }

        public void Update()
        {
            missionHandler.Update();
            moduleManager.Update();
            objectManager.Update();
        }

        public void LateUpdate()
        {
            missionHandler.LateUpdate();
            moduleManager.LateUpdate();
            objectManager.LateUpdate();
        }

        public void FixedUpdate()
        {
            missionHandler.FixedUpdate();
            moduleManager.FixedUpdate();
            objectManager.FixedUpdate();
        }

        public void RegisterInternal(IModule module)
        {
            moduleManager.Register(module);
            messageHandler.Register(module);
        }

        public void RegisterInternal(Component component)
        {
            componentManager.Register(component);
            messageHandler.Register(component);
        }

        public void RegisterInternal(object obj)
        {
            objectManager.Register(obj);
            messageHandler.Register(obj);
        }

        public void UnregisterInternal(IModule module)
        {
            if (moduleManager.Unregister(module))
            {
                messageHandler.Unregister(module);
            }
        }

        public void UnregisterInternal(Component component)
        {
            if (componentManager.Unregister(component))
            {
                messageHandler.Unregister(component);
            }
        }

        public void UnregisterInternal(object obj)
        {
            if (objectManager.Unregister(obj))
            {
                messageHandler.Unregister(obj);
            }
        }

        public void CleanUp()
        {
            moduleManager.CleanUp();
            componentManager.CleanUp();
            objectManager.Cleanup();
        }
    }
}