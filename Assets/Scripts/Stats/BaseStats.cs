using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseStats : MonoBehaviour, IDamageAble
{
    [SerializeField] StatsScrptableObject stats;
    [SerializeField] protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    [SerializeField] protected float currentHealth;
    public float CurrentHealth { get { return currentHealth; } }
    [SerializeField] protected float damage;
    public float Damage { get { return damage; } }
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float movementSpeed;
    float blinkTimer;
    NavMeshAgent agent;

    AiAgent modifiedAiAgent;
    void Awake()
    {
        SetStats();
        agent = GetComponent<NavMeshAgent>();
        modifiedAiAgent = GetComponent<AiAgent>();
    }

    protected virtual void Update()
    {
        ClampHealth();
        // blinkTimer -= Time.deltaTime;
        // ShowHitEffect();

    }

    protected virtual void SetStats()
    {
        maxHealth = stats.maxHp;
        currentHealth = stats.maxHp;
        stats.currentHp = currentHealth;
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
        movementSpeed = stats.movementSpeed;
    }

    void ClampHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
    }

    public virtual void TakeDamage(float _damage)
    {
        Debug.Log("takes damage");
        currentHealth -= _damage;
        if (currentHealth <= 0.0f)
        {
            Die();
        }
        // blinkTimer = blinkDuration;

        // ShowHitEffect();
    }

    public virtual void Die()
    {
        AiDeathState deathState = modifiedAiAgent.GetAiStateMachine.GetState(AiStateId.Death) as AiDeathState;
        modifiedAiAgent.GetAiStateMachine.ChangeState(AiStateId.Death);
        Destroy(this.gameObject, 8f);
    }

}