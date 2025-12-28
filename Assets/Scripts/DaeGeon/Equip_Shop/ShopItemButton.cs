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

        if (icon != null) icon.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;
        if (priceText != null) priceText.text = item.price.ToString();

        // 구매 버튼 설정
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => Buy());

        // 루트 버튼(자기 자신) 클릭 시 미리보기 실행
        var rootBtn = GetComponent<Button>();
        if (rootBtn != null)
        {
            rootBtn.onClick.RemoveAllListeners();
            // Player.Instance 대신 방송국에 토글 신호를 보냄
            rootBtn.onClick.AddListener(() => 
            {
                PlayerEvents.OnTogglePreviewRequest?.Invoke(data);
            });
        }
    }

    void Buy()
    {
        // 골드 체크 (CurrencyManager 필요)
        if (!CurrencyManager.Instance.SpendGold(data.price))
        {
            Debug.Log("골드 부족");
            return;
        }

        // 인벤토리에 추가
        InventoryManager.Instance.AddItem(data);

        // 인벤토리 UI 갱신
        var invUI = Object.FindAnyObjectByType<InventoryUI>();
        if (invUI != null) invUI.Refresh();

        // 구매 성공 시 모든 플레이어 유닛의 미리보기 초기화 방송
        PlayerEvents.OnClearPreviewRequest?.Invoke();

        Debug.Log($"{data.itemName} 구매 성공");
        
        // 품절 처리 (재구매 불가 시)
        buyButton.interactable = false;
    }
}