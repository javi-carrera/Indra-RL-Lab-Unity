using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.InterfacesPkg;

public class Turret2DSensor : Sensor {
    
    [HideInInspector] public Turret2DSensorMsg turret2DSensorMsg;


    [Header("Turret 2D Sensor Settings")]
    public Turret2DActuator turret2DActuator;
    private Vector3 _rayStartPoint;
    private float _laserRange;
    [Header("Debug")]
    public bool drawDebugRay;


    public override void Initialize() {
        turret2DSensorMsg = new Turret2DSensorMsg();
    }

    public override void GetSensorData() {
        // Convert Unity data to ROS message
        turret2DSensorMsg.current_angle = turret2DActuator.currentAngle;
        turret2DSensorMsg.fire_rate = turret2DActuator.fireRate;
        turret2DSensorMsg.cooldown = turret2DActuator.cooldown;
        turret2DSensorMsg.has_fired = turret2DActuator.hasFired;
        turret2DSensorMsg.max_laser_range = turret2DActuator.range;
        turret2DSensorMsg.laser_range = _laserRange;
        turret2DActuator.hasFired = false;
    }

    protected override void UpdateSensor() {

        _rayStartPoint = new Vector3(transform.position.x, turret2DActuator.shootingPoint.position.y, transform.position.z);

        if (Physics.Raycast(_rayStartPoint, turret2DActuator.shootingPoint.forward, out RaycastHit hit, turret2DActuator.range, ~LayerMask.GetMask("TriggerVolume")))
            _laserRange = hit.distance;
        else
            _laserRange = turret2DActuator.range;

        if (drawDebugRay) DrawDebugRay();
    }

    private void DrawDebugRay() {
        Color rayColor = _laserRange < turret2DActuator.range ? Color.magenta : Color.yellow;
        Debug.DrawRay(_rayStartPoint, turret2DActuator.shootingPoint.forward * _laserRange, rayColor);
    }

    public override void ResetSensor() {
    }


}
