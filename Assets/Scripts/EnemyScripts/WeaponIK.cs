using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    public float weight = 1.0f;
}

public class WeaponIK : MonoBehaviour
{

    [SerializeField] Transform targetTransform;
    [SerializeField] Transform aimTransform;
    [SerializeField] Transform bone; // not used
    [SerializeField] int iterations = 10;
    [SerializeField] float angleLimit = 90.0f;
    [SerializeField] float distanceLimit = 1.5f;

    [Range(0, 1)]
    [SerializeField] float weight = 1f;

    [SerializeField] HumanBone[] humanBones;
    Transform[] boneTransforms;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }



    private void LateUpdate()
    {
        if (aimTransform == null) return;
        if (targetTransform == null) return;


        Vector3 targetPosition = GetTargetPosition();
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < boneTransforms.Length; j++)
            {
                Transform bone = boneTransforms[j];
                float boneWeight = humanBones[j].weight * weight;
                AimAtTarget(bone, targetPosition);
            }

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

    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = targetTransform.position - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0.0f;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);

        // clamp angle
        if (targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }

        // check for distance to player - only look forward if to close
        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;
        }

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }

    public void SetTargetTransform(Transform _target)
    {
        targetTransform = _target;
    }
    public void SetAimTransform(Transform _aim)
    {
        aimTransform = _aim;
    }
}
