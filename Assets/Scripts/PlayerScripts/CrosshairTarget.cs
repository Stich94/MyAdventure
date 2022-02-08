using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    Ray ray;
    RaycastHit hit;


    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HitInfo();
    }

    private void HitInfo()
    {
        ray.origin = mainCam.transform.position;
        ray.direction = mainCam.transform.forward;
        Physics.Raycast(ray, out hit);
        transform.position = hit.point;
    }
}
