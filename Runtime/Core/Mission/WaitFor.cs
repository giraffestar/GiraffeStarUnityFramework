using System;
using System.Collections.Generic;

namespace GiraffeStar
{
    public class WaitFor
    {
        public class Update : IMissionInstruction
        {
            public Mission.MissionInstructionType MissionInstructionType =>
                Mission.MissionInstructionType.WaitForUpdate;
        }

        public class LateUpdate : IMissionInstruction
        {
            public Mission.MissionInstructionType MissionInstructionType => 
                Mission.MissionInstructionType.WaitForLateUpdate;
        }

        public class FixedUpdate : IMissionInstruction
        {
            public Mission.MissionInstructionType MissionInstructionType =>
                Mission.MissionInstructionType.WaitForFixedUpdate;
        }

        public class Seconds : IMissionInstruction
        {
            public readonly float WaitTime;

            public Mission.MissionInstructionType MissionInstructionType =>
                Mission.MissionInstructionType.WaitForSeconds;
            public KeyValuePair<float, IMission> PairReference { get; set; }

            public Seconds(float time)
            {
                WaitTime = time < 0f ? 0f : time;
            }
        }

        public class Message : IMessageInstruction
        {
            public readonly string[] Domains;
            public readonly Type MessageType;

            protected MessageCore ReceivedMessage;

            public Mission.MissionInstructionType MissionInstructionType =>
                Mission.MissionInstructionType.WaitForMessage;

            public void SetMessage(MessageCore msg)
            {
                ReceivedMessage = msg;
            }

            protected Message(Type messageType)
            {
                Domains = new [] { "Default" };
                MessageType = messageType;
            }

            protected Message(Type messageType, params string[] args)
            {
                Domains = args;
                MessageType = messageType;
            }
        }

        public class Message<T> : Message
            where T : MessageCore
        {
            public T Msg => ReceivedMessage as T;

            public Message() : base(typeof(T))
            {
            }

            public Message(params string[] args) : base(typeof(T), args)
            {
            }
        }
    }
}