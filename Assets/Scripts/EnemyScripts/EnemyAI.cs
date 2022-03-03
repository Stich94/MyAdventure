using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] protected StatsScrptableObject stats;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform enemyPos;
    [SerializeField] protected Transform playerPos;
    public Transform PlayerPosition { get { return playerPos; } set { playerPos = value; } }
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask enemyLayer;


    // Field of View
    [Header("Field of View")][SerializeField] float radius;
    public float Radius { get { return radius; } }
    [SerializeField][Range(0, 360)] float angle;
    public float SightAngle { get { return angle; } }
    [SerializeField] float CheckDelay = .2f;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructedLayerMask;
    [SerializeField] bool canSeePlayer = false;
    [SerializeField] float wallCheckRadius = 3f;
    public bool CanSeePlayer { get { return canSeePlayer; } }

    // patroling 
    [Header("Patrouling")][SerializeField] protected Vector3 walkpoint;
    float randomZ;
    float randomX;
    protected bool walkPointSet;
    [SerializeField] protected float walkPointRange;

    // Attacking
    [Header("Attack Variables")][SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float timeBetweenAttacks;
    protected bool alreadyAttacked;
    protected bool canAttack = true;

    float cooldown = 4f;
    float cooldownTimer;

    // States
    [Header("States")][SerializeField] protected float sightRange;
    [SerializeField] protected bool playerIsInSightRange;
    [SerializeField] protected bool playerIsInAttackRange;
    public bool HasBeenAttacked { get { return alreadyAttacked; } set { alreadyAttacked = value; } }
    public bool IsDead { get; set; } = false;

    EnemyMeleeCombat combat;


    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<EnemyMeleeCombat>();

    }

    protected void Start()
    {
        SetNavMeshStats();
        StartCoroutine(FoVRoutine()); //TODO - Fow 
    }

    protected void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (!IsDead)
        {
            if (!alreadyAttacked)
            {
                CheckForWall();
            }
            CheckIfPlayerIsInSightRange();
            CheckIfPlayerIsInAttackRange();
        }

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

    protected void SetNavMeshStats()
    {
        agent.speed = stats.movementSpeed;
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
        float distance = Vector3.Distance(playerPos.position, transform.position);
        if (distance >= agent.stoppingDistance && canSeePlayer)
        {
            agent.isStopped = true;
            agent.SetDestination(playerPos.position);
        }
        agent.isStopped = false;
        transform.LookAt(playerPos);
    }

    protected void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(playerPos);

        // !alreadyAttacked

        if (cooldownTimer <= 0)
        {
            //TODO - Attack code here
            Debug.Log("Enemy attacked Player");
            BaseStats playerStats = playerPos.gameObject.GetComponent<BaseStats>();
            if (playerStats != null)
            {
                combat.Attack(playerStats);
                cooldownTimer = cooldown;
            }


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

        if (!canSeePlayer && !playerIsInSightRange) Patrouling();
        if (playerIsInSightRange && !playerIsInAttackRange) ChasePlayer();
        if (playerIsInAttackRange && playerIsInSightRange) AttackPlayer();

    }

    protected void CheckIfPlayerIsInAttackRange()
    {
        playerIsInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
    }

    protected void CheckForWall()
    {
        if (Physics.CheckSphere(transform.position, wallCheckRadius, wallLayer))
        {
            SearchWalkPoint();
        }
    }

    protected void FoVCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRange, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructedLayerMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    IEnumerator FoVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CheckDelay);
            Debug.Log("Entered FoV Check");
            FoVCheck();
        }
    }


    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


}
