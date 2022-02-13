using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0, 360)] public float angle;
    [SerializeField] GameObject player;


    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstructedLayerMask;
    [SerializeField] bool canSeePlayer;
}
