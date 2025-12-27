using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public Transform content;                   // ScrollView → Content
    public GameObject itemButtonPrefab;         // ShopItemButton 프리팹
    public List<ItemData> itemList;             // 판매할 아이템 목록
    
    [Header("UI References")]
    public Button previewResetButton;           // 미리보기 리셋 버튼

    private void Start()
    {
        // 상점 아이템 리스트 생성
        foreach (var item in itemList)
            CreateShopItem(item);

        // 리셋 버튼 설정
        if (previewResetButton != null)
        {
            previewResetButton.onClick.RemoveAllListeners();
            // Player.Instance를 직접 부르는 대신 방송국에 신호를 보냄
            previewResetButton.onClick.AddListener(() => 
            {
                PlayerEvents.OnClearPreviewRequest?.Invoke();
            });
        }
    }

    void CreateShopItem(ItemData data)
    {
        if (itemButtonPrefab == null || content == null) return;

        GameObject obj = Instantiate(itemButtonPrefab, content);
        ShopItemButton btn = obj.GetComponent<ShopItemButton>();
        if (btn != null)
        {
            btn.Setup(data);
        }
    }
}