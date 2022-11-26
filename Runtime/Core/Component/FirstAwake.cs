using UnityEngine;

namespace GiraffeStar
{
    [DefaultExecutionOrder(-1000)]
    public class FirstAwake : MonoBehaviour
    {
        public BaseBootstrap Bootstrap;

        void Awake()
        {
            if (!GameSystem.IsInitialized)
            {
                var gameSystem = GameSystem.Init();
                Bootstrap.Setup();

                var systemObj = new GameObject("SystemUpdate", typeof(SystemUpdate));
                var systemUpdate = systemObj.GetComponent<SystemUpdate>();
                systemUpdate.SetGameSystem(gameSystem);
                DontDestroyOnLoad(systemUpdate);
            }

            Destroy(gameObject);
        }
    }
}