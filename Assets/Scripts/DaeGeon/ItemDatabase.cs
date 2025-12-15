using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Tooltip("모든 ItemData를 에디터에서 할당하세요. Save/Load 시 itemID로 검색합니다.")]
    public List<ItemData> allItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public ItemData GetByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var it in allItems)
        {
            if (it != null && it.itemID == id) return it;
        }
        return null;
    }
}
