using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<string> items = new List<string>();
    private const string SAVE_KEY = "player_inventory";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Load();
    }

    public void AddItem(string itemID)
    {
        items.Add(itemID);
        Save();
        // UI 갱신 호출 (옵션)
    }

    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(new SaveWrapper { itemIDs = items.ToArray() });
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            var w = JsonUtility.FromJson<SaveWrapper>(json);
            items = new List<string>(w.itemIDs);
        }
    }

    [System.Serializable]
    private class SaveWrapper
    {
        public string[] itemIDs;
    }
}
