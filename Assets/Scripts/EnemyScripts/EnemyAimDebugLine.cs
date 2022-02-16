using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAimDebugLine : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 50);
    }
}
