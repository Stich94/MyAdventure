using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "playerStats/playerScriptableSO")]
public class playerStatsSO : ScriptableObject
{
    public float CurrentHP;
    public float MaxHP = 100f;
    public float SprintSpeed;
    public float movementSpeed = 0.0f;
    public float jumpHeight;


    // void Awake()
    // {
    //     CurrentHP = MaxHP;
    // }
}
