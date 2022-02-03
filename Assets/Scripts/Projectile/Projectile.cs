using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] string targetTag = "Enemy";
    [SerializeField] LayerMask targetLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            Destroy(this.gameObject);

        }
    }

    protected virtual void Init()
    {
        float speed = 50f;
        rb.velocity = transform.forward * speed;
    }
}
