using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] RayCastWeapon weaponPrefab;

    [SerializeField] WeapnScriptable weaponScriptable;
    [SerializeField] string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
        if (activeWeapon != null && other.gameObject.CompareTag(targetTag))
        {

            // RayCastWeapon newWeapon = Instantiate(weaponPrefab);


            // // newWeapon.GetComponent<RayCastWeapon>().enabled = true;
            // activeWeapon.Equip(newWeapon);
        }
    }
}
