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

    RagDoll ragdoll;

    AiAgent modifiedAiAgent;

    ThirdPersonMovementController playerController;
    ThirdPersonShooterController shootController;
    void Awake()
    {
        ragdoll = GetComponent<RagDoll>();
        agent = GetComponent<NavMeshAgent>();
        modifiedAiAgent = GetComponent<AiAgent>();
        SetStats();
    }

    private void Start()
    {
        AddHitBoxComponents();
        OnStart();
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

        // apply the stats to our NavMesh Agent
        agent.speed = movementSpeed;
    }

    void ClampHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
    }

    public virtual void TakeDamage(float _damage, Vector3 _direction)
    {
        OnDamage(_direction);
        Debug.Log("takes damage");
        currentHealth -= _damage;
        if (currentHealth <= 0.0f)
        {
            Die(_direction);
        }
        // blinkTimer = blinkDuration;

        // ShowHitEffect();
    }

    public virtual void Die(Vector3 _direction)
    {
        OnDeath(_direction);
        //     AiDeathState deathState = modifiedAiAgent.GetAiStateMachine.GetState(AiStateId.Death) as AiDeathState;
        //     modifiedAiAgent.GetAiStateMachine.ChangeState(AiStateId.Death);
        //     Destroy(this.gameObject, 8f);
        // }
    }
    public void TakeDamage(float _damage)
    {

    }

    // add hitbox and point to our basestats script
    void AddHitBoxComponents()
    {
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rb.Length; i++)
        {
            HitBox hitbox = rb[i].gameObject.AddComponent<HitBox>();
            hitbox.Health = this; // we assign our basestats to the hixbox
                                  // if (hitbox.gameObject != gameObject)
                                  // {
                                  //     hitbox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
                                  // }
        }
    }

    protected virtual void OnStart()
    {


    }

    protected virtual void OnDeath(Vector3 _direction)
    {
        ragdoll.ActivateRagdoll();
        playerController.enabled = false;
        shootController.enabled = false;

    }

    protected virtual void OnDamage(Vector3 _direction)
    {

    }
}

