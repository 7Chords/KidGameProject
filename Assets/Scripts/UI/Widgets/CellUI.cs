using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI count;
    private void Awake()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        count = transform.Find("Count").GetComponent<TextMeshProUGUI>();
    }

    public void SetUIWithMaterial(MaterialSlotInfo material)
    {
        icon.sprite = material.materialData.materialIcon;
        count.text = material.amount.ToString();
    }

    public void SetUIWithTrap(TrapSlotInfo trap)
    {
        icon.sprite = trap.trapData.trapIcon;
        count.text = trap.amount.ToString();
    }
}
