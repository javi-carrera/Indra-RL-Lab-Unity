using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using RosMessageTypes.InterfacesPkg;

public class Turret2DActuator : Actuator<Turret2DActuatorMsg> {
    
    
    [Header("Turret 2D Actuator Settings")]
    public Transform target;
    public Transform shootingPoint;
    public GameObject bulletPrefab;
    public float shootVelocity;
    public float fireRate;

    [HideInInspector] public float rotationSpeed;
    [HideInInspector] public float currentAngle;
    [HideInInspector] public float cooldown;
    [HideInInspector] public bool fire;
    [HideInInspector] public bool hasFired;

    private Vector3 _previousShootingPointPosition;
    private Vector3 _shootingPointVelocity;
    
    



    public override void Initialize() {

        // Reset the actuator
        ResetActuator();

    }

    public override void SetActuatorData(Turret2DActuatorMsg msg) {

        // Convert from ROS message to Unity data
        rotationSpeed = msg.target_angle;
        fire = msg.fire;
    }

    public override void ResetActuator() {

        // Reset ROS message data
        rotationSpeed = 0.0f;
        fire = false;
        hasFired = false;

        // Reset velocity and cooldown
        _previousShootingPointPosition = shootingPoint.position;
        _shootingPointVelocity = Vector3.zero;
        cooldown = 1.0f / fireRate;
    }

    protected override void UpdateActuator() {

        // Rotate the turret with rotation speed
        target.transform.Rotate(Vector3.up, -rotationSpeed * Time.fixedDeltaTime);
        currentAngle = target.localEulerAngles.y;

        // Calculate the velocity of the shooting point
        _shootingPointVelocity = (shootingPoint.position - _previousShootingPointPosition) / Time.fixedDeltaTime;
        _previousShootingPointPosition = shootingPoint.position;

        // Fire the bullet
        cooldown = Mathf.Max(0, cooldown - Time.fixedDeltaTime);
        if (fire && cooldown <= 0) {
            cooldown = 1.0f / fireRate;
            Shoot();
            hasFired = true;
        }
    }

    private void Shoot() {
        
        // GameObject bullet = _objectPooler.GetPooledObject();
        GameObject bullet = Instantiate(bulletPrefab);
        
        if (bullet != null) {

            // Get the rigidbody of the bullet
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            
            // Set the position and rotation of the bullet
            bullet.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);

            // Set the velocity of the bullet
            rb.AddForce(_shootingPointVelocity + shootVelocity * shootingPoint.forward, ForceMode.VelocityChange);

        }
    }
}
