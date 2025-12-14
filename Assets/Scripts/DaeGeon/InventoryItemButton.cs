using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemButton : MonoBehaviour
{
    public Image icon;           // 아이템 아이콘
    public TMP_Text nameText;    // 아이템 이름
    public TMP_Text attachText;  // 장착 상태 표시 (예: Head, Body)
    public Button equipButton;   // 장착 버튼

    private ItemData data;       // 버튼에 연결된 아이템 데이터

    // 버튼 UI 세팅
    public void Setup(ItemData item)
    {
        data = item;

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        attachText.text = GetAttachPointLabel(item.attachPoint);

        // 기존 이벤트 제거
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(() => OnEquip());
    }

    // 장착 버튼 클릭 시
    private void OnEquip()
    {
        // Player에 장착 (Player 스크립트 필요)
        Player.Instance.AttachItem(data.icon, data.attachPoint);

        Debug.Log($"{data.itemName} 장착됨!");
    }

    // AttachPoint를 한국어 레이블로 반환
    private string GetAttachPointLabel(AttachPoint ap)
    {
        switch (ap)
        {
            case AttachPoint.Head:
                return "머리";
            case AttachPoint.Body:
                return "몸통";
            default:
                return ap.ToString();
        }
    }
}