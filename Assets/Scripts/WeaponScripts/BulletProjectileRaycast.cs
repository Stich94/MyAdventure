using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectileRaycast : MonoBehaviour
{
    [SerializeField] Transform vfxHitGreen;
    [SerializeField] Transform vfxHitRed;
    [SerializeField] float moveSpeed;

    Vector3 targetPosition;


    public void Setup(Vector3 _targetPosition)
    {
        this.targetPosition = _targetPosition;
    }

    void Update()
    {
        float distanceBefore = Vector3.Distance(transform.position, targetPosition);

        Vector3 moveDir = (targetPosition - transform.position).normalized;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position, targetPosition);

        if (distanceBefore < distanceAfter)
        {
            Instantiate(vfxHitGreen, targetPosition, Quaternion.identity);
            transform.Find("Trail").SetParent(null);
            Destroy(gameObject);
        }

    }
}
