using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentAmmoText;



    public void UpdatePlayerAmmoUI(int _currentAmmo, int _maxMagazineSize)
    {
        currentAmmoText.text = _currentAmmo + " / " + _maxMagazineSize;
    }
}




