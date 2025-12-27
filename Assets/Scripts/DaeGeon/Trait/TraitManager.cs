using System;
using UnityEngine;

public class TraitManager : MonoBehaviour
{
    public static TraitManager Instance;

    [SerializeField] private int totalTraitPoints = 10;
    private int usedPoints = 0;

    public int RemainingPoints => totalTraitPoints - usedPoints;
    public event Action OnPointsChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool TryUnlockTrait(TraitData data)
    {
        // 포인트 부족 시 실패
        if (RemainingPoints < data.unlockCost)
        {
            Debug.LogWarning("특성 포인트가 부족합니다!");
            return false;
        }

        // 1. 포인트 차감
        usedPoints += data.unlockCost;

        // 2. PlayerStatManager에 스탯 변화 적용
        if (PlayerStatManager.Instance != null)
        {
            // 고정 수치 반영
            if (data.addStrength != 0) PlayerStatManager.Instance.AddBaseStrength(data.addStrength);
            if (data.addAgility != 0) PlayerStatManager.Instance.AddBaseAgility(data.addAgility);
            if (data.addIntelligence != 0) PlayerStatManager.Instance.AddBaseIntelligence(data.addIntelligence);
            if (data.addMana != 0) PlayerStatManager.Instance.AddBaseMana(data.addMana);

            // 배수(Multiplier) 반영
            if (data.addStrMultiplier != 0) PlayerStatManager.Instance.AddStrMultiplier(data.addStrMultiplier);
            if (data.addAgiMultiplier != 0) PlayerStatManager.Instance.AddAgiMultiplier(data.addAgiMultiplier);
            if (data.addIntMultiplier != 0) PlayerStatManager.Instance.AddIntMultiplier(data.addIntMultiplier);
            if (data.addManaMultiplier != 0) PlayerStatManager.Instance.AddManaMultiplier(data.addManaMultiplier);
            
            Debug.Log($"{data.traitName} 해금 성공!");
        }

        // 3. UI 갱신용 이벤트 알림
        OnPointsChanged?.Invoke();
        return true;
    }
}