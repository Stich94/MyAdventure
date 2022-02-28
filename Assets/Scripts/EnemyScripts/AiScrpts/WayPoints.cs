using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour
{

    [SerializeField] Transform[] waypoints;

    public Transform[] GetWayPoints()
    {
        return waypoints;
    }

}
