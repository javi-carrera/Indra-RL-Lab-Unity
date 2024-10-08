//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterfacesPkg
{
    [Serializable]
    public class UC2EnvironmentStepResponse : Message
    {
        public const string k_RosMessageName = "interfaces_pkg/UC2EnvironmentStep";
        public override string RosMessageName => k_RosMessageName;

        public BuiltinInterfaces.TimeMsg request_received_timestamp;
        public BuiltinInterfaces.TimeMsg response_sent_timestamp;
        public BuiltinInterfaces.TimeMsg response_received_timestamp;
        public Sensor.CompressedImageMsg compressed_image;
        public UC2AgentStateMsg state;

        public UC2EnvironmentStepResponse()
        {
            this.request_received_timestamp = new BuiltinInterfaces.TimeMsg();
            this.response_sent_timestamp = new BuiltinInterfaces.TimeMsg();
            this.response_received_timestamp = new BuiltinInterfaces.TimeMsg();
            this.compressed_image = new Sensor.CompressedImageMsg();
            this.state = new UC2AgentStateMsg();
        }

        public UC2EnvironmentStepResponse(BuiltinInterfaces.TimeMsg request_received_timestamp, BuiltinInterfaces.TimeMsg response_sent_timestamp, BuiltinInterfaces.TimeMsg response_received_timestamp, Sensor.CompressedImageMsg compressed_image, UC2AgentStateMsg state)
        {
            this.request_received_timestamp = request_received_timestamp;
            this.response_sent_timestamp = response_sent_timestamp;
            this.response_received_timestamp = response_received_timestamp;
            this.compressed_image = compressed_image;
            this.state = state;
        }

        public static UC2EnvironmentStepResponse Deserialize(MessageDeserializer deserializer) => new UC2EnvironmentStepResponse(deserializer);

        private UC2EnvironmentStepResponse(MessageDeserializer deserializer)
        {
            this.request_received_timestamp = BuiltinInterfaces.TimeMsg.Deserialize(deserializer);
            this.response_sent_timestamp = BuiltinInterfaces.TimeMsg.Deserialize(deserializer);
            this.response_received_timestamp = BuiltinInterfaces.TimeMsg.Deserialize(deserializer);
            this.compressed_image = Sensor.CompressedImageMsg.Deserialize(deserializer);
            this.state = UC2AgentStateMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.request_received_timestamp);
            serializer.Write(this.response_sent_timestamp);
            serializer.Write(this.response_received_timestamp);
            serializer.Write(this.compressed_image);
            serializer.Write(this.state);
        }

        public override string ToString()
        {
            return "UC2EnvironmentStepResponse: " +
            "\nrequest_received_timestamp: " + request_received_timestamp.ToString() +
            "\nresponse_sent_timestamp: " + response_sent_timestamp.ToString() +
            "\nresponse_received_timestamp: " + response_received_timestamp.ToString() +
            "\ncompressed_image: " + compressed_image.ToString() +
            "\nstate: " + state.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}
