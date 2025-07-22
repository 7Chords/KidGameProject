using KidGame.Core;
using UnityEngine;
using UnityEngine.UI;
using KidGame;

public class TrapHudIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectedFrame;
    [SerializeField] private Text amountText;
    
    /// <summary>
    /// ������ʾ��Ϣ
    /// </summary>
    /// <param name="info"></param>
    /// <param name="isSelected"></param>
    public void Setup(ISlotInfo info, bool isSelected)
    {
        iconImage.color = Color.white;
        switch (info.ItemData.UseItemType)
        {
            case UseItemType.trap:
                iconImage.sprite = ResourceHelper.GetSpriteByPath((info.ItemData as TrapData).trapIconPath);
                break;
            case UseItemType.Material:
                iconImage.sprite = ResourceHelper.GetSpriteByPath((info.ItemData as MaterialData).materialIconPath);
                break;
            case UseItemType.weapon://todo:guihuala
                //iconImage.sprite = ResourceHelper.GetSpriteByPath((info.ItemData as WeaponData).weaponIconPath);
                break;
            case UseItemType.food:
                //iconImage.sprite = ResourceHelper.GetSpriteByPath((info.ItemData as FoodData).foodIconPath);
                break;
            default:
                Debug.LogError("û��ƥ�����ͣ��Ҳ���icon");
                break;

        }
        amountText.text = info.Amount.ToString();
        selectedFrame.gameObject.SetActive(isSelected);
    }

    /// <summary>
    /// �����ʾ��Ϣ
    /// </summary>
    public void SetEmpty()
    {
        iconImage.color = new Color(1, 1, 1, 0);
        amountText.text = "";
        selectedFrame.gameObject.SetActive(false);
    }
    
    public void SetSelected(bool isSelected)
    {
        selectedFrame.gameObject.SetActive(isSelected);
    }
}