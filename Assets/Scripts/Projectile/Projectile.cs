using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Projectile : MonoBehaviour
{
    #region 
    Rigidbody rb;
    [SerializeField] string targetTag = "Enemy";
    [SerializeField] LayerMask targetLayer;

    RagDoll ragdoll;

    CinemachineImpulseSource source;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        Init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            IDamageAble stat = other.gameObject.GetComponent<IDamageAble>();
            stat.TakeDamage(10f);
            Destroy(this.gameObject);

        }
        else
        {
            Destroy(this.gameObject);
        }


    }


    protected virtual void Init()
    {
        float speed = 100f;
        // rb.useGravity = false;
        rb.velocity = transform.forward * speed;

        // source.GenerateImpulse(Camera.main.transform.forward);
    }
    #endregion

    // [SerializeField] float time;
    // [SerializeField] Vector3 initialPosition;
    // [SerializeField] Vector3 initialVelocity;
    // [SerializeField] float bulletSpeed = 1000f;
    // [SerializeField] float bulletDrop = 0.0f;



    // protected Vector3 GetPosition(Projectile _projectile)
    // {
    //     // p + v*t + 0.5 * g * t * t
    //     Vector3 gravity = Vector3.down * bulletDrop;

    //     return (_projectile.initialPosition) + (_projectile.initialVelocity * _projectile.time) + (0.5f * gravity * _projectile.time * _projectile.time);
    // }

    // public Projectile Init(Vector3 _position, Vector3 _velocity)
    // {
    //     Projectile projectile = new Projectile();
    //     projectile.initialPosition = _position;
    //     projectile.initialVelocity = _velocity;
    //     projectile.time = 0.0f;
    //     return projectile;
    // }

    // protected virtual void FireBullet(Transform _destinationPos, Transform _origin)
    // {
    //     Vector3 velocity = (_destinationPos.position - _origin.position).normalized * bulletSpeed;
    //     Projectile projectile = Init(_origin.position, velocity);
    // }

    // private void OnTriggerEnter(Collider other)
    // {

    // }


}
