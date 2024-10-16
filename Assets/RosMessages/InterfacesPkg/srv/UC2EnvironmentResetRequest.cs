//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterfacesPkg
{
    [Serializable]
    public class UC2EnvironmentResetRequest : Message
    {
        public const string k_RosMessageName = "interfaces_pkg/UC2EnvironmentReset";
        public override string RosMessageName => k_RosMessageName;

        public BuiltinInterfaces.TimeMsg request_sent_timestamp;
        public bool reset;

        public UC2EnvironmentResetRequest()
        {
            this.request_sent_timestamp = new BuiltinInterfaces.TimeMsg();
            this.reset = false;
        }

        public UC2EnvironmentResetRequest(BuiltinInterfaces.TimeMsg request_sent_timestamp, bool reset)
        {
            this.request_sent_timestamp = request_sent_timestamp;
            this.reset = reset;
        }

        public static UC2EnvironmentResetRequest Deserialize(MessageDeserializer deserializer) => new UC2EnvironmentResetRequest(deserializer);

        private UC2EnvironmentResetRequest(MessageDeserializer deserializer)
        {
            this.request_sent_timestamp = BuiltinInterfaces.TimeMsg.Deserialize(deserializer);
            deserializer.Read(out this.reset);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.request_sent_timestamp);
            serializer.Write(this.reset);
        }

        public override string ToString()
        {
            return "UC2EnvironmentResetRequest: " +
            "\nrequest_sent_timestamp: " + request_sent_timestamp.ToString() +
            "\nreset: " + reset.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
