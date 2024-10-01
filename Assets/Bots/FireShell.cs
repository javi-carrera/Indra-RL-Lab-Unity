using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShell : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform shootingPoint;
    public GameObject agent;
    public Transform turretBase;

    public float shootVelocity = 25.0f;
    public float rotationSpeed = 100.0f;
    public float fireRate = 1.0f;

    private float _cooldown;
    private float maxbulletDistance;
    private Rigidbody agent_rigidbody;
    
    private void CalculateMaxbulletDistance() {
        float height = shootingPoint.position.y;
        // Debug.Log(height);
        float v_x = shootVelocity;
        float time = - 2 * height / Physics.gravity.y;  
        time = Mathf.Sqrt(time); 
        maxbulletDistance = v_x * time;
    }


    void Start() {
        CalculateMaxbulletDistance();
        _cooldown = 1.0f / fireRate;
        agent_rigidbody = agent.GetComponent<Rigidbody>();
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
            rb.AddForce(shootVelocity * shootingPoint.forward, ForceMode.VelocityChange);

        }
    }

    void Update() {

        _cooldown -= Time.deltaTime;
        Vector3 direction = (agent.transform.position - shootingPoint.position);
        
        if (direction.magnitude > maxbulletDistance) {
            turretBase.LookAt(agent.transform);
            return;
        }
        if (Physics.Raycast(shootingPoint.position, direction.normalized, out var hit, Mathf.Infinity)) {
            // Debug.DrawRay(shootingPoint.position, direction.normalized * hit.distance, Color.red);
            if (hit.collider.gameObject != agent) {
                turretBase.LookAt(agent.transform);
                return;
            }
        }else{
            return;
        }
        
        Vector2 a = new Vector2(agent.transform.position.x, agent.transform.position.z);
        Vector2 b = new Vector2(shootingPoint.position.x, shootingPoint.position.z);
        Vector2 vA = new Vector2(agent_rigidbody.velocity.x, agent_rigidbody.velocity.z);
        float sB = shootVelocity;

        if (InterceptionDirection(a, b, vA, sB, out var prediction)){
                var angle = Vector2.SignedAngle(Vector2.up, prediction);
                Quaternion lookRotation = Quaternion.Euler(0, -angle, 0);
                turretBase.rotation = Quaternion.Slerp(turretBase.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                if (_cooldown <= 0.0f) {
                    Shoot();
                    _cooldown = 1.0f / fireRate;
                }
        }

        
    }

    public bool InterceptionDirection(Vector2 a, Vector2 b, Vector2 vA, float sB, out Vector2 result){
        var aToB = b - a;
        var dC = aToB.magnitude;
        var alpha = Vector2.Angle(aToB, vA) * Mathf.Deg2Rad;
        var sA = vA.magnitude;
        var r = sA / sB;
        if (SolveQuadratic(1 - r * r, 2 * r * dC * Mathf.Cos(alpha), -dC * dC, out var root1, out var root2) == 0){
            result = Vector2.zero;
            return false;
        }
        var dA = Mathf.Max(root1, root2);
        var t = dA / sB;
        var c = a + t * vA;
        result = (c - b).normalized;
        return true;
    }


    public static int SolveQuadratic(float a, float b, float c, out float root1, out float root2){
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0){
            root1 = root2 = 0;
            return 0;
        }   
        else if (discriminant == 0){
            root1 = root2 = -b / (2 * a);
            return 1;
        }else{
            root1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            root2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            return 2;
        }
    }
}
