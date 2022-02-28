using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiPatrol : MonoBehaviour
{
    public void Update(AiAgent _agent)
    {
        // if (!_agent.enabled) return;

        // timer -= Time.deltaTime;

        // if (!_agent.GetNavAgent.hasPath)
        // {
        //     _agent.GetNavAgent.destination = _agent.PlayerPos.position;
        // }

        // if (timer <= 0.0f)
        // {
        //     Vector3 direction = (_agent.PlayerPos.position - _agent.GetNavAgent.destination);
        //     direction.y = 0;
        //     if (direction.magnitude > _agent.GetConfig.maxDistance * _agent.GetConfig.maxDistance)
        //     {
        //         if (_agent.GetNavAgent.pathStatus != NavMeshPathStatus.PathPartial)
        //         {
        //             _agent.GetNavAgent.destination = _agent.PlayerPos.position;


        //         }
        //         _agent.GetAiStateMachine.ChangeState(AiStateId.AttackPlayer);
        //     }
        //     timer = _agent.GetConfig.maxTime;
        // }
    }
}
