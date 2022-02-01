using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyScriptables/Enemy")]
public class StatsScrptableObject : ScriptableObject
{
    public float damage;
    public float maxHp;
    public float attackSpeed;
    public float movementSpeed;
}
