using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // [SerializeField] RayCastWeapon activeWeapon;
    [SerializeField] TextMeshProUGUI currentAmmoText;
    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] WeapnScriptable activeWeapon;
    [SerializeField] Slider slider;
    [SerializeField] playerStatsSO playerStats;

    float showCurrentHealth;

    // public void UpdatePlayerAmmoUI(int _currentAmmo, int _maxMagazineSize)
    // {
    //     currentAmmoText.text = _currentAmmo + " / " + _maxMagazineSize;
    // }
    private void Start()
    {
        // showCurrentHealth = playerStats.CurrentHP;
    }

    private void Update()
    {
        UpdateWeaponAmmoUI();
        UpdatePlayerHealthBar();
    }

    public void UpdateWeaponAmmoUI()
    {
        // getAmmoUpdateEvent.RaisEvent();
        if (activeWeapon?.currentMagazineSize <= 3)
        {
            currentAmmoText.color = Color.red;
        }
        else
        {
            currentAmmoText.color = Color.white;
        }
        currentAmmoText.text = activeWeapon?.currentMagazineSize + " / " + activeWeapon?.magazineSize;
    }

    public void SetActiveWeapon(WeapnScriptable _newWeapon)
    {
        if (_newWeapon != null)
        {
            activeWeapon = _newWeapon;
            weaponNameText.text = activeWeapon.weaponName;
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




