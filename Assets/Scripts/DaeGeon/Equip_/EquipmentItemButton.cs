using UnityEngine;
using UnityEngine.UI;
using TMPro; // 텍스트 출력을 위해 사용

public class EquipmentItemButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statText; // [추가] 공격력/방어력 표시용 텍스트
    public Image gradeFrame;         // [추가] 등급별 테두리 색상용 (선택)

    private ItemInstance currentInstance;

    public void Setup(ItemInstance instance)
    {
        currentInstance = instance;

        // 1. 고정 정보 (ItemData에서 가져옴)
        iconImage.sprite = instance.data.icon;
        nameText.text = instance.data.itemName;

        // 2. 가변 정보 (ItemInstance에서 가져옴)
        // 공격력이나 방어력이 있는 경우에만 표시
        if (statText != null)
        {
            if (instance.attack > 0) statText.text = $"ATK: {instance.attack}";
            else if (instance.defense > 0) statText.text = $"DEF: {instance.defense}";
            else statText.text = "";
        }

        // 3. 등급에 따른 시각적 처리 (선택 사항)
        if (gradeFrame != null)
        {
            gradeFrame.color = GetGradeColor(instance.data.grade);
        }
    }

    public void OnClickItem()
    {
        // 장착 버튼을 눌렀을 때 실행될 로직
        EquipmentManager.Instance.Equip(currentInstance);
    }

    private Color GetGradeColor(ItemGrade grade) => grade switch
    {
        ItemGrade.Common => Color.white,
        ItemGrade.Uncommon => Color.green,
        ItemGrade.Rare => Color.blue,
        ItemGrade.Epic => new Color(0.5f, 0, 0.5f), // 보라색
        ItemGrade.Legendary => Color.orange,
        _ => Color.white
    };
}