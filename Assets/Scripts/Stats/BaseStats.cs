using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    RagDoll ragdoll;

    void Awake()
    {
        SetStats();
        ragdoll = GetComponent<RagDoll>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected virtual void Update()
    {
        ClampHealth();
        blinkTimer -= Time.deltaTime;
        // ShowHitEffect();

    }

    protected virtual void SetStats()
    {
        maxHealth = stats.maxHp;
        currentHealth = stats.maxHp;
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
        ragdoll.ActivateRagdoll();
        // Destroy(this.gameObject, 8f);
    }

    void ShowHitEffect()
    {
        float lerp = Mathf.Clamp01(blinkTimer / blinkTimer);
        float intensity = (lerp * blinkIntensity); // the + 1f is required, otherwise it will be black
        // float intensity = (lerp * blinkIntensity) + 1.0f; // the + 1f is required, otherwise it will be black
        skinnedMeshRenderer.material.color = Color.white * intensity;
    }





}