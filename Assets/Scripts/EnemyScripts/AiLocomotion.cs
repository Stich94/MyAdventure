using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] Animator animator;
    [SerializeField] float maxTime = 1.0f;
    [SerializeField] float maxDistance = 1.0f;

    float timer = 0.0f;
    NavMeshAgent agent;

    int animatorMovementId;

    // Animator
    [SerializeField] float animationSmoothTime = 0.1f; // the lower the value, the faster is the transition
    [SerializeField] float animationPlayTransition = 0.15f;
    [SerializeField] Transform aimTarget;
    [SerializeField] float aimDistance = 1f;
    int moveXAnimationParameterId;
    int moveZAnimationParameterId;
    int jumpAnimation;
    float animationBlend;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelcity;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorMovementId = Animator.StringToHash("Speed");
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0.01f)
        {
            float sqDistance = (playerPos.position - agent.destination).sqrMagnitude;
            if (sqDistance > maxDistance * maxDistance)
            {
                agent.destination = playerPos.transform.position;

            }
            timer = maxTime; // reset the timer
        }


        animator.SetFloat(animatorMovementId, agent.velocity.magnitude);
    }
}
