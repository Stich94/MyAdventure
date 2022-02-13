using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDeathState : AiStateI
{
    public void Enter(AiAgent _agent)
    {
        _agent.GetNavAgent.isStopped = true;
        _agent.GetRagDoll.ActivateRagdoll();
        // Destroy(this.gameObject, 8f);
    }

    public void Exit(AiAgent agent)
    {

    }

    public AiStateId GetId()
    {
        return AiStateId.Death;
    }

    public void Update(AiAgent agent)
    {

    }

}
