using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemButton : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text attachText;
    public Button equipButton;
    public TMP_Text equipButtonText;

    private ItemData data;

    // InventoryUI에서 접근용
    public AttachPoint AttachPoint => data.attachPoint;

    public void Setup(ItemData item)
    {
        data = item;

        if (icon != null) icon.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;
        if (attachText != null) attachText.text = GetAttachPointLabel(item.attachPoint);

        RefreshButton();

        // 루트 버튼 클릭으로 미리보기 실행
        var rootBtn = GetComponent<UnityEngine.UI.Button>();
        if (rootBtn != null)
        {
            rootBtn.onClick.RemoveAllListeners();
            rootBtn.onClick.AddListener(() => 
            {
                // 직접 호출 대신 방송 송출
                PlayerEvents.OnTogglePreviewRequest?.Invoke(data);
            });
        }
    }

    // 장착 버튼 새로고침
    public void RefreshButton()
    {
        if (equipButton == null || equipButtonText == null)
        {
            Debug.LogWarning($"InventoryItemButton missing UI refs on '{gameObject.name}'");
            return;
        }

        equipButton.onClick.RemoveAllListeners();

        // 데이터 체크를 위해 Player.Instance(기준점)는 참조하되, 명령은 방송으로 보냅니다.
        if (Player.Instance == null)
        {
            equipButtonText.text = "장착";
            equipButton.interactable = false;
            return;
        }

        bool isEquipped = Player.Instance.IsEquipped(data);

        if (isEquipped)
        {
            equipButtonText.text = "해제";
            equipButton.onClick.AddListener(OnUnequip);
            equipButton.interactable = true;
        }
        else
        {
            equipButtonText.text = "장착";
            equipButton.onClick.AddListener(OnEquip);
            equipButton.interactable = true;
        }
    }

    private void OnEquip()
    {
        // Player.Instance.Equip(data); 대신 방송 송출
        PlayerEvents.OnEquipRequest?.Invoke(data);

        // UI 갱신 (기존 방식 유지)
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.RefreshByAttachPoint(data.attachPoint);
    }

    private void OnUnequip()
    {
        // Player.Instance.Unequip(data.attachPoint); 대신 방송 송출
        PlayerEvents.OnUnequipRequest?.Invoke(data.attachPoint);

        // UI 갱신 (기존 방식 유지)
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.RefreshByAttachPoint(data.attachPoint);
    }

    private string GetAttachPointLabel(AttachPoint ap)
    {
        switch (ap)
        {
            case AttachPoint.Head: return "머리";
            case AttachPoint.Body: return "몸통";
            default: return ap.ToString();
        }
    }
}