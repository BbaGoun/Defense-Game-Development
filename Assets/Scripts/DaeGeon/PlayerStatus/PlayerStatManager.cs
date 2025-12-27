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

    [Header("Multipliers (배수 설정)")]
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

    // 모든 연산이 끝난 최종 스탯 반환
    public PlayerStatus TotalStatus 
    {
        get
        {
            PlayerStatus total = baseStatus + bonusStatus;

            // 각각의 배수 적용 후 반올림
            total.strength = Mathf.RoundToInt(total.strength * strMultiplier);
            total.agility = Mathf.RoundToInt(total.agility * agiMultiplier);
            total.intelligence = Mathf.RoundToInt(total.intelligence * intMultiplier);
            total.mana = Mathf.RoundToInt(total.mana * manaMultiplier);

            return total;
        }
    }

    #region 배수 수정 함수
    public void AddStrMultiplier(float amount) { strMultiplier += amount; NotifyChanged(); }
    public void AddAgiMultiplier(float amount) { agiMultiplier += amount; NotifyChanged(); }
    public void AddIntMultiplier(float amount) { intMultiplier += amount; NotifyChanged(); }
    public void AddManaMultiplier(float amount) { manaMultiplier += amount; NotifyChanged(); }
    #endregion

    #region 기본 스탯 수정 함수
    public void AddBaseStrength(int value) { baseStatus.strength += value; NotifyChanged(); }
    public void AddBaseAgility(int value) { baseStatus.agility += value; NotifyChanged(); }
    public void AddBaseIntelligence(int value) { baseStatus.intelligence += value; NotifyChanged(); }
    public void AddBaseMana(int value) { baseStatus.mana += value; NotifyChanged(); }

    public void AddBonus(PlayerStatus bonus) 
    { 
        bonusStatus.strength += bonus.strength; 
        bonusStatus.agility += bonus.agility;
        bonusStatus.intelligence += bonus.intelligence;
        bonusStatus.mana += bonus.mana;
        NotifyChanged(); 
    }

    private void NotifyChanged()
    {
        OnStatChanged?.Invoke(TotalStatus);
    }
    #endregion
}