using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : MonoBehaviour
{
    [SerializeField] Transform targetHead;
    [SerializeField] Camera mainCam;
    [SerializeField] Vector3 offset;
    [SerializeField] StatsScrptableObject stats;
    [SerializeField] Image foregroundImage;
    [SerializeField] Image backgroundImage;




    void LateUpdate()
    {
        transform.position = mainCam.WorldToScreenPoint(targetHead.position + offset);
    }

    void UpdateEnemyHealthBar()
    {
        // if health is below max show Healthbar
        if (stats.currentHp <= stats.maxHp)
        {
            this.gameObject.SetActive(true);
        }
    }

    void SetHealthPercentage(float _percentage)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * _percentage;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
