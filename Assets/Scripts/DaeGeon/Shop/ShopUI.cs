using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public Transform content;                   // ScrollView → Content
    public GameObject itemButtonPrefab;         // ShopItemButton
    public List<ItemData> itemList;             // 판매할 아이템 목록
    // 미리보기 리셋 버튼
    public Button previewResetButton;

    private void Start()
    {
        foreach (var item in itemList)
            CreateShopItem(item);

        if (previewResetButton != null)
        {
            previewResetButton.onClick.RemoveAllListeners();
            previewResetButton.onClick.AddListener(() => { if (Player.Instance != null) Player.Instance.ClearPreview(); });
        }
    }

    void CreateShopItem(ItemData data)
    {
        GameObject obj = Instantiate(itemButtonPrefab, content);
        ShopItemButton btn = obj.GetComponent<ShopItemButton>();
        btn.Setup(data);
    }
}
