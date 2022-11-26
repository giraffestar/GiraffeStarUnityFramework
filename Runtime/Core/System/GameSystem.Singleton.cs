using UnityEngine;

namespace GiraffeStar
{
    // 싱글톤 구현을 위해 Wrapper 역할을 해주는 부분들
    public partial class GameSystem
    {
        public static bool IsInitialized { get; private set; }

        static GameSystem instance;
        
        public static GameSystem Init()
        {
            if (IsInitialized)
            {
                return instance;
            }

            IsInitialized = true;
            instance = new GameSystem();
            Setup();

            return instance;
        }

        public static void Register(IModule module)
        {
            instance.RegisterInternal(module);
        }

        public static void Register(Component component)
        {
            instance.RegisterInternal(component);
        }

        public static void Register(object obj)
        {
            instance.RegisterInternal(obj);
        }

        public static void Unregister(IModule module)
        {
            instance.UnregisterInternal(module);
        }

        public static void Unregister(Component component)
        {
            instance.UnregisterInternal(component);
        }

        public static void Unregister(object obj)
        {
            instance.UnregisterInternal(obj);
        }

        public static void ProcessMessage(MessageCore message)
        {
            instance.messageHandler.ProcessMessage(message);
        }

        public static void ProcessMessage(MessageCore message, GameSystem gameSystem)
        {
            gameSystem.messageHandler.ProcessMessage(message);
        }

        public static void AddMission(Mission.MissionInstructionType waitType, Mission mission)
        {
            instance.missionHandler.AddMission(waitType, mission);
        }

        public static void AddMission(Mission.MissionInstructionType waitType, Mission mission, GameSystem gameSystem)
        {
            gameSystem.missionHandler.AddMission(waitType, mission);
        }

        public static void CancelMission(Mission mission)
        {
            instance.missionHandler.CancelMission(mission);
        }

        public static void CancelMission(Mission mission, GameSystem gameSystem)
        {
            gameSystem.missionHandler.CancelMission(mission);
        }

        public static T FindModule<T>()
            where T : IModule
        {
            if (!IsInitialized)
            {
                throw new FrameworkException("Need to initialize GameSystem first");
            }

            return instance.moduleManager.FindModule<T>();
        }

        static void Setup()
        {
            Config.Init();
        }
    }
}