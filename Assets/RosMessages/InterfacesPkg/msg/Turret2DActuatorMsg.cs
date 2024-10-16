//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.InterfacesPkg
{
    [Serializable]
    public class Turret2DActuatorMsg : Message
    {
        public const string k_RosMessageName = "interfaces_pkg/Turret2DActuator";
        public override string RosMessageName => k_RosMessageName;

        public float target_angle;
        public bool fire;

        public Turret2DActuatorMsg()
        {
            this.target_angle = 0.0f;
            this.fire = false;
        }

        public Turret2DActuatorMsg(float target_angle, bool fire)
        {
            this.target_angle = target_angle;
            this.fire = fire;
        }

        public static Turret2DActuatorMsg Deserialize(MessageDeserializer deserializer) => new Turret2DActuatorMsg(deserializer);

        private Turret2DActuatorMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.target_angle);
            deserializer.Read(out this.fire);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.target_angle);
            serializer.Write(this.fire);
        }

        public override string ToString()
        {
            return "Turret2DActuatorMsg: " +
            "\ntarget_angle: " + target_angle.ToString() +
            "\nfire: " + fire.ToString();
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
