namespace GiraffeStar
{
    public interface IMessageInstruction : IMissionInstruction
    {
        void SetMessage(MessageCore msg);
    }
}