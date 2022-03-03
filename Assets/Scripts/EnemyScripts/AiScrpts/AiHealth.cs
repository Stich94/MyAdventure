using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiHealth : BaseStats
{
    [SerializeField] StatsScrptableObject enemyStats;
    [SerializeField] AiAgent modifiedAiAgent;
    [SerializeField] Canvas canvas;
    NavMeshAgent agent;

    RagDoll ragdoll;

    EnemyAI meleeEnemyAi;

    bool inAttackStance = false;

    protected override void Awake()
    {
        canvas.enabled = false;

        ragdoll = GetComponent<RagDoll>();
        agent = GetComponent<NavMeshAgent>();
        modifiedAiAgent = GetComponent<AiAgent>();
        SetStats();
    }

    protected override void OnStart()
    {
        meleeEnemyAi = GetComponent<EnemyAI>();
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

    public override void TakeDamage(float _damage, Vector3 _direction)
    {
        canvas.enabled = true;
        // OnDamage(_direction);
        Debug.Log("takes damage");
        currentHealth -= _damage;
        OnDamage();
        if (currentHealth <= 0.0f)
        {
            if (meleeEnemyAi != null)
            {
                meleeEnemyAi.IsDead = true;
                Die();
            }
            else
            {
                Die();

            }
        }
        // blinkTimer = blinkDuration;

        // ShowHitEffect();
    }

    public override void TakeDamage(float _damage)
    {
        canvas.enabled = true;
        Debug.Log("takes damage");
        currentHealth -= _damage;
        OnDamage();
        if (currentHealth <= 0.0f)
        {
            if (meleeEnemyAi != null)
            {
                meleeEnemyAi.IsDead = true;
                Die();
            }
            else
            {
                Die();

            }
        }
    }

    public override void Die()
    {
        OnDeath();
    }


    protected override void OnDeath()
    {
        if (modifiedAiAgent != null)
        {
            AiDeathState deathState = modifiedAiAgent?.GetAiStateMachine.GetState(AiStateId.Death) as AiDeathState;
            modifiedAiAgent?.GetAiStateMachine.ChangeState(AiStateId.Death);
            modifiedAiAgent?.AiWeapon.SetFiring(false);
        }

        ragdoll.ActivateRagdoll();
        Destroy(this.gameObject, 8f);
    }



    protected override void OnDamage()
    {
        if (currentHealth < maxHealth && !inAttackStance)
        {
            Debug.Log("Enemy entereed onDamage");
            // change enemy state to attack
            modifiedAiAgent?.ChangeToAttackState();
            inAttackStance = true;
        }
        Soundmanager.PlaySound(Soundmanager.Sound.EnemyHit, transform.position);
    }

}
