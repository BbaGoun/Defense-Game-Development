using UnityEngine;
using System.Collections.Generic;

public class UnitUI : MonoBehaviour
{
    [Header("ScrollView")]
    public Transform content;              // ScrollView → Content
    public GameObject unitPrefabUI;        // 위 UnitPrefabUI

    [Header("표시할 유닛 데이터")]
    public List<UnitData> unitList;

    private void Start()
    {
        RefreshUI();
    }

    // UI 새로고침
    public void RefreshUI()
    {
        // 기존 UI 제거
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        // 새로 생성
        foreach (var unitData in unitList)
            CreateUnitUI(unitData);
    }

    private void CreateUnitUI(UnitData data)
    {
        GameObject obj = Instantiate(unitPrefabUI, content);
        UnitPrefabUI ui = obj.GetComponent<UnitPrefabUI>();

        if (ui != null)
        {
            ui.Setup(data); // 초기 조각 0
        }
    }
}
