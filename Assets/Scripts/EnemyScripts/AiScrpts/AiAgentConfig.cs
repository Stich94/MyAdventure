using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyScriptables/AiConfig")]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime = 1.0f; // wait time until calculates new Path
    public float maxDistance = 1.0f;
    public float maxSightDistance = 5.0f;
}
