using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // [SerializeField] WeapnScriptable currentWeapon;
    [SerializeField] TMP_Text currentAmmoText;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] static WeapnScriptable activeWeapon;
    [SerializeField] Slider slider;
    [SerializeField] playerStatsSO playerStats;

    float currentMagazineSize;
    float maxMagazineSize;

    float showCurrentHealth;

    private void Start()
    {
        // SetWeaponStats();
    }
    private void Update()
    {
        UpdateWeaponAmmoUI();
        UpdatePlayerHealthBar();
    }

    // void SetWeaponStats()
    // {
    //     currentMagazineSize = currentWeapon.currentMagazineSize;
    //     maxMagazineSize = currentWeapon.magazineSize;

    // }

    public void UpdateWeaponAmmoUI()
    {
        // SetWeaponStats();
        // getAmmoUpdateEvent.RaisEvent();
        if (activeWeapon?.currentMagazineSize <= 3)
        {
            currentAmmoText.color = Color.red;
        }
        else
        {
            currentAmmoText.color = Color.white;
        }
        // weird stuff is going on active Weapon is not empty but compiler is yelling - Null Reference
        currentAmmoText.text = activeWeapon.currentMagazineSize.ToString() + " / " + activeWeapon.magazineSize.ToString();
        // currentAmmoText.text = currentMagazineSize + " / " + maxMagazineSize;
    }

    public void SetActiveWeapon(WeapnScriptable _newWeapon)
    {
        if (_newWeapon != null)
        {
            activeWeapon = _newWeapon;
            weaponNameText.text = activeWeapon.weaponName;
            // SetWeaponStats();
        }

    }

    public void UpdatePlayerHealthBar()
    {
        showCurrentHealth = playerStats.CurrentHP;
        // if health is below max show Healthbar
        if (playerStats.CurrentHP <= playerStats.MaxHP)
        {
            slider.value = CalculateHealth();
        }
        if (playerStats.CurrentHP <= 0f)
        {
            Debug.Log("Player is dead");
        }
    }

    float CalculateHealth()
    {
        return playerStats.CurrentHP / playerStats.MaxHP;
    }



}




