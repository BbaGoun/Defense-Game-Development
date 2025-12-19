using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance;

    [Header("Result UI")]
    public GameObject resultPanel;
    public Transform resultRoot;
    public GameObject resultUnitPrefab;

    private void Awake()
    {
        Instance = this;
        resultPanel.SetActive(false);
    }

    public void Draw(int drawCount)
    {
        ClearResultUI();
        resultPanel.SetActive(true);

        for (int i = 0; i < drawCount; i++)
        {
            UnitData unit = GetRandomUnitByProbability();

            // 🔑 상태 변경은 UnitManager만
            UnitManager.Instance.AddShards(unit.unitId, 1);

            CreateResultUnitUI(unit);

            Debug.Log($"{unit.unitName} 조각 획득");
        }

        // 가챠 끝 → 유닛 UI 갱신
        UnitUI.Instance.RefreshUI();
    }

    void CreateResultUnitUI(UnitData data)
    {
        GameObject go = Instantiate(resultUnitPrefab, resultRoot);
        ResultUnitPrefab ui = go.GetComponent<ResultUnitPrefab>();

        UnitState state = UnitManager.Instance.GetState(data.unitId);
        ui.Setup(data, state);
    }

    void ClearResultUI()
    {
        for (int i = resultRoot.childCount - 1; i >= 0; i--)
            Destroy(resultRoot.GetChild(i).gameObject);
    }

    UnitData GetRandomUnitByProbability()
    {
        // 🔑 UnitManager의 allUnits 참조
        var allUnits = UnitManager.Instance.allUnits;

        float roll = Random.value;

        var normal = allUnits.Where(u => u.grade == UnitGrade.NORMAL).ToList();
        var rare   = allUnits.Where(u => u.grade == UnitGrade.RARE).ToList();
        var unique = allUnits.Where(u => u.grade == UnitGrade.UNIQUE).ToList();

        List<UnitData> pool = null;

        if (roll < 0.6f && normal.Count > 0)
            pool = normal;
        else if (roll < 0.9f && rare.Count > 0)
            pool = rare;
        else if (unique.Count > 0)
            pool = unique;
        else
            pool = normal; // 안전망

        return pool[Random.Range(0, pool.Count)];
    }

    public void CloseResultPanel()
    {
        resultPanel.SetActive(false);
    }
}
