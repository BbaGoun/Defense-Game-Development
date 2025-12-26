using UnityEngine;
using System;

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager Instance;

    [Header("초기 데이터 설정 (에셋 할당)")]
    [SerializeField] private PlayerBaseStatusSO initialStatusSO;

    [Header("Base Status (영구 스탯)")]
    [SerializeField] private PlayerStatus baseStatus;

    [Header("Bonus Status (장비, 버프 등)")]
    [SerializeField] private PlayerStatus bonusStatus;

    [Header("Multipliers (배수)")]
    [SerializeField] private float strMultiplier = 1.0f; // 기본 1배

    public event Action<PlayerStatus> OnStatChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeStats();
    }

    private void InitializeStats()
    {
        if (initialStatusSO != null && initialStatusSO.baseStatus != null)
        {
            baseStatus = new PlayerStatus
            {
                strength = initialStatusSO.baseStatus.strength,
                agility = initialStatusSO.baseStatus.agility,
                intelligence = initialStatusSO.baseStatus.intelligence,
                mana = initialStatusSO.baseStatus.mana
            };
        }
        else
        {
            baseStatus = new PlayerStatus();
        }
        bonusStatus = new PlayerStatus();
        NotifyChanged();
    }

    /// <summary>
    /// 곱연산이 적용된 최종 스탯을 반환합니다.
    /// </summary>
    public PlayerStatus TotalStatus 
    {
        get
        {
            // 1. 먼저 합연산을 합니다 (Base + Bonus)
            PlayerStatus total = baseStatus + bonusStatus;

            // 2. 합산된 결과에 배수를 곱합니다.
            // 정수형(int) 스탯이므로 계산 후 반올림(RoundToInt) 처리를 해줍니다.
            total.strength = Mathf.RoundToInt(total.strength * strMultiplier);
            
            // 다른 스탯도 배수가 필요하다면 여기에 추가하면 됩니다.
            // total.agility = Mathf.RoundToInt(total.agility * agiMultiplier);

            return total;
        }
    }

    /// <summary>
    /// 외부(특성 매니저 등)에서 배수를 변경할 때 호출합니다.
    /// </summary>
    public void UpdateStrMultiplier(float newMultiplier)
    {
        strMultiplier = newMultiplier;
        NotifyChanged(); // 배수가 바뀌었으니 최종 스탯이 변했다고 알림
    }

    #region 나머지 관리 함수들 (동일)
    public void AddBaseStrength(int value) { baseStatus.strength += value; NotifyChanged(); }
    public void AddBonus(PlayerStatus bonus) { bonusStatus.strength += bonus.strength; /*...*/ NotifyChanged(); }
    public void RemoveBonus(PlayerStatus bonus) { /*...*/ NotifyChanged(); }
    private void NotifyChanged() { OnStatChanged?.Invoke(TotalStatus); }
    #endregion
}