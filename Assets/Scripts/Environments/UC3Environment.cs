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

        _agents = new List<IAgent> {
            agent,
            target,
        };

        foreach (IAgent agent in _agents)
            agent.Initialize();
    }

    protected override void Action(StateRequest request) {
        agent.Action(request.action);
        target.Action(request.target_action);
    }

    protected override StateResponse State(TimeMsg requestReceivedTimestamp) {

        StateResponse response = new() {
            state = agent.State(),
            target_state = target.State(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        return response;
    }

    protected override ResetResponse ResetEnvironment(ResetRequest request, TimeMsg requestReceivedTimestamp) {

        if (overrideReset) return new ResetResponse{
            state = agent.State(),
            target_state = target.State(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        int agentSpawnPointIndex = Random.Range(0, spawnPoints.Count);
        int targetSpawnPointIndex = Random.Range(0, spawnPoints.Count);

        while (targetSpawnPointIndex == agentSpawnPointIndex) {
            targetSpawnPointIndex = Random.Range(0, spawnPoints.Count);
        }

        agent.transform.SetPositionAndRotation(spawnPoints[agentSpawnPointIndex].position, spawnPoints[agentSpawnPointIndex].rotation);
        target.transform.SetPositionAndRotation(spawnPoints[targetSpawnPointIndex].position, spawnPoints[targetSpawnPointIndex].rotation);

        ResetResponse response = new() {
            state = agent.ResetAgent(),
            target_state = target.ResetAgent(),
            request_received_timestamp = requestReceivedTimestamp,
            response_sent_timestamp = GetCurrentTimestamp(),
        };

        return response;
    }
}
