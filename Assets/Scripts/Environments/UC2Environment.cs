using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.BuiltinInterfaces;

using AgentType = UC2Agent;
using StateRequest = RosMessageTypes.InterfacesPkg.UC2EnvironmentStepRequest;
using StateResponse = RosMessageTypes.InterfacesPkg.UC2EnvironmentStepResponse;
using ResetRequest = RosMessageTypes.InterfacesPkg.UC2EnvironmentResetRequest;
using ResetResponse = RosMessageTypes.InterfacesPkg.UC2EnvironmentResetResponse;


public class UC2Environment : Environment<
    StateRequest,
    StateResponse,
    ResetRequest,
    ResetResponse> {
    
    [Header("UC2 Environment Settings")]
    public AgentType agent;
    public GameObject target;
    public List<Transform> spawnPoints;
    private BotController botController;

    protected override void InitializeEnvironment() {

        _agents = new List<IAgent> {
            agent
        };

        foreach (IAgent agent in _agents)
            agent.Initialize();

        botController = target.GetComponent<BotController>();
    }

    protected override void Action(StateRequest request) {
        agent.Action(request.action);
    }

    protected override StateResponse State(TimeMsg requestReceivedTimestamp) {

        StateResponse response = new() {
            state = agent.State(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        return response;
    }
    protected override ResetResponse ResetEnvironment(ResetRequest request, TimeMsg requestReceivedTimestamp) {

        if (overrideReset) return new ResetResponse{
            state = agent.State(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        int agentSpawnPointIndex = Random.Range(0, spawnPoints.Count);
        int targetSpawnPointIndex = Random.Range(0, spawnPoints.Count);

        while (targetSpawnPointIndex == agentSpawnPointIndex) {
            targetSpawnPointIndex = Random.Range(0, spawnPoints.Count);
        }

        agent.transform.SetPositionAndRotation(spawnPoints[agentSpawnPointIndex].position, spawnPoints[agentSpawnPointIndex].rotation);
        botController.navAgent.Warp(spawnPoints[targetSpawnPointIndex].position);
        
        botController.navAgent.speed = request.bot_params.speed;
        botController.fireRate = request.bot_params.fire_rate; 
        botController.canShoot = request.bot_params.can_shoot;
        botController.followWaypoints = request.bot_params.follow_waypoints;
        botController.range = request.bot_params.range;
        botController.turretRotationSpeed = request.bot_params.turret_rotation_speed;
        botController.angleError = request.bot_params.angle_error;

        ResetResponse response = new() {
            state = agent.ResetAgent(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        return response;
    }
}
