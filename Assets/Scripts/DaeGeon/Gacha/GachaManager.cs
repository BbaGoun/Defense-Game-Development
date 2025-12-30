using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance;

    [Header("Result UI")]
    public GameObject resultPanel;
    public Transform resultRoot;
    
    [Header("Prefabs")]
    public GameObject resultUnitPrefab;   // 유닛 결과용 프리팹
    public GameObject resultItemPrefab;   // 장비 결과용 프리팹

    private void Awake()
    {
        Instance = this;
        resultPanel.SetActive(false);
    }

    // --- 1. 유닛 가챠 (GachaButton에서 호출함) ---
    public void Draw(int drawCount)
    {
        ClearResultUI();
        resultPanel.SetActive(true);

        for (int i = 0; i < drawCount; i++)
        {
            UnitData unit = GetRandomUnitByProbability();
            // 유닛 조각 추가 로직 (기존 유지)
            if (UnitManager.Instance != null) UnitManager.Instance.AddShards(unit.unitId, 1);
            CreateResultUnitUI(unit);
        }

        if (UnitUI.Instance != null) UnitUI.Instance.RefreshUI();
    }

    // --- 2. 장비 가챠 ---
    public void DrawEquipment(int drawCount)
    {
        ClearResultUI();
        resultPanel.SetActive(true);

        for (int i = 0; i < drawCount; i++)
        {
            ItemData itemData = GetRandomEquipmentByProbability();

            if (itemData != null)
            {
                ItemInstance newInstance = InventoryManager.Instance.AddItem(itemData);
                if (newInstance != null)
                {
                    CreateResultItemUI(newInstance);
                }
            }
        }

        if (EquipmentUI.Instance != null) EquipmentUI.Instance.RefreshList();
    }

    // --- 유닛 결과 UI 생성 ---
    void CreateResultUnitUI(UnitData data)
    {
        GameObject go = Instantiate(resultUnitPrefab, resultRoot);
        var ui = go.GetComponent<ResultUnitPrefab>();
        if (ui != null && UnitManager.Instance != null)
        {
            UnitState state = UnitManager.Instance.GetState(data.unitId);
            ui.Setup(data, state);
        }
    }

    // --- 장비 결과 UI 생성 ---
    void CreateResultItemUI(ItemInstance instance)
    {
        GameObject go = Instantiate(resultItemPrefab, resultRoot);
        var ui = go.GetComponent<EquipmentItemButton>();
        if (ui != null) ui.Setup(instance);
    }

    // --- 유닛 확률 로직 (기존 유지) ---
    UnitData GetRandomUnitByProbability()
    {
        var allUnits = UnitManager.Instance.allUnits;
        float roll = Random.value;

        var normal = allUnits.Where(u => u.grade == UnitGrade.NORMAL).ToList();
        var rare   = allUnits.Where(u => u.grade == UnitGrade.RARE).ToList();
        var unique = allUnits.Where(u => u.grade == UnitGrade.UNIQUE).ToList();

        if (roll < 0.6f && normal.Count > 0) return normal[Random.Range(0, normal.Count)];
        if (roll < 0.9f && rare.Count > 0) return rare[Random.Range(0, rare.Count)];
        if (unique.Count > 0) return unique[Random.Range(0, unique.Count)];
        
        return allUnits[Random.Range(0, allUnits.Count)];
    }

    // --- 장비 확률 로직 (안전장치 포함) ---
    ItemData GetRandomEquipmentByProbability()
    {
        var pool = ItemDatabase.Instance.equipmentItems;

        var normal = pool.Where(it => it != null && it.grade == ItemGrade.NORMAL).ToList();
        var rare = pool.Where(it => it != null && it.grade == ItemGrade.RARE).ToList();
        var legendary = pool.Where(it => it != null && it.grade == ItemGrade.LEGENDARY).ToList();

        float roll = Random.value;
        ItemData selected = null;

        if (roll >= 0.95f) // Legendary
        {
            if (legendary.Count > 0) selected = legendary[Random.Range(0, legendary.Count)];
            else if (rare.Count > 0) selected = rare[Random.Range(0, rare.Count)];
            else if (normal.Count > 0) selected = normal[Random.Range(0, normal.Count)];
        }
        else if (roll >= 0.70f) // Rare
        {
            if (rare.Count > 0) selected = rare[Random.Range(0, rare.Count)];
            else if (normal.Count > 0) selected = normal[Random.Range(0, normal.Count)];
        }
        else // Normal
        {
            if (normal.Count > 0) selected = normal[Random.Range(0, normal.Count)];
        }

        if (selected == null)
        {
            var validItems = pool.Where(it => it != null).ToList();
            if (validItems.Count > 0) selected = validItems[Random.Range(0, validItems.Count)];
        }

        return selected;
    }

    void ClearResultUI()
    {
        foreach (Transform child in resultRoot)
        {
            Destroy(child.gameObject);
        }
    }

    public void CloseResultPanel() => resultPanel.SetActive(false);
}