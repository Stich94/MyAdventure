using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] float healAmount = 15f;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<IOnItemPickup>().OnItemEnter(healAmount);
        Destroy(this.gameObject);
    }
}
