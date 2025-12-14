using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    public Image icon;        // Icon
    public TMP_Text nameText; // NameText
    public TMP_Text priceText;// PriceText
    public Button buyButton;  // BuyButton

    private ItemData data;

    public void Setup(ItemData item)
    {
        data = item;

        // UI 세팅
        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => Buy());
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

        // 인벤토리 UI가 있으면 갱신
        var invUI = Object.FindObjectOfType<InventoryUI>();
        if (invUI != null) invUI.Refresh();

        Debug.Log($"{data.itemName} 구매 성공!");
        buyButton.interactable = false;
        
    }
}
