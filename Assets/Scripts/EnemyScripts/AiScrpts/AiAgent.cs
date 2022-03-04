using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class AiAgent : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    public Transform PlayerPos { get { return playerPos; } set { playerPos = value; } }
    [SerializeField] AiStateMachine stateMachine;
    [SerializeField] AiStateId initialState;
    [SerializeField] AiStateId currentState;
    public AiStateId GetCurrentAiState => currentState;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] AiAgentConfig config;

    public AiStateMachine GetAiStateMachine { get { return stateMachine; } set { stateMachine = value; } }

    public RagDoll GetRagDoll { get { return ragdoll; } }
    public AiAgentConfig GetConfig { get { return config; } }

    public NavMeshAgent GetNavAgent { get { return navMeshAgent; } }



    [Header("Current Weapon Equipped")]
    [SerializeField] EnemyWeapon aiEnemyWeapon;
    RagDoll ragdoll;
    AiWeapons aiWeapon;
    public AiWeapons AiWeapon => aiWeapon;

    [Header("Aim Rig")]
    [SerializeField] Rig aimRig;
    float aimRigWeigth = 0f;


    BaseStats playerStats;


    void Start()
    {
        if (playerPos == null)
        {

            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            playerStats = playerPos.gameObject.GetComponent<BaseStats>();
        }
        aiWeapon = GetComponent<AiWeapons>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        ragdoll = GetComponent<RagDoll>();
        RegisterAiStates();


        SetCurrentWeapon();
        // ragdoll.DisableRigidbody();

        // disable AimRig at beginning
        aimRig.weight = 0f;

    }

    void Update()
    {
        stateMachine.Update();
        currentState = stateMachine.GetCurrentState();
        CheckIfAIAgentisInAttackMode();
        CheckIfPlayerIsDead();
        CheckIfPlayerIsPatrol();
    }

    void RegisterAiStates()
    {
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState()); // create a new instance of our enemy state
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiPatrolState());
        stateMachine.RegisterState(new AiAttackPlayerState());
        stateMachine.ChangeState(initialState);
    }

    void SetCurrentWeapon()
    {
        aiEnemyWeapon = GetComponentInChildren<EnemyWeapon>();
        aiWeapon.Equip(aiEnemyWeapon);
        aiWeapon.WeaponIsAcive = true;
    }

    void CheckIfAIAgentisInAttackMode()
    {
        if (currentState == AiStateId.AttackPlayer)
        {
            aimRig.weight = 1f;
            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeigth, Time.deltaTime * 20f);
        }
    }

    void CheckIfPlayerIsDead()
    {
        if (playerStats.isDead)
        {
            stateMachine.ChangeState(initialState);
        }
    }

    void CheckIfPlayerIsPatrol()
    {
        if (currentState == AiStateId.Patrol || currentState == AiStateId.Idle)
        {
            aimRig.weight = 0f;
        }
    }

    public void ChangeToAttackState()
    {
        stateMachine.ChangeState(AiStateId.AttackPlayer);
    }


}
