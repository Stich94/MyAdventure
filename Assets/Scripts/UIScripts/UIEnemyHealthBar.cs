using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] Slider slider;
    [SerializeField] GameObject healthBarUI;
    [SerializeField] float showCurrentHealth;

    BaseStats baseStats;


    void Start()
    {
        baseStats = GetComponent<BaseStats>();
        slider.value = CalculateHealth();
        mainCam = Camera.main;
        healthBarUI.SetActive(false);

    }

    void Update()
    {
        UpdateEnemyHealthBar();
    }

    void LateUpdate()
    {
        // transform.position = mainCam.WorldToScreenPoint(targetHead.position);
        FaceCamera();
    }

    public void UpdateEnemyHealthBar()
    {
        showCurrentHealth = baseStats.CurrentHealth;
        // if health is below max show Healthbar
        if (baseStats.CurrentHealth <= baseStats.MaxHealth)
        {
            healthBarUI.SetActive(true);
            slider.value = CalculateHealth();
        }
        if (baseStats.CurrentHealth <= 0f)
        {
            healthBarUI.SetActive(false);
        }
    }

    float CalculateHealth()
    {
        return baseStats.CurrentHealth / baseStats.MaxHealth;
    }

    void FaceCamera()
    {
        healthBarUI.transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
    }
}
