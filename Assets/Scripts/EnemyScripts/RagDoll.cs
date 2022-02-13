using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDoll : MonoBehaviour
{
    [SerializeField] Rigidbody[] rigidodies;
    Animator animator;

    void Start()
    {
        rigidodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody r in rigidodies)
        {
            r.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        EnableRigidbody();
        Debug.Log("Ragdoll triggered");
        foreach (Rigidbody r in rigidodies)
        {
            r.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void DisableRigidbody()
    {
        foreach (Rigidbody r in rigidodies)
        {
            r.detectCollisions = false;
        }
    }

    public void EnableRigidbody()
    {
        foreach (Rigidbody r in rigidodies)
        {
            r.detectCollisions = true;
        }
    }
}
