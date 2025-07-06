using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KidGame.Core;

public class TrapHudIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectedFrame;
    [SerializeField] private Text amountText;
    
    public void Setup(TrapSlotInfo info, bool isSelected)
    {
        iconImage.sprite = info.trapData.trapIcon;
        amountText.text = info.amount.ToString();
        selectedFrame.gameObject.SetActive(isSelected);
    }
    
    public void SetSelected(bool isSelected)
    {
        selectedFrame.gameObject.SetActive(isSelected);
    }
}