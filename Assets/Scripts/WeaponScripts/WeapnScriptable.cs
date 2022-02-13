using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Weapon", fileName = "Weapon")]
public class WeapnScriptable : ScriptableObject
{
    public event Action FireAction;
    public bool isActive = false;

    public float damage;
    public int magazineSize;
    public int currentMagazineSize;
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
