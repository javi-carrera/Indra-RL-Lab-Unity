using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class State {

    public enum STATE {

        IDLE,
        PATROL,
        PURSUE,
        ATTACK,
        SLEEP,
        RUNAWAY
    };

    public enum EVENT {

        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent;

    float visDist = 10.0f;
    float visAngle = 30.0f;
    float shootDist = 7.0f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) {

        npc = _npc;
        agent = _agent;
        anim = _anim;
        player = _player;
        stage = EVENT.ENTER;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public State Process() {

        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT) {

            Exit();
            return nextState;
        }

        return this;
    }

    public bool CanSeePlayer() {

        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);

        if (direction.magnitude < visDist && angle < visAngle) {

            return true;
        }

        return false;
    }

    public bool IsPlayerBehind() {

        Vector3 direction = npc.transform.position - player.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if (direction.magnitude < 2.0f && angle < 30.0f) return true;
        return false;
    }

    public bool CanAttackPlayer() {

        Vector3 direction = player.position - npc.transform.position;
        if (direction.magnitude < shootDist) {

            return true;
        }

        return false;
    }
}

public class BotAgentTracking : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Transform target;
    public List<Transform> waypoints;
    public float linearTimeConstant;
    public float angularTimeConstant;

    private int currentWaypointIndex = 0;
    public float arrivalThreshold = 1.0f;  // How close the agent needs to be to consider it "arrived"

    private Rigidbody _rb;
    private Vector3 _currentLinearVelocity;
    private Vector3 _currentAngularVelocity;

    private Vector3 dad;

    private void Start() {
        navAgent.updatePosition = true;  // Disable position updates
        navAgent.updateRotation = false;  // Disable rotation updates
        _rb = GetComponent<Rigidbody>();
        _currentLinearVelocity = Vector3.zero;
        _currentAngularVelocity = Vector3.zero;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
    
    private void Update()
    {
        // if (waypoints.Count == 0) return;  // Exit if there are no waypoints

        // // Check if the agent is close enough to the current waypoint to consider it "arrived"
        // if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance + 1)
        // {
        //     GoToNextWaypoint();
        // }

        navAgent.SetDestination(target.position);

        // ConstrainToForwardMovement();
    }

    private void ConstrainToForwardMovement()
    {
        Vector3 directionToTarget = (navAgent.steeringTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        
        
        // transform.position += transform.forward * navAgent.speed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * navAgent.angularSpeed);
        // navAgent.nextPosition = transform.position;
        // _rb.velocity = navAgent.desiredVelocity;

        transform.position = navAgent.nextPosition;
        
        // navAgent.velocity = _rb.velocity;

        Debug.Log("angular_velocity" + _rb.angularVelocity);
    }

    


    void GoToNextWaypoint() {
        // Increment the waypoint index
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;  // Loops back to the start when done

        // Set the agent's destination to the next waypoint
        navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        
    }
}
