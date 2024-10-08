// Project: Playground
// File: LidarSensor.cs
// Authors: Javier Carrera, Guillermo Escolano
// License: Apache 2.0 (refer to LICENSE file in the project root)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.InterfacesPkg;
using System;

public class Lidar2DSensor : Sensor {
    
    [HideInInspector] public LaserScanMsg laserScan2DMsg;

    [Header("Lidar Sensor Settings")]
    public Vector3 positionOffset;
    public float angleMin;
    public float angleMax;
    public uint numRays;
    public float rangeMin;
    public float rangeMax;
    
    private float[] _ranges;
    private float _angleIncrement;

    [Header("Debug Settings")]
    public bool drawDebugRays;
    private Color _hitColor = Color.red;
    private Color _noHitColor = Color.green;



    public override void Initialize() {
        laserScan2DMsg = new LaserScanMsg();
    }

    public override void GetSensorData() {

        // Convert Unity data to ROS message
        laserScan2DMsg.angle_min = angleMin;
        laserScan2DMsg.angle_max = angleMax;
        laserScan2DMsg.angle_increment = _angleIncrement;
        laserScan2DMsg.range_min = rangeMin;
        laserScan2DMsg.range_max = rangeMax;
        laserScan2DMsg.ranges = _ranges;

    }

    protected override void UpdateSensor() {

        // Check the number of rays is greater than 0
        if (numRays <= 0) {
            Debug.LogError("numRays must be greater than 0");
            return;
        }

        // angleMin = angleMin < 0.0f ? 360.0f + angleMin : angleMin;
        // angleMax = angleMax < 0.0f ? 360.0f + angleMax : angleMax;

        // angleMax = angleMax < angleMin ? angleMin : angleMax;

        // angleMin = angleMin > 360.0f ? angleMin % 360.0f : angleMin;
        // angleMax = angleMax > 360.0f ? angleMax % 360.0f : angleMax;

        
        // Initialize ranges array and calculate angle increment
        _ranges = new float[numRays];
        _angleIncrement = numRays == 1 ? (angleMax - angleMin) : (angleMax - angleMin) / (numRays - 1);
        
        // Check if rangeMax is greater than rangeMin
        if (rangeMax <= rangeMin) {

            Debug.LogError("rangeMax must be greater than rangeMin");

            // Initialize ranges array with 0s
            for (int i = 0; i < numRays; i++) {
                _ranges[i] = 0.0f;
            }
            
            return;
        }

        for (int i = 0; i < numRays; i++) {

            // Calculate ray angle, origin and direction
            float angle = angleMin + i * _angleIncrement;
            Vector3 rotation = transform.TransformDirection(Quaternion.Euler(0, -angle, 0) * Vector3.right);
            Vector3 origin = transform.position + positionOffset + rotation * rangeMin;
            Vector3 direction = rotation ;

            // Cast ray and get distance
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, rangeMax - rangeMin, ~LayerMask.GetMask("TriggerVolume"))) {
                _ranges[i] = hit.distance + rangeMin;
            }
            else {
                _ranges[i] = rangeMax;
            }

            // Draw debug rays
            if (drawDebugRays) {
                Color color = hit.collider ? _hitColor : _noHitColor;
                Debug.DrawRay(origin, direction * (_ranges[i] - rangeMin), color);
            }
        }
    }



    public override void ResetSensor() {
    }

}
