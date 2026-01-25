using UnityEngine;
using System;

namespace DaeGeon
{
    public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager Instance;

    [Header("초기 데이터 설정")]
    [SerializeField] private PlayerBaseStatusSO initialStatusSO;
    
    [Header("Current Status")]
    [SerializeField] private PlayerStatus baseStatus;  // 특성 등 영구 스탯
    [SerializeField] private PlayerStatus bonusStatus; // 장비 등 유동 스탯

    [Header("Multipliers (배수)")]
    [SerializeField] private float strMultiplier = 1.0f;
    [SerializeField] private float agiMultiplier = 1.0f;
    [SerializeField] private float intMultiplier = 1.0f;
    [SerializeField] private float manaMultiplier = 1.0f;

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
            baseStatus = new PlayerStatus {
                strength = initialStatusSO.baseStatus.strength,
                agility = initialStatusSO.baseStatus.agility,
                intelligence = initialStatusSO.baseStatus.intelligence,
                mana = initialStatusSO.baseStatus.mana
            };
        }
        else baseStatus = new PlayerStatus();
        
        bonusStatus = new PlayerStatus();
        NotifyChanged();
    }

    // 모든 연산이 끝난 최종 스탯 반환 (중복 제거됨)
    public PlayerStatus TotalStatus 
    {
        get
        {
            // PlayerStatus 클래스에 + 연산자 오버로딩이 필요합니다.
            PlayerStatus total = baseStatus + bonusStatus;

            total.strength = Mathf.RoundToInt(total.strength * strMultiplier);
            total.agility = Mathf.RoundToInt(total.agility * agiMultiplier);
            total.intelligence = Mathf.RoundToInt(total.intelligence * intMultiplier);
            total.mana = Mathf.RoundToInt(total.mana * manaMultiplier);

            return total;
        }
    }

    // --- [장비용] 보너스 스탯 전체 갱신 ---
    // EquipmentManager에서 합산된 값을 한 번에 적용할 때 사용
    public void SetBonusStatus(PlayerStatus totalBonus) 
    { 
        bonusStatus = totalBonus; 
        NotifyChanged(); 
    }

    #region 배수 수정 함수 (특성/버프용)
    public void AddStrMultiplier(float amount) { strMultiplier += amount; NotifyChanged(); }
    public void AddAgiMultiplier(float amount) { agiMultiplier += amount; NotifyChanged(); }
    public void AddIntMultiplier(float amount) { intMultiplier += amount; NotifyChanged(); }
    public void AddManaMultiplier(float amount) { manaMultiplier += amount; NotifyChanged(); }
    #endregion

    #region 기본 스탯 수정 함수 (특성 영구 증가용)
    public void AddBaseStrength(int value) { baseStatus.strength += value; NotifyChanged(); }
    public void AddBaseAgility(int value) { baseStatus.agility += value; NotifyChanged(); }
    public void AddBaseIntelligence(int value) { baseStatus.intelligence += value; NotifyChanged(); }
    public void AddBaseMana(int value) { baseStatus.mana += value; NotifyChanged(); }
    #endregion

    private void NotifyChanged()
    {
        OnStatChanged?.Invoke(TotalStatus);
    }
}
}