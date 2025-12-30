using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // [중요] 프리팹의 "장착/해제" 버튼을 눌렀을 때 실행 (인벤토리 리스트에서 즉시 처리)
    public void OnEquipClick() 
    {
        if (currentInstance == null) return;

        // 실제 장착/해제 처리 (에러 났던 selectedItem 대신 currentInstance 사용)
        if (currentInstance.isEquipped)
            EquipmentManager.Instance.Unequip(currentInstance.data.equipmentType);
        else
            EquipmentManager.Instance.Equip(currentInstance);

        // UI들 새로고침
        UpdateBtnText(); 
        EquipmentUI.Instance.RefreshList();

        // 만약 정보창이 열려있다면 정보창도 같이 갱신해줌
        if (ItemDetailUI.Instance != null && ItemDetailUI.Instance.gameObject.activeSelf)
        {
            ItemDetailUI.Instance.Open(currentInstance);
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