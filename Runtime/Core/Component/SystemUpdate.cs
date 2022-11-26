using UnityEngine;

namespace GiraffeStar
{
    public class SystemUpdate : MonoBehaviour
    {
        GameSystem gameSystem;

        public void SetGameSystem(GameSystem gameSystem)
        {
            this.gameSystem = gameSystem;
        }

        void Update()
        {
            gameSystem.Update();
        }

        void LateUpdate()
        {
            gameSystem.LateUpdate();
        }

        void FixedUpdate()
        {
            gameSystem.FixedUpdate();
        }
    }
}