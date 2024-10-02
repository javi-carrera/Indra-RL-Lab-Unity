using UnityEngine;
using RosMessageTypes.InterfacesPkg;
using UnityEngine.AI;


public class BotTwist2DSensor : Sensor {

    [HideInInspector] public Twist2DMsg twist2DMsg;

    [Header("Twist Sensor Settings")]
    public GameObject target;
    private NavMeshAgent navAgent;

    public override void Initialize() {
        twist2DMsg = new Twist2DMsg();
        navAgent = target.GetComponent<NavMeshAgent>();
    }

    public override void GetSensorData() {

        Vector3 linearVelocity = navAgent.velocity;

        // Transform the velocities to the target object's local frame
        linearVelocity = target.transform.InverseTransformDirection(linearVelocity);

        // Convert Unity data to ROS message
        twist2DMsg.x = linearVelocity.x;
        twist2DMsg.y = linearVelocity.z;
        twist2DMsg.theta = 0.0f;

    }

    protected override void UpdateSensor() {
    }

    public override void ResetSensor() {
    }
}
