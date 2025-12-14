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
        if (!items.Contains(data))
            items.Add(data);

        // UI 갱신 호출 가능
    }

    // 아이템 존재 여부 확인
    public bool HasItem(ItemData data)
    {
        return items.Contains(data);
    }
}
