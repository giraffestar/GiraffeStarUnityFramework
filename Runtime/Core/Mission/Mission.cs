using System;
using System.Collections;
using UnityEngine;

namespace GiraffeStar
{
    public sealed class Mission : IMission
    {
        public enum MissionState
        {
            Ready,
            Waiting,
            Paused,
            Cancelled,
            Completed,
        }

        public enum MissionInstructionType
        {
            WaitForUpdate,
            WaitForLateUpdate,
            WaitForFixedUpdate,
            WaitForSeconds,
            WaitForMessage,
        }

        readonly Func<IEnumerator> instructor;
        readonly GameSystem gameSystem;
        
        public MissionState State { get; private set; }
        public IMissionInstruction CurrentMissionInstruction { get; private set; }
        public Action OnMissionComplete;
        
        IEnumerator mission;

        public Mission(Func<IEnumerator> instruction)
        {
            instructor = instruction;
            mission = instruction();
            State = MissionState.Ready;
        }

        public Mission(Func<IEnumerator> instruction, GameSystem gameSystem) : this(instruction)
        {
            this.gameSystem = gameSystem;
        }

        public static Mission Create(Func<IEnumerator> instruction)
        {
            return new Mission(instruction);
        }

        //public static Mission Create<T1>(Func<T1, IEnumerator> instruction, T1 arg1)
        //{
        //    return new Mission(() => instruction(arg1));
        //}

        //public static Mission Create<T1, T2>(Func<T1, T2, IEnumerator> instruction, T1 arg1, T2 arg2)
        //{
        //    return new Mission(() => instruction(arg1, arg2));
        //}

        public void Run()
        {
            switch (State)
            {
                case MissionState.Ready:
                    (this as IMission).Progress();
                    break;
                case MissionState.Paused:
                    State = MissionState.Waiting;
                    break;
                default:
                    Debug.LogWarning("Trying to initiate non-runnable mission");
                    break;
            }
        }

        public void Pause()
        {
            if (State == MissionState.Waiting)
            {
                State = MissionState.Paused;
            }
        }

        public void Cancel()
        {
            switch (State)
            {
                case MissionState.Waiting:
                case MissionState.Paused:
                    State = MissionState.Cancelled;
                    if (gameSystem != null)
                    {
                        GameSystem.CancelMission(this, gameSystem);
                    }
                    else
                    {
                        GameSystem.CancelMission(this);
                    }
                    break;
                default:
                    State = MissionState.Cancelled;
                    break;
            }
        }

        public void Reset()
        {
            if (State == MissionState.Ready) { return; }

            Cancel();
            mission = instructor();
            State = MissionState.Ready;
        }

        void IMission.Progress()
        {
            if (mission.MoveNext())
            {
                var current = mission.Current ?? new WaitFor.Update();

                if (current is IMissionInstruction missionInstruction)
                {
                    CurrentMissionInstruction = missionInstruction;

                    switch (missionInstruction.MissionInstructionType)
                    {
                        case MissionInstructionType.WaitForUpdate:
                        case MissionInstructionType.WaitForLateUpdate:
                        case MissionInstructionType.WaitForFixedUpdate:
                        case MissionInstructionType.WaitForSeconds:
                        case MissionInstructionType.WaitForMessage:
                            State = MissionState.Waiting;
                            if (gameSystem != null)
                            {
                                GameSystem.AddMission(missionInstruction.MissionInstructionType, this, gameSystem);
                            }
                            else
                            {
                                GameSystem.AddMission(missionInstruction.MissionInstructionType, this);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    throw new FrameworkException("Unknown mission instruction error");
                }
            }
            else
            {
                State = MissionState.Completed;
                OnMissionComplete?.Invoke();
            }
        }
    }
}