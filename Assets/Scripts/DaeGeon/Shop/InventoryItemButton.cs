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

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        attachText.text = GetAttachPointLabel(item.attachPoint);

        RefreshButton();

        // 루트 버튼 클릭으로 미리보기 실행 (프리팹의 부모 버튼에 붙여서 바로 사용 가능)
        var rootBtn = GetComponent<UnityEngine.UI.Button>();
        if (rootBtn != null)
        {
            rootBtn.onClick.RemoveAllListeners();
            rootBtn.onClick.AddListener(() => {
                if (Player.Instance != null) Player.Instance.TogglePreview(data);
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

        // 안전하게 Player 인스턴스 체크
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
        Player.Instance.Equip(data);

        // 같은 부위 버튼 전부 갱신
        InventoryUI.Instance.RefreshByAttachPoint(data.attachPoint);
    }

    private void OnUnequip()
    {
        Player.Instance.Unequip(data.attachPoint);

        // 같은 부위 버튼 전부 갱신
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
