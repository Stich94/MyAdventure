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

    private void Awake()
    {
        SetStats();
    }

    protected virtual void Update()
    {
        ClampHealth();
    }

    protected virtual void SetStats()
    {
        maxHealth = stats.maxHp;
        currentHealth = stats.maxHp;
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
        movementSpeed = stats.movementSpeed;
    }

    private void ClampHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, currentHealth);
    }

    public virtual void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
    }


}