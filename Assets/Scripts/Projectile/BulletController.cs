using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IBulletController
{
    [SerializeField] GameObject bulletDecal;

    float speed = 50f;
    float timeToDestroy = 3f;

    public Vector3 target { get; set; }
    public bool hit { get; set; }

    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
    }


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (!hit && Vector3.Distance(transform.position, target) < .01f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contact = other.GetContact(0);
        Instantiate(bulletDecal, contact.point + contact.normal * .0001f, Quaternion.LookRotation(contact.normal)); // we need to offset the decal by a minimum amount to get it rendered
        Destroy(gameObject);
    }

    public void GetBulletComponent()
    {
        throw new System.NotImplementedException();
    }
}
