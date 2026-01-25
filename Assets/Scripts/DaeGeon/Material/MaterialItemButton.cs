using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DaeGeon
{
    public class MaterialItemButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText; // 재료의 핵심: 현재 수량
    public Image gradeFrame;

    private ItemInstance currentInstance;

    public void Setup(ItemInstance instance)
    {
        currentInstance = instance;

        // 기본 정보
        iconImage.sprite = instance.data.icon;
        nameText.text = instance.data.itemName;

        // 수량 표시 (1개 이상일 때만 표시하거나 항상 표시)
        if (countText != null)
        {
            countText.text = instance.stackCount.ToString();
        }

        // 등급 색상 (재료도 등급이 있다면)
        if (gradeFrame != null)
        {
            // 이전에 만든 GetGradeColor 함수를 활용하세요
        }
    }

    // 클릭 시 상세 정보 팝업 열기
    public void OnClickItem()
    {
        if (currentInstance == null) return;
        if (ItemDetailUI.Instance != null)
        {
            ItemDetailUI.Instance.Open(currentInstance);
        }
    }
}
}