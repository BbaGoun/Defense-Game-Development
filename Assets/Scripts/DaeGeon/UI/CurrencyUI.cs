using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public static CurrencyUI Instance;

    public TMP_Text goldText;
    public TMP_Text cashText;
    public TMP_Text staminaText;
    public TMP_Text staminaText2;

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
            // 기존 메인 UI 스태미나 갱신
            if (staminaText != null)
                staminaText.text = CurrencyManager.Instance.stamina.ToString();

            // 새로 추가된 스테이지 패널용 스태미나 갱신
            if (staminaText2 != null)
                staminaText2.text = CurrencyManager.Instance.stamina.ToString();
        }
}
