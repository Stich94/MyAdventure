using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Weapon", fileName = "Weapon")]
public class WeapnScriptable : ScriptableObject
{
    public string weaponName;
    public int weaponId;
    public bool isActive = false;

    public float damage;
    public int magazineSize = 20;
    public int currentMagazineSize = 20;
    public float fireRate;
    public float reloadTime;
    WaitForSeconds reloadWait;

    [SerializeField] VisualEffect fireEffect;
    [SerializeField] ParticleSystem muzzleFlash;

    // private void OnEnable()
    // {

    // }

    // public void StartFiring()
    // {

    //     FireAction.Invoke();
    // }

    // public void Register(Action _action)
    // {
    //     FireAction += _action;
    // }

    // public void UnRegister(Action _action)
    // {
    //     FireAction -= _action;
    // }

    public void SingleBurtMode()
    {
        muzzleFlash.Emit(1);
    }


}
