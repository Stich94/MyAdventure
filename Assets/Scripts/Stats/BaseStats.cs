using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseStats : MonoBehaviour, IDamageAble
{
    [SerializeField] StatsScrptableObject stats;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float blinkIntensity;
    [SerializeField] protected float blinkDuration;

    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    float blinkTimer;

    NavMeshAgent agent;
    RagDoll ragdoll;

    void Awake()
    {
        SetStats();
        ragdoll = GetComponent<RagDoll>();
        agent = GetComponent<NavMeshAgent>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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
        blinkTimer = blinkDuration;
        // ShowHitEffect();
    }

    public virtual void Die()
    {
        agent.isStopped = true;
        ragdoll.ActivateRagdoll();
        Destroy(this.gameObject, 8f);
        // Destroy(this.gameObject, 8f);
    }


    // not working - emission is always at 10
    void ShowHitEffect()
    {
        float lerp = Mathf.Clamp01(blinkTimer / blinkTimer);
        float intensity = (lerp * blinkIntensity) + 1.0f; // the + 1f is required, otherwise it will be black
                                                          // float intensity = (lerp * blinkIntensity) + 1.0f; // the + 1f is required, otherwise it will be black
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.material.color = Color.white * intensity;

        }
        else
        {
            Debug.LogWarning("No Skinned Material found on Enemy");
        }

    }





}