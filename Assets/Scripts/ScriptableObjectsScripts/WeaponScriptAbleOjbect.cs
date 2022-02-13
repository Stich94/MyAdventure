using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponScriptables/Weapon", fileName = "Weapon")]
public class WeaponScriptAbleOjbect : ScriptableObject
{
    public string WeaponName;
    public float Damage;
    public int MagazineSize;
    public int currentMagazineSize;
}
