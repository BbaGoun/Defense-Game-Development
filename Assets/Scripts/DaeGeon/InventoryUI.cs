using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform content;                  
    public GameObject itemButtonPrefab;        
    
    private void Start()
    {
        Refresh();
    }

    // Refresh inventory UI (clear and repopulate)
    public void Refresh()
    {
        if (content == null || itemButtonPrefab == null) return;

        // Clear existing
        for (int i = content.childCount - 1; i >= 0; --i)
            Destroy(content.GetChild(i).gameObject);

        if (InventoryManager.Instance == null) return;

        foreach (var item in InventoryManager.Instance.items)
            CreateInventoryItem(item);
    }

    void CreateInventoryItem(ItemData data)
    {
        GameObject obj = Instantiate(itemButtonPrefab, content);
        InventoryItemButton btn = obj.GetComponent<InventoryItemButton>();
        if (btn != null) btn.Setup(data);
    }
}
