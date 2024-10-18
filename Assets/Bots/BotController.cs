using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using ResetRequest = RosMessageTypes.InterfacesPkg.UC2EnvironmentResetRequest;
public class BotController : MonoBehaviour
{
    // Dropdown in the Inspector for selecting difficulty
    [Header("Bot Movement")]
    // Waypoints for the bot to move through
    public List<Transform> waypoints;
    public bool followWaypoints;
    private int currentWaypointIndex = 0;


    [Header("Bot Shooting")]
    // Reference to shooting script or functionality
    public GameObject bulletPrefab;
    public GameObject agent;
    public bool canShoot;
    public float range;
    public float turretRotationSpeed;
    public float fireRate;
    public float angleError;
    public NavMeshAgent navAgent;
    private float _shootVelocity;
    private float _cooldown;
    private Rigidbody agent_rigidbody;
    private Transform turretBase;
    private Transform shootingPoint;
    

    private void Start()
    {
        _cooldown = 1.0f / fireRate;
        navAgent = transform.GetComponent<NavMeshAgent>();
        agent_rigidbody = agent.GetComponent<Rigidbody>();
        turretBase = transform.Find("TankRenderers/TankTurret").transform;
        shootingPoint = turretBase.transform.Find("ShootingPoint").transform;
        _shootVelocity = range / Mathf.Sqrt(2 * shootingPoint.position.y / Mathf.Abs(Physics.gravity.y));
    }

    public void ResetBot(ResetRequest request){
        navAgent.speed = request.bot_params.speed;
        fireRate = request.bot_params.fire_rate; 
        canShoot = request.bot_params.can_shoot;
        followWaypoints = request.bot_params.follow_waypoints;
        range = request.bot_params.range;
        turretRotationSpeed = request.bot_params.turret_rotation_speed;
        angleError = request.bot_params.angle_error;
        _cooldown = 1.0f / fireRate;
        _shootVelocity = range / Mathf.Sqrt(2 * shootingPoint.position.y / Mathf.Abs(Physics.gravity.y));
    }

    public void SetPosition(Vector3 position){
        navAgent.Warp(position);
    }

    private void FixedUpdate()
    {
        if (navAgent.speed > 0.0f){
            if (followWaypoints){
                navAgent.stoppingDistance = 2.0f;
                GoToNextWaypoint();
            }else{
                navAgent.stoppingDistance = Mathf.Min(range, 8.0f);
                navAgent.SetDestination(agent.transform.position);
            }
        }

        if (canShoot)
        {
            TryShoot();
        }
    }

    void GoToNextWaypoint() {
        if (waypoints.Count == 0) return;  // Exit if there are no waypoints

        // Check if the agent is close enough to the current waypoint to consider it "arrived"
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance + 1.0f)
        {
            // Get new random waypoint index
            int newWaypointIndex = Random.Range(0, waypoints.Count);
            currentWaypointIndex = currentWaypointIndex == newWaypointIndex ? (currentWaypointIndex + 1) % waypoints.Count : newWaypointIndex;
            
            // Set the agent's destination to the next waypoint
            navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void TryShoot(){
        _cooldown -= Time.fixedDeltaTime;
        Vector3 direction = new Vector3(agent.transform.position.x, shootingPoint.position.y, agent.transform.position.z) - shootingPoint.position;

        if (_cooldown > 0.0f){
            // Debug.DrawRay(shootingPoint.position, direction, Color.blue);  
            turretBase.LookAt(new Vector3(agent.transform.position.x, turretBase.position.y, agent.transform.position.z));
            return; 
        }
        
        if (direction.magnitude > range) {
            // Debug.DrawRay(shootingPoint.position, direction, Color.green);
            turretBase.LookAt(new Vector3(agent.transform.position.x, turretBase.position.y, agent.transform.position.z));
            return;
        }
        
        if (Physics.Raycast(shootingPoint.position, direction.normalized, out var hit, Mathf.Infinity)) {
            if (hit.collider.gameObject != agent) {
                // Debug.DrawRay(shootingPoint.position, direction, Color.red);
                turretBase.LookAt(new Vector3(agent.transform.position.x, turretBase.position.y, agent.transform.position.z));
                return;
            }
        }else{
            return;
        }
        
        Vector2 a = new Vector2(agent.transform.position.x, agent.transform.position.z);
        Vector2 b = new Vector2(shootingPoint.position.x, shootingPoint.position.z);
        Vector2 vA = new Vector2(agent_rigidbody.velocity.x, agent_rigidbody.velocity.z);
        float sB = _shootVelocity;

        _cooldown = 1.0f / fireRate;
        if (InterceptionDirection(a, b, vA, sB, out var prediction))
        {
            var angle = Vector2.SignedAngle(Vector2.up, prediction);
            Quaternion lookRotation = Quaternion.Euler(0, -(angle + Random.Range(-angleError, angleError)), 0);
            turretBase.rotation = Quaternion.Slerp(turretBase.rotation, lookRotation, Time.fixedDeltaTime * turretRotationSpeed * Mathf.Rad2Deg);
            Shoot();
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
            rb.AddForce(_shootVelocity * shootingPoint.forward, ForceMode.VelocityChange);
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
