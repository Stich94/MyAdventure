using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiPatrolState : AiStateI
{
    ThirdPersonMovementController playerController;
    AiLocomotion aiLocomotion;
    float speedWalk = 2f;

    float startWaitTime = 6f;
    float timeToRotate = 2f;

    public Transform[] waypoints;
    int currentWayPointIndex;

    float waitTime;

    bool playerIsNear = false;

    Vector3 playerDirection;
    Vector3 agentDirection;


    float timer = 0.0f;

    public void Enter(AiAgent _agent)
    {
        _agent.GetNavAgent.isStopped = false;
        _agent.GetNavAgent.speed = speedWalk;
        waypoints = _agent.gameObject.GetComponent<WayPoints>()?.GetWayPoints();
        _agent.GetNavAgent.SetDestination(waypoints[currentWayPointIndex].position);
    }

    public void Exit(AiAgent _agent)
    {
        _agent.GetNavAgent.isStopped = false;

    }

    public AiStateId GetId()
    {
        return AiStateId.Patrol;
    }

    public void Update(AiAgent _agent)
    {
        CheckIfPlayerIsInRange(_agent);
        Patroling(_agent);


    }

    void CheckIfPlayerIsInRange(AiAgent _agent)
    {
        playerDirection = _agent.PlayerPos.position - _agent.transform.position;
        if (playerDirection.magnitude > _agent.GetConfig.maxSightDistance)
        {
            return;
        }

        agentDirection = _agent.transform.forward;
        agentDirection.Normalize();

        // if player is in Range change State
        float dotProduct = Vector3.Dot(playerDirection, agentDirection);
        if (dotProduct > 0.0f)
        {
            playerIsNear = true;
            _agent.GetAiStateMachine.ChangeState(AiStateId.ChasePlayer);

        }
    }

    void GoToNextCheckPoint(AiAgent _agent)
    {
        if (waitTime <= 0f)
        {
            _agent.GetNavAgent.SetDestination(waypoints[currentWayPointIndex].position);
            waitTime = startWaitTime;
        }
        else
        {
            StopAiMovement(_agent);
            waitTime -= Time.deltaTime;
        }

    }

    void StopAiMovement(AiAgent _agent)
    {
        _agent.GetNavAgent.isStopped = true;
        _agent.GetNavAgent.speed = 0f;
    }

    void NextWayPoint(AiAgent _agent)
    {
        currentWayPointIndex = (currentWayPointIndex + 1) % waypoints.Length;
        _agent.GetNavAgent.SetDestination(waypoints[currentWayPointIndex].position);
    }

    void Patroling(AiAgent _agent)
    {
        if (!playerIsNear)
        {
            if (waypoints == null)
            {
                waypoints = _agent.gameObject.GetComponent<WayPoints>()?.GetWayPoints();
            }
            _agent.GetNavAgent.SetDestination(waypoints[currentWayPointIndex].position);
            if (_agent.GetNavAgent.remainingDistance <= _agent.GetNavAgent.stoppingDistance)
            {
                if (waitTime <= 0f)
                {
                    NextWayPoint(_agent);

                    waitTime = startWaitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

}
