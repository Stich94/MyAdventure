using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIK : MonoBehaviour
{

    [SerializeField] Transform targetTransform;
    [SerializeField] Transform aimTransform;
    [SerializeField] Transform bone;
    [SerializeField] int iterations;

    [Range(0, 1)]
    [SerializeField] float weight = 1.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        Vector3 targetPosition = targetTransform.position;
        for (int i = 0; i < iterations; i++)
        {
            AimAtTarget(bone, targetPosition);

        }
    }

    void AimAtTarget(Transform bone, Vector3 _targetPosition)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = _targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }
}
