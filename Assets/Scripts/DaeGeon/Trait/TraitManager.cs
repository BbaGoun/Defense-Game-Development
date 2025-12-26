using System;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    public static TraitManager Instance;

    [Header("Points")]
    [SerializeField] private int totalTraitPoints = 10; // 보유한 총 포인트
    private int usedPoints = 0;

    // 현재 모든 특성으로 인해 추가된 스탯 보너스 합계
    // 예: Attack -> 15.5, Health -> 100
    private Dictionary<StatType, float> statBonuses = new Dictionary<StatType, float>();

    // 실제 데이터 변경을 알리는 이벤트 (캐릭터 스탯창 갱신용)
    public event Action OnTraitChanged;

    public int RemainingPoints => totalTraitPoints - usedPoints;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        
        // 모든 StatType에 대해 Dictionary 초기화
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            statBonuses[type] = 0;
        }
    }

    /// <summary>
    /// 특정 특성을 레벨업 시도함
    /// </summary>
    public bool TryUpgradeTrait(TraitData data)
    {
        // 1. 포인트 체크
        if (RemainingPoints <= 0)
        {
            Debug.LogWarning("특성 포인트가 부족합니다.");
            return false;
        }

        // 2. 선행 조건 체크 (필요한 경우)
        // if (!IsRequirementMet(data)) return false;

        // 3. 포인트 소모 및 스탯 반영
        usedPoints++;
        ApplyStatEffect(data.targetStat, data.value);

        Debug.Log($"{data.traitName} 업그레이드 성공! 남은 포인트: {RemainingPoints}");
        
        // UI나 캐릭터 정보 갱신을 위해 이벤트 호출
        OnTraitChanged?.Invoke();
        return true;
    }

    private void ApplyStatEffect(StatType type, float value)
    {
        if (statBonuses.ContainsKey(type))
        {
            statBonuses[type] += value;
        }
    }

    /// <summary>
    /// 특정 스탯의 현재 총 보너스 값을 반환 (캐릭터 스탯 계산 시 참조)
    /// </summary>
    public float GetTotalBonus(StatType type)
    {
        return statBonuses.ContainsKey(type) ? statBonuses[type] : 0;
    }

    // [추후 구현] 세이브/로드 기능
    // public void SaveTraits() { ... }
}