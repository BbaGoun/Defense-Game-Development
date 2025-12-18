using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    public List<UnitData> allUnits;
    public Transform resultParent;
    public GameObject unitPrefab; // 여기에 Prefab 할당

    public void Draw(int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            UnitData randomUnit = GetRandomUnitByProbability();
            CreateResultUI(randomUnit);
        }
    }

    UnitData GetRandomUnitByProbability()
    {
        float roll = Random.value;
        List<UnitData> pool;

        if (roll < 0.6f)
            pool = allUnits.Where(u => u.grade == UnitGrade.NORMAL).ToList();
        else if (roll < 0.9f)
            pool = allUnits.Where(u => u.grade == UnitGrade.RARE).ToList();
        else
            pool = allUnits.Where(u => u.grade == UnitGrade.UNIQUE).ToList();

        int idx = Random.Range(0, pool.Count);
        return pool[idx];
    }

    void CreateResultUI(UnitData data)
    {
        GameObject go = Instantiate(unitPrefab, resultParent);
        UnitPrefab unitComp = go.GetComponent<UnitPrefab>();
        unitComp.AddShards(1); // 예시: 1조각 추가
    }
}
