using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiIdleState : AiStateI
{
    public void Enter(AiAgent agent)
    {

    }

    public void Exit(AiAgent agent)
    {

    }

    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Update(AiAgent _agent)
    {
        Vector3 playerDirection = _agent.PlayerPos.position - _agent.transform.position;
        if (playerDirection.magnitude > _agent.GetConfig.maxSightDistance)
        {
            return;
        }

        Vector3 agentDirection = _agent.transform.forward;
        agentDirection.Normalize();

        float dotProduct = Vector3.Dot(playerDirection, agentDirection);
        if (dotProduct > 0.0f)
        {
            _agent.GetAiStateMachine.ChangeState(AiStateId.ChasePlayer);

        }

    }


}
