using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    // only responsible for Animation
    [SerializeField] Animator animator;
    [SerializeField] Transform playerPos;
    NavMeshAgent agent;

    int animatorMovementId;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animatorMovementId = Animator.StringToHash("Speed");
    }

    void Update()
    {

        if (agent.hasPath)
        {
            animator.SetFloat(animatorMovementId, agent.velocity.magnitude);

        }
        else
        {
            animator.SetFloat(animatorMovementId, 0f);
        }

    }
}
