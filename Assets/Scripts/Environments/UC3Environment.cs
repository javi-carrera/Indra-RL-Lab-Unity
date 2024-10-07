using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.BuiltinInterfaces;

using AgentType = UC3Agent;
using StateRequest = RosMessageTypes.InterfacesPkg.UC3EnvironmentStepRequest;
using StateResponse = RosMessageTypes.InterfacesPkg.UC3EnvironmentStepResponse;
using ResetRequest = RosMessageTypes.InterfacesPkg.UC3EnvironmentResetRequest;
using ResetResponse = RosMessageTypes.InterfacesPkg.UC3EnvironmentResetResponse;


public class UC3Environment : Environment<
    StateRequest,
    StateResponse,
    ResetRequest,
    ResetResponse> {
    
    [Header("UC3 Environment Settings")]
    public AgentType agent;
    public AgentType target;
    public List<Transform> spawnPoints;


    protected override void InitializeEnvironment() {

        // Append the agent to the list
        _agents = new List<IAgent> {
            agent,
            target,
        };

        // Initialize agents list
        foreach (IAgent agent in _agents) {
            agent.Initialize();
        }
        
    }


    protected override void Action(StateRequest request) {
        agent.Action(request.action);
        target.Action(request.target_action);
    }


    protected override StateResponse State(TimeMsg requestReceivedTimestamp) {

        Debug.Log("State requested");


        // Get the state from the agent
        StateResponse response = new() {
            state = agent.State(),
            target_state = target.State(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        Debug.Log("State sent");

        return response;
    }


    protected override ResetResponse ResetEnvironment(ResetRequest request, TimeMsg requestReceivedTimestamp) {

        Debug.Log("Reset requested");

        // Override reset
        if (overrideReset) return new ResetResponse{
            state = agent.State(),
            target_state = target.State(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        // Choose (different) random spawn points for the agent and the target
        int agentSpawnPointIndex = Random.Range(0, spawnPoints.Count);
        int targetSpawnPointIndex = Random.Range(0, spawnPoints.Count);

        while (targetSpawnPointIndex == agentSpawnPointIndex) {
            targetSpawnPointIndex = Random.Range(0, spawnPoints.Count);
        }

        // Move the agent and the target to the spawn points
        agent.transform.SetPositionAndRotation(spawnPoints[agentSpawnPointIndex].position, spawnPoints[agentSpawnPointIndex].rotation);
        target.transform.SetPositionAndRotation(spawnPoints[targetSpawnPointIndex].position, spawnPoints[targetSpawnPointIndex].rotation);

        // Reset the agent
        ResetResponse response = new() {
            state = agent.ResetAgent(),
            target_state = target.ResetAgent(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        Debug.Log("Reset sent");

        return response;
    }
}
