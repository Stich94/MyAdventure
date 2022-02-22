using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiHealth : BaseStats
{
    [SerializeField] StatsScrptableObject enemyStats;
    AiAgent modifiedAiAgent;
    NavMeshAgent agent;

    RagDoll ragdoll;

    protected override void Awake()
    {
        ragdoll = GetComponent<RagDoll>();
        agent = GetComponent<NavMeshAgent>();
        modifiedAiAgent = GetComponent<AiAgent>();
        SetStats();
    }

    protected override void OnStart()
    {
        modifiedAiAgent = GetComponent<AiAgent>();
    }

    protected override void SetStats()
    {
        maxHealth = enemyStats.maxHp;
        currentHealth = enemyStats.maxHp;
        // enemyStats.currentHp = currentHealth;
        damage = enemyStats.damage;
        attackSpeed = enemyStats.attackSpeed;
        movementSpeed = enemyStats.movementSpeed;

        // apply the enemyStats to our NavMesh Agent
        agent.speed = movementSpeed;
    }

    protected override void OnDeath(Vector3 _direction)
    {
        AiDeathState deathState = modifiedAiAgent.GetAiStateMachine.GetState(AiStateId.Death) as AiDeathState;
        modifiedAiAgent.GetAiStateMachine.ChangeState(AiStateId.Death);
        modifiedAiAgent.AiWeapon.SetFiring(false);
        Destroy(this.gameObject, 8f);
    }

    protected override void OnDamage(Vector3 _direction)
    {

    }

}
