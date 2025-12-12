using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int gold = 1000;
    public int cash = 50;
    public int stamina = 20;

    private void Awake()
    {
        Instance = this;
    }

    // 골드 소비
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            CurrencyUI.Instance.UpdateGold(gold);
            return true;
        }
        return false;
    }

    // 골드 추가
    public void AddGold(int amount)
    {
        gold += amount;
        CurrencyUI.Instance.UpdateGold(gold);
    }

    // 캐시 소비
    public bool SpendCash(int amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            CurrencyUI.Instance.UpdateCash(cash);
            return true;
        }
        return false;
    }

    // 캐시 추가
    public void AddCash(int amount)
    {
        cash += amount;
        CurrencyUI.Instance.UpdateCash(cash);
    }

    // 스태미너 소비
    public bool SpendStamina(int amount)
    {
        if (stamina >= amount)
        {
            stamina -= amount;
            CurrencyUI.Instance.UpdateStamina(stamina);
            return true;
        }
        return false;
    }

    // 스태미너 추가
    public void AddStamina(int amount)
    {
        stamina += amount;
        CurrencyUI.Instance.UpdateStamina(stamina);
    }
}
