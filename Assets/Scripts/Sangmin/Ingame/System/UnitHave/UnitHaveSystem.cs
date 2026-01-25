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
        if (DaeGeon.UnitManager.Instance == null)
            return;
        foreach (var unitData in DaeGeon.UnitManager.Instance.allUnits)
        {
            if (!DaeGeon.UnitManager.Instance.GetState(unitData.unitId).owned)
                continue;
            switch (unitData.grade)
            {
                case DaeGeon.UnitGrade.NORMAL:
                case DaeGeon.UnitGrade.RARE:
                case DaeGeon.UnitGrade.UNIQUE:
                    RandomSummon.Instance.UnitList.Add(unitData.prefab);
                    ObjectPoolManager.Instance.AddObjectInfo(unitData.prefab, 3);
                    break;
                case DaeGeon.UnitGrade.LEGEND:
                case DaeGeon.UnitGrade.MYTHIC:
                    // 가능 조합식에 추가
                    ObjectPoolManager.Instance.AddObjectInfo(unitData.prefab, 1);
                    break;
            }
        }
    }


}
