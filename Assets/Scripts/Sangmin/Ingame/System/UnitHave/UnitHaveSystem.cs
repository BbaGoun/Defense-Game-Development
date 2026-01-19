using Sangmin;
using UnityEngine;

public class UnitHaveSystem : MonoBehaviour
{
    private static UnitHaveSystem _instance;
    public static UnitHaveSystem Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void Init()
    {
        if (UnitManager.Instance == null)
            return;
        foreach (var unitData in UnitManager.Instance.allUnits)
        {
            if (!UnitManager.Instance.GetState(unitData.unitId).owned)
                continue;
            switch (unitData.grade)
            {
                case UnitGrade.NORMAL:
                case UnitGrade.RARE:
                case UnitGrade.UNIQUE:
                    RandomSummon.Instance.UnitList.Add(unitData.prefab);
                    ObjectPoolManager.Instance.AddObjectInfo(unitData.prefab, 3);
                    break;
                case UnitGrade.LEGEND:
                case UnitGrade.MYTHIC:
                    // 가능 조합식에 추가
                    ObjectPoolManager.Instance.AddObjectInfo(unitData.prefab, 1);
                    break;
            }
        }
    }


}
