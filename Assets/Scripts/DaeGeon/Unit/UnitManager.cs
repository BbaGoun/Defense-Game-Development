using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public List<UnitData> allUnits;

    [Header("Grade Sprites")]
    public Sprite normalSprite;
    public Sprite rareSprite;
    public Sprite uniqueSprite;

    private Dictionary<int, UnitState> states = new();

    private void Awake()
    {
        Instance = this;

        foreach (var data in allUnits)
        {
            states[data.unitId] = new UnitState();
        }
    }

    public UnitState GetState(int unitId)
    {
        return states[unitId];
    }

    public void AddShards(int unitId, int amount)
    {
        var state = states[unitId];

        if (!state.owned)
        {
            state.owned = true; // 처음 획득, owned 활성화만
            return;             
        }

        // 이미 소유 중이면 샤드 증가
        state.shards += amount;
    }


    public bool TryUpgrade(int unitId)
    {
        UnitState state = states[unitId];
        UnitData data = allUnits.Find(u => u.unitId == unitId);

        if (data == null) return false;
        if (!state.owned) return false;
        if (state.shards < data.shardsRequiredPerUpgrade) return false;

        state.shards -= data.shardsRequiredPerUpgrade;
        state.level++;

        return true;
    }

    
    public Sprite GetGradeSprite(UnitGrade grade)
    {
        return grade switch
        {
            UnitGrade.NORMAL => normalSprite,
            UnitGrade.RARE => rareSprite,
            UnitGrade.UNIQUE => uniqueSprite,
            _ => null
        };
    }
}
