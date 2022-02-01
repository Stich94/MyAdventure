using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAI))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyAI foV = (EnemyAI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(foV.transform.position, Vector3.up, Vector3.forward, 360, foV.Radius);

        Vector3 viewAngle01 = DirectionFromAngle(foV.transform.eulerAngles.y, -foV.SightAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(foV.transform.eulerAngles.y, foV.SightAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(foV.transform.position, foV.transform.position + viewAngle01 * foV.Radius);
        Handles.DrawLine(foV.transform.position, foV.transform.position + viewAngle02 * foV.Radius);

        if (foV.CanSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(foV.transform.position, foV.PlayerPosition.transform.position);
        }
    }



    private Vector3 DirectionFromAngle(float eulerY, float angleInDegress)
    {
        angleInDegress += eulerY;

        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
    }
}
