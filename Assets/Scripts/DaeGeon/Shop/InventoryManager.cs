using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 아이템 추가
    public void AddItem(ItemData data)
    {
        if (items.Contains(data))
            return;

        // 가능한 경우 상점의 정렬 순서를 따름
        var shop = FindAnyObjectByType<ShopUI>();
        if (shop != null && shop.itemList != null && shop.itemList.Contains(data))
        {
            int shopIndex = shop.itemList.IndexOf(data);
            int insertPos = 0;

            // Count how many existing inventory items appear before this item in the shop list
            foreach (var existing in items)
            {
                if (existing == null) continue;
                int exIdx = shop.itemList.IndexOf(existing);
                if (exIdx >= 0 && exIdx < shopIndex)
                    insertPos++;
            }

            // Insert at the computed position
            items.Insert(insertPos, data);
        }
        else
        {
            items.Add(data);
        }

        // UI 갱신 호출 가능
        // 아이템이 인벤토리에 추가될 때(예: 상점에서 이동) 미리보기 초기화
        if (Player.Instance != null)
            Player.Instance.ClearPreview();
    }

        // 편의: 아이템 ID 목록으로 인벤토리 설정 (SaveManager에서 사용)
        public void SetItemsByIDs(List<string> ids)
        {
            items.Clear();
            if (ids == null || ids.Count == 0) return;
            if (ItemDatabase.Instance == null) return;

            foreach (var id in ids)
            {
                var it = ItemDatabase.Instance.GetByID(id);
                if (it != null && !items.Contains(it)) items.Add(it);
            }
        }

    // 아이템 존재 여부 확인
    public bool HasItem(ItemData data)
    {
        return items.Contains(data);
    }
}
