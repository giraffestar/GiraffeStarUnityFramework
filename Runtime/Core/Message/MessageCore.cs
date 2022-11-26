namespace GiraffeStar
{
    public abstract class MessageCore
    {
        public string Filter = "Default";

        public void Dispatch()
        {
            GameSystem.ProcessMessage(this);
        }

        public void Dispatch(GameSystem gameSystem)
        {
            GameSystem.ProcessMessage(this, gameSystem);
        }
    }
}