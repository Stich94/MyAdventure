using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform enemyPos;
    [SerializeField] protected Transform playerPos;
    public Transform PlayerPosition { set { playerPos = value; } }
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;

    // patroling 
    [SerializeField] protected Vector3 walkpoint;
    float randomZ;
    float randomX;
    protected bool walkPointSet;
    [SerializeField] protected float walkPointRange;

    // Attacking
    [Header("Attack Variables")] [SerializeField] protected float attackSpeed;
    [Header("Attack Variables")] [SerializeField] protected float attackRange;
    [SerializeField] protected float timeBetweenAttacks;
    protected bool alreadyAttacked;
    protected bool canAttack = true;

    // States
    [SerializeField] protected float sightRange;
    [SerializeField] protected bool playerIsInSightRange;
    [SerializeField] protected bool playerIsInAttackRange;
    public bool HasBeenAttacked { get { return alreadyAttacked; } set { alreadyAttacked = value; } }


    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Patrouling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkpoint);

        Vector3 distanceToWalkPoint = transform.position - walkpoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    protected void SearchWalkPoint()
    {
        // Calculate random point in range
        randomZ = Random.Range(-walkPointRange, walkPointRange);
        randomX = Random.Range(-walkPointRange, walkPointRange);

        walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkpoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;

    }

    protected void ChasePlayer()
    {
        agent.SetDestination(playerPos.position);
    }

    protected void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(playerPos);
        if (!alreadyAttacked)
        {
            //TODO - Attack code here


            alreadyAttacked = true;
            // Invoke(nameof(ResetAttack), timeBetweenAttacks); //TODO
        }
    }

    protected void ResetAttack()
    {

    }

    IEnumerator AttackDelay(float _timebetweenAttacks)
    {
        agent.speed = 0;
        canAttack = false;
        yield return new WaitForSeconds(_timebetweenAttacks);

        if (!playerIsInAttackRange)
        {
            //TODO - set animation
            // move
        }
        // agent.speed = GetComponent<BaseStats>();
        canAttack = true;

    }

    protected void CheckIfPlayerIsInSightRange()
    {
        playerIsInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        if (!playerIsInSightRange && !playerIsInSightRange) Patrouling();
        if (playerIsInSightRange && !playerIsInAttackRange) ChasePlayer();
        if (playerIsInAttackRange && playerIsInSightRange) AttackPlayer();

    }

    protected void CheckIfPlayerIsInAttackRange()
    {
        playerIsInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
