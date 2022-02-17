using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    public Transform PlayerPos { get { return playerPos; } set { playerPos = value; } }
    [SerializeField] AiStateMachine stateMachine;
    [SerializeField] AiStateId initialState;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] AiAgentConfig config;
    public AiStateMachine GetAiStateMachine { get { return stateMachine; } set { stateMachine = value; } }

    public RagDoll GetRagDoll { get { return ragdoll; } }
    public AiAgentConfig GetConfig { get { return config; } }

    public NavMeshAgent GetNavAgent { get { return navMeshAgent; } }
    RagDoll ragdoll;

    AiWeapons aiWeapon;
    public AiWeapons AiWeapon => aiWeapon;
    [Header("Current Weapon Equipped")]
    [SerializeField] EnemyWeapon aiEnemyWeapon;
    [SerializeField] string currentAiState;
    void Start()
    {
        if (playerPos == null)
        {

            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        }
        aiWeapon = GetComponent<AiWeapons>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        ragdoll = GetComponent<RagDoll>();
        RegisterAiStates();

        SetCurrentWeapon();
        // ragdoll.DisableRigidbody();
    }

    void Update()
    {
        stateMachine.Update();
        stateMachine.GetCurrentState();
    }

    void RegisterAiStates()
    {
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState()); // create a new instance of our enemy state
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiAttackPlayerState());
        stateMachine.ChangeState(initialState);
    }

    void SetCurrentWeapon()
    {
        aiEnemyWeapon = GetComponentInChildren<EnemyWeapon>();
        aiWeapon.Equip(aiEnemyWeapon);
        aiWeapon.WeaponIsAcive = true;
    }

}
