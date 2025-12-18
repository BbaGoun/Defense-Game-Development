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
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        RefreshAll();
    }

    // ================= 전체 갱신 =================
    public void RefreshAll()
    {
        if (CurrencyManager.Instance == null) return;

        UpdateGold();
        UpdateCash();
        UpdateStamina();
    }

    public void UpdateGold()
    {
        goldText.text = CurrencyManager.Instance.gold.ToString();
    }

    public void UpdateCash()
    {
        cashText.text = CurrencyManager.Instance.cash.ToString();
    }

    public void UpdateStamina()
    {
        staminaText.text = CurrencyManager.Instance.stamina.ToString();
    }
}
