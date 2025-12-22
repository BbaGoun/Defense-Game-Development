using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    public Image icon;       
    public TMP_Text nameText; 
    public TMP_Text priceText;
    public Button buyButton;  

    private ItemData data;

    public void Setup(ItemData item)
    {
        data = item;

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => Buy());

        // 루트 버튼 클릭으로 미리보기 실행
        var rootBtn = GetComponent<Button>();
        if (rootBtn != null)
        {
            rootBtn.onClick.RemoveAllListeners();
            rootBtn.onClick.AddListener(() => { if (Player.Instance != null) Player.Instance.TogglePreview(data); });
        }
    }

    void Buy()
    {
        // 골드 체크
        if (!CurrencyManager.Instance.SpendGold(data.price))
        {
            Debug.Log("골드 부족");
            return;
        }
        // 인벤토리에 추가
        InventoryManager.Instance.AddItem(data);

        // 인벤토리 갱신
        var invUI = Object.FindAnyObjectByType<InventoryUI>();
        if (invUI != null) invUI.Refresh();

        // 구매로 인해 인벤토리로 이동했으므로 미리보기 초기화
        if (Player.Instance != null) Player.Instance.ClearPreview();

        Debug.Log($"{data.itemName} 구매 성공");
        buyButton.interactable = false;
        
    }
}
