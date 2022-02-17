using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackPlayerState : AiStateI
{
    public void Enter(AiAgent _agent)
    {
        _agent.AiWeapon.SetTarget(_agent.PlayerPos);
        _agent.GetNavAgent.stoppingDistance = 5.0f;
        // _agent.AiWeapon.SetFiring(true);
    }
    public void Exit(AiAgent _agent)
    {
        _agent.GetNavAgent.stoppingDistance = 0.0f;
    }

    public AiStateId GetId()
    {
        return AiStateId.AttackPlayer;
    }

    public void Update(AiAgent _agent)
    {
        _agent.GetNavAgent.destination = _agent.PlayerPos.position;
    }

}
