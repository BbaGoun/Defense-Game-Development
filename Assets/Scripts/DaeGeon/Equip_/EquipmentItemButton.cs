using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DaeGeon
{
    public class EquipmentItemButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statText; 
    public Image gradeFrame;         
    
    [Header("추가 UI 요소")]
    public TextMeshProUGUI buttonText; // 프리팹 내 "장착/해제" 버튼의 텍스트

    private ItemInstance currentInstance;

    public void Setup(ItemInstance instance)
    {
        currentInstance = instance;

        // 1. 기본 정보 세팅
        iconImage.sprite = instance.data.icon;
        nameText.text = instance.data.itemName;

        // 2. 스탯 표시
        if (statText != null)
        {
            if (instance.attack > 0) statText.text = $"ATK: {instance.attack}";
            else if (instance.defense > 0) statText.text = $"DEF: {instance.defense}";
            else statText.text = "";
        }

        // 3. 등급 색상
        if (gradeFrame != null)
            gradeFrame.color = GetGradeColor(instance.data.grade);

        // 4. 장착 여부에 따른 버튼 텍스트 업데이트
        UpdateBtnText();
    }

    public void OnEquipClick() 
    {
        if (currentInstance == null) return;

        if (currentInstance.isEquipped)
            EquipmentManager.Instance.Unequip(currentInstance.data.equipmentType);
        else
            EquipmentManager.Instance.Equip(currentInstance);

        UpdateBtnText(); 
        EquipmentUI.Instance.RefreshList();

        if (ItemDetailUI.Instance != null && ItemDetailUI.Instance.panelContent.activeSelf)
        {
            ItemDetailUI.Instance.Close(); 
        }
    }

    // [중요] 아이템 슬롯(이미지 등)을 클릭했을 때 정보창 열기
    public void OnClickItem()
    {
        if (currentInstance == null) return;

        if (ItemDetailUI.Instance != null)
        {
            ItemDetailUI.Instance.Open(currentInstance);
        }
    }

    private void UpdateBtnText()
    {
        if (buttonText != null)
            buttonText.text = currentInstance.isEquipped ? "해제" : "장착";
    }

    private Color GetGradeColor(ItemGrade grade) => grade switch
    {
        ItemGrade.NORMAL => Color.white,
        ItemGrade.UNIQUE => Color.green,
        ItemGrade.RARE => Color.blue,
        ItemGrade.LEGENDARY => new Color(0.5f, 0, 0.5f), 
        ItemGrade.MYTHIC => Color.orange,
        _ => Color.white
    };
}
}