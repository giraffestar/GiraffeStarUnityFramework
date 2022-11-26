using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiraffeStar
{
    public class MissionHandler
    {
#region UpdateList
        class UpdateList
        {
            readonly List<IMission> handlingList;
            readonly Stack<IMission> missionStack;

            public UpdateList()
            {
                handlingList = new List<IMission>();
                missionStack = new Stack<IMission>();
            }

            public void Update()
            {
                if (handlingList.Count == 0) { return; }

                for (int i = handlingList.Count - 1; i >= 0; i--)
                {
                    var mission = handlingList[i];
                    if (mission.State == Mission.MissionState.Paused) { continue; }

                    handlingList.RemoveAt(i);
                    missionStack.Push(mission);
                }

                while (missionStack.Count > 0)
                {
                    var mission = missionStack.Pop();
                    if (mission.State == Mission.MissionState.Waiting)
                    {
                        mission.Progress();
                    }
                }

                missionStack.Clear();
            }

            public void Add(IMission mission)
            {
                handlingList.Add(mission);
            }

            public void Remove(IMission mission)
            {
                handlingList.Remove(mission);
            }
        }
#endregion
        
        readonly UpdateList updateHandler;
        readonly UpdateList lateUpdateHandler;
        readonly UpdateList fixedUpdateHandler;
        readonly PriorityQueue<float, IMission> timeUpdateHandler;
        readonly Dictionary<string, Dictionary<Type, List<IMission>>> messageHandler;
        readonly Queue<IMission> cancelBuffer;
        bool cancelLock;

        public MissionHandler()
        {
            updateHandler = new UpdateList();
            lateUpdateHandler = new UpdateList();
            fixedUpdateHandler = new UpdateList();
            timeUpdateHandler = new PriorityQueue<float, IMission>();
            messageHandler = new Dictionary<string, Dictionary<Type, List<IMission>>>();
            cancelBuffer = new Queue<IMission>();
        }

        public void Update()
        {
            LockForProcessing(true);
            updateHandler.Update();
            UpdateTimeHandler();
            LockForProcessing(false);
        }

        public void LateUpdate()
        {
            LockForProcessing(true);
            lateUpdateHandler.Update();
            LockForProcessing(false);
        }

        public void FixedUpdate()
        {
            LockForProcessing(true);
            fixedUpdateHandler.Update();
            LockForProcessing(false);
        }

        public void AddMission(Mission.MissionInstructionType waitType, Mission mission)
        {
            switch (waitType)
            {
                case Mission.MissionInstructionType.WaitForUpdate:
                    updateHandler.Add(mission);
                    break;
                case Mission.MissionInstructionType.WaitForLateUpdate:
                    lateUpdateHandler.Add(mission);
                    break;
                case Mission.MissionInstructionType.WaitForFixedUpdate:
                    fixedUpdateHandler.Add(mission);
                    break;
                case Mission.MissionInstructionType.WaitForSeconds:
                    AddTimeHandler(mission);
                    break;
                case Mission.MissionInstructionType.WaitForMessage:
                    AddMessageHandler(mission);
                    break;
                default:
                    throw new FrameworkException("Wrong mission type handling");
            }
        }

        public void CancelMission(IMission mission)
        {
            if (cancelLock)
            {
                cancelBuffer.Enqueue(mission);
            }
            else
            {
                RemoveFromHandlers(mission);
            }
        }

        void RemoveFromHandlers(IMission mission)
        {
            if(cancelLock) { return; }  

            switch (mission.CurrentMissionInstruction.MissionInstructionType)
            {
                case Mission.MissionInstructionType.WaitForUpdate:
                    updateHandler.Remove(mission);
                    break;
                case Mission.MissionInstructionType.WaitForLateUpdate:
                    lateUpdateHandler.Remove(mission);
                    break;
                case Mission.MissionInstructionType.WaitForFixedUpdate:
                    fixedUpdateHandler.Remove(mission);
                    break;
                case Mission.MissionInstructionType.WaitForSeconds:
                    RemoveTimeHandler(mission);
                    break;
                case Mission.MissionInstructionType.WaitForMessage:
                    RemoveMessageHandler(mission);
                    break;
                default:
                    throw new FrameworkException($"Wrong mission type handling: {mission.CurrentMissionInstruction.MissionInstructionType}");
            }
        }

        void AddTimeHandler(IMission mission)
        {
            if (mission.CurrentMissionInstruction is WaitFor.Seconds instruction)
            {
                instruction.PairReference = new KeyValuePair<float, IMission>(Time.time + instruction.WaitTime, mission);
                timeUpdateHandler.Add(instruction.PairReference);
            }
            else
            {
                throw new FrameworkException("Tried to add non-time instruction");
            }
        }

        void UpdateTimeHandler()
        {
            if (timeUpdateHandler.IsEmpty) { return; }

            var currentTime = Time.time;
            while (!timeUpdateHandler.IsEmpty && timeUpdateHandler.Peek().Key < currentTime)
            {
                var keyPair = timeUpdateHandler.Dequeue();
                keyPair.Value.Progress();
            }
        }

        void RemoveTimeHandler(IMission mission)
        {
            if (mission.CurrentMissionInstruction is WaitFor.Seconds instruction)
            {
                timeUpdateHandler.Remove(instruction.PairReference);
            }
            else
            {
                throw new FrameworkException("Tried to remove non-time instruction");
            }
        }

        void AddMessageHandler(IMission mission)
        {
            if (mission.CurrentMissionInstruction is WaitFor.Message instruction)
            {
                foreach (var service in instruction.Domains)
                {
                    if (!messageHandler.ContainsKey(service))
                    {
                        messageHandler.Add(service, new Dictionary<Type, List<IMission>>());
                    }

                    var serviceDic = messageHandler[service];
                    if (!serviceDic.ContainsKey(instruction.MessageType))
                    {
                        serviceDic.Add(instruction.MessageType, new List<IMission>());
                    }

                    var missionList = serviceDic[instruction.MessageType];
                    missionList.Add(mission);
                }
            }
            else
            {
                throw new FrameworkException("Wrong mission instruction handling");
            }
        }

        void RemoveMessageHandler(IMission mission)
        {
            if (mission.CurrentMissionInstruction is WaitFor.Message instruction)
            {
                foreach (var service in instruction.Domains)
                {
                    messageHandler[service][instruction.MessageType].Remove(mission);
                }
            }
            else
            {
                throw new FrameworkException("Wrong mission instruction handling");
            }
        }

        public int DispatchMessage(MessageCore msg)
        {
            if (!messageHandler.ContainsKey(msg.Filter)) { return 0; }

            var service = messageHandler[msg.Filter];

            if (!service.ContainsKey(msg.GetType())) { return 0; }

            LockForProcessing(true);

            var subscriberCount = 0;
            var missionList = service[msg.GetType()];
            var waitingStack = new Stack<IMission>();

            // try to consider potential collection corruption
            for (int i = missionList.Count - 1; i >= 0; i--)
            {
                if (missionList[i].State == Mission.MissionState.Paused)
                {
                    // paused missions are included
                    subscriberCount++;
                    continue;
                }

                waitingStack.Push(missionList[i]);
                missionList.RemoveAt(i);
            }

            while (waitingStack.Count > 0)
            {
                var mission = waitingStack.Pop();
                if (mission.State == Mission.MissionState.Waiting)
                {
                    (mission.CurrentMissionInstruction as IMessageInstruction)?.SetMessage(msg);
                    mission.Progress();
                    subscriberCount++;
                }
            }

            LockForProcessing(false);

            return subscriberCount;
        }

        void LockForProcessing(bool isLock)
        {
            cancelLock = isLock;
            if (!cancelLock)
            {
                while (cancelBuffer.Count > 0)
                {
                    var mission = cancelBuffer.Dequeue();
                    RemoveFromHandlers(mission);
                }
            }
        }
    }
}