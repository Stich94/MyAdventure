using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyScriptables/Enemy")]
public class StatsScrptableObject : ScriptableObject
{
    public float damage;
    public float maxHp;
    public float currentHp = 0f;
    public float attackSpeed;
    public float movementSpeed;
}
