using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int gold = 1000;
    public int cash = 50;
    public int stamina = 20;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 골드 소비
    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;

        gold -= amount;
        CurrencyUI.Instance.UpdateGold();
        return true;
    }

    // 골드 추가
    public void AddGold(int amount)
    {
        gold += amount;
        CurrencyUI.Instance.UpdateGold();
    }

    // 캐시 소비
    public bool SpendCash(int amount)
    {
        if (cash < amount) return false;

        cash -= amount;
        CurrencyUI.Instance.UpdateCash();
        return true;
    }

    // 캐시 추가
    public void AddCash(int amount)
    {
        cash += amount;
        CurrencyUI.Instance.UpdateCash();
    }

    // 스태미너 소비
    public bool SpendStamina(int amount)
    {
        if (stamina < amount) return false;

        stamina -= amount;
        CurrencyUI.Instance.UpdateStamina();
        return true;
    }

    // 스태미너 추가
    public void AddStamina(int amount)
    {
        stamina += amount;
        CurrencyUI.Instance.UpdateStamina();
    }
}
