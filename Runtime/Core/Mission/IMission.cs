namespace GiraffeStar
{
    public interface IMission
    {
        Mission.MissionState State { get; }
        IMissionInstruction CurrentMissionInstruction { get; }
        void Progress();
    }
}