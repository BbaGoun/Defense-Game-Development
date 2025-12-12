using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public static CurrencyUI Instance;

    public TMP_Text goldText;
    public TMP_Text cashText;
    public TMP_Text staminaText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 시작할 때 UI 싹 업데이트
        UpdateGold(CurrencyManager.Instance.gold);
        UpdateCash(CurrencyManager.Instance.cash);
        UpdateStamina(CurrencyManager.Instance.stamina);
    }

    public void UpdateGold(int amount)
    {
        goldText.text = amount.ToString();
    }

    public void UpdateCash(int amount)
    {
        cashText.text = amount.ToString();
    }

    public void UpdateStamina(int amount)
    {
        staminaText.text = amount.ToString();
    }
}
