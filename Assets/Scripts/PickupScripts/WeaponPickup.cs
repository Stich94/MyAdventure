using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] RayCastWeapon weaponPrefab;




    private void OnTriggerEnter(Collider other)
    {
        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
        if (activeWeapon != null)
        {
            RayCastWeapon newWeapon = Instantiate(weaponPrefab);
            // newWeapon = newWeapon as RapidFireWeapon;
            newWeapon.GetComponent<RayCastWeapon>().enabled = true;
            activeWeapon.Equip(newWeapon);
        }
    }
}
