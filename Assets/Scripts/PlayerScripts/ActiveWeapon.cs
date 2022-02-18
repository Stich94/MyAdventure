using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// calling the Weapon Shoot Event in here, cause a weird lag, Player shoot must be handled in the Weapon itself
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] Transform crosshairTarget;
    [SerializeField] Transform bulletSpawnPoint; // only for debug
    [SerializeField] Transform weaponParent;

    [Header("Current Equipped Weapon")]
    [SerializeField] RayCastWeapon currentEquippedWeapon;

    [SerializeField] GameObject[] playerWeapons = new GameObject[3];

    ThirdPersonShooterController shootAimController;
    RayCastWeapon weapon;

    [SerializeField] UIManager playerHud;



    private void Start()
    {

        shootAimController = GetComponentInParent<ThirdPersonShooterController>();
        RayCastWeapon existingWeapon = GetComponentInChildren<RayCastWeapon>();
        if (existingWeapon) // if the player has a weapon - active it
        {
            Equip(existingWeapon);
        }
    }


    public void Equip(RayCastWeapon _newWeapon)
    {
        // if there is a weapon we need to destroy it
        if (weapon)
        {
            Destroy(weapon.gameObject);
        }
        Debug.Log("Weapon Equipped");
        weapon = _newWeapon;
        weapon.RayCastDestination = crosshairTarget;
        weapon.transform.parent = weaponParent;
        weapon.transform.localPosition = Vector3.zero;
        // weapon.transform.localRotation = Quaternion.identity;
        // weapon.transform.localRotation = _newWeapon.transform.localRotation;
        shootAimController.BulletSpawnOrigin = _newWeapon.GetBulletOriginPosition;
        bulletSpawnPoint = _newWeapon.GetBulletOriginPosition;
        currentEquippedWeapon = weapon;
        WeapnScriptable weaponData = weapon.GetWeaponData();
        playerHud?.SetActiveWeapon(weaponData);

    }

}
