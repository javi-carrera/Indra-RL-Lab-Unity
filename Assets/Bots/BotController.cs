using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.VisualScripting;
public enum BotDifficulty
{
    Easy,      // Level 1: Move through waypoints only
    Medium,    // Level 2: Move through waypoints and fire
    Hard       // Level 3: Move through waypoints, fire, and be faster
}

public class BotController : MonoBehaviour
{
    // Dropdown in the Inspector for selecting difficulty
    public BotDifficulty botDifficulty;

    [Header("Bot Movement")]
    // Waypoints for the bot to move through
    public float speed = 2f;
    public List<Transform> waypoints;
    private bool followWaypoints;
    private int currentWaypointIndex = 0;


    [Header("Bot Shooting")]
    // Reference to shooting script or functionality
    public GameObject bulletPrefab;
    public GameObject agent;
    public float shootVelocity = 25.0f;
    public float turretRotationSpeed = 100.0f;
    public float fireRate = 1.0f;
    

    private bool canShoot;
    private float _cooldown;
    private float maxbulletDistance;
    private Rigidbody agent_rigidbody;
    private Transform turretBase;
    private Transform shootingPoint;
    private NavMeshAgent navAgent;

    void Start()
    {
        _cooldown = 1.0f / fireRate;
        navAgent = gameObject.transform.GetComponent<NavMeshAgent>();
        agent_rigidbody = agent.GetComponent<Rigidbody>();
        turretBase = gameObject.transform.Find("TankRenderers/TankTurret").transform;
        shootingPoint = turretBase.transform.Find("ShootingPoint").transform;

        // Initialize bot behavior based on the selected difficulty
        CalculateMaxbulletDistance();
        SetBotDifficulty();
    }

    void FixedUpdate()
    {
        // Bot movement behavior
        if (followWaypoints){
            GoToNextWaypoint();
        }else{
            navAgent.SetDestination(agent.transform.position);
        }

        if (canShoot)
        {
            TryShoot();
        }
    }

    // Set bot behavior based on difficulty level
    void SetBotDifficulty()
    {
        switch (botDifficulty)
        {
            case BotDifficulty.Easy:
                // Easy: Only move between waypoints, no shooting
                followWaypoints = true;
                canShoot = false;
                break;

            case BotDifficulty.Medium:
                // Medium: Move and shoot
                followWaypoints = true;
                canShoot = true;
                break;

            case BotDifficulty.Hard:
                // Hard: Move, shoot, and faster
                followWaypoints = false;
                canShoot = true;
                break;
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
        

        if (direction.magnitude > maxbulletDistance) {
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
        float sB = shootVelocity;

        if (InterceptionDirection(a, b, vA, sB, out var prediction)){
            var angle = Vector2.SignedAngle(Vector2.up, prediction);
            Quaternion lookRotation = Quaternion.Euler(0, -angle, 0);
            turretBase.rotation = Quaternion.Slerp(turretBase.rotation, lookRotation, Time.fixedDeltaTime * turretRotationSpeed);
            if (_cooldown <= 0.0f) {
                Shoot();
                _cooldown = 1.0f / fireRate;
            }
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
            rb.AddForce(shootVelocity * shootingPoint.forward, ForceMode.VelocityChange);

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
    private void CalculateMaxbulletDistance() {
        float height = shootingPoint.position.y;
        // Debug.Log(height);
        float v_x = shootVelocity;
        float time = - 2 * height / Physics.gravity.y;  
        time = Mathf.Sqrt(time); 
        maxbulletDistance = v_x * time;
    }
}
