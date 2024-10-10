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
    public float range;
    public float fireRate;

    [HideInInspector] public float rotationSpeed;
    [HideInInspector] public float currentAngle;
    [HideInInspector] public float cooldown;
    [HideInInspector] public bool fire;
    [HideInInspector] public bool hasFired;

    private Vector3 _previousShootingPointPosition;
    private Vector3 _shootingPointVelocity;
    private float _shootVelocity;
    
    
    public override void Initialize() {
        ResetActuator();
    }

    public override void SetActuatorData(Turret2DActuatorMsg msg) {
        rotationSpeed = msg.rotation_speed;
        fire = msg.fire;
    }

    public override void ResetActuator() {

        rotationSpeed = 0.0f;
        fire = false;
        hasFired = false;

        _previousShootingPointPosition = shootingPoint.position;
        _shootingPointVelocity = Vector3.zero;
        cooldown = 1.0f / fireRate;
    }

    protected override void UpdateActuator() {

        target.transform.Rotate(Vector3.up, -Mathf.Rad2Deg * rotationSpeed * Time.fixedDeltaTime);
        currentAngle = target.localEulerAngles.y;

        _shootingPointVelocity = (shootingPoint.position - _previousShootingPointPosition) / Time.fixedDeltaTime;
        _previousShootingPointPosition = shootingPoint.position;

        cooldown = Mathf.Max(0, cooldown - Time.fixedDeltaTime);
        if (fire && cooldown <= 0) {
            cooldown = 1.0f / fireRate;
            Shoot();
            hasFired = true;
        }
    }

    private void Shoot() {
        
        GameObject bullet = Instantiate(bulletPrefab);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        bullet.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);

        _shootVelocity = range / Mathf.Sqrt(2 * shootingPoint.position.y / Mathf.Abs(Physics.gravity.y));

        rb.AddForce(_shootingPointVelocity + _shootVelocity * shootingPoint.forward, ForceMode.VelocityChange);
    }
}
