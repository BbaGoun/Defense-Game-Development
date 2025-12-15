using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<string> inventoryItemIDs = new List<string>();
    public string headSlotID;
    public string bodySlotID;
    public int gold;
    public int cash;
    public int stamina;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Save()
    {
        var data = new SaveData();

        // Inventory
        if (InventoryManager.Instance != null)
        {
            foreach (var it in InventoryManager.Instance.items)
            {
                if (it != null) data.inventoryItemIDs.Add(it.itemID);
            }
        }

        // Equipped
        if (Player.Instance != null)
        {
            data.headSlotID = Player.Instance.headSlot != null ? Player.Instance.headSlot.itemID : null;
            data.bodySlotID = Player.Instance.bodySlot != null ? Player.Instance.bodySlot.itemID : null;
        }

        // Currency
        if (CurrencyManager.Instance != null)
        {
            data.gold = CurrencyManager.Instance.gold;
            data.cash = CurrencyManager.Instance.cash;
            data.stamina = CurrencyManager.Instance.stamina;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Saved to: " + savePath);
    }

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found: " + savePath);
            return;
        }

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
        {
            Debug.LogError("Failed to parse save file.");
            return;
        }

        // ---------------- Inventory ----------------
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.items.Clear();

            foreach (var id in data.inventoryItemIDs)
            {
                ItemData it = null;

                // primary: ItemDatabase
                if (ItemDatabase.Instance != null)
                    it = ItemDatabase.Instance.GetByID(id);

                if (it != null)
                    InventoryManager.Instance.AddItem(it);
            }
        }

        // ---------------- Equipped ----------------
        if (Player.Instance != null)
        {
            ItemData head = null;
            ItemData body = null;

            // try database first
            if (ItemDatabase.Instance != null)
            {
                head = ItemDatabase.Instance.GetByID(data.headSlotID);
                body = ItemDatabase.Instance.GetByID(data.bodySlotID);
            }

            // fallback: search inventory items
            if (head == null && InventoryManager.Instance != null)
            {
                foreach (var it in InventoryManager.Instance.items)
                    if (it != null && it.itemID == data.headSlotID) { head = it; break; }
            }

            if (body == null && InventoryManager.Instance != null)
            {
                foreach (var it in InventoryManager.Instance.items)
                    if (it != null && it.itemID == data.bodySlotID) { body = it; break; }
            }

            // 장착/해제 적용
            if (string.IsNullOrEmpty(data.headSlotID))
                Player.Instance.Unequip(AttachPoint.Head);
            else if (head != null)
                Player.Instance.Equip(head);

            if (string.IsNullOrEmpty(data.bodySlotID))
                Player.Instance.Unequip(AttachPoint.Body);
            else if (body != null)
                Player.Instance.Equip(body);
        }

        // ---------------- Currency ----------------
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.gold = data.gold;
            CurrencyManager.Instance.cash = data.cash;
            CurrencyManager.Instance.stamina = data.stamina;

            if (CurrencyUI.Instance != null)
            {
                CurrencyUI.Instance.UpdateGold(data.gold);
                CurrencyUI.Instance.UpdateCash(data.cash);
                CurrencyUI.Instance.UpdateStamina(data.stamina);
            }
        }

        // ---------------- UI 갱신 ----------------
        var invUI = FindObjectOfType<InventoryUI>();
        if (invUI != null)
        {
            invUI.Refresh();

            // 각 버튼 상태 갱신
            foreach (var btn in invUI.GetComponentsInChildren<InventoryItemButton>())
                btn.RefreshButton();

            // 슬롯 이미지 갱신
            invUI.UpdateSlotIcon(AttachPoint.Head);
            invUI.UpdateSlotIcon(AttachPoint.Body);
        }

        Debug.Log("Loaded save: " + savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }
}
