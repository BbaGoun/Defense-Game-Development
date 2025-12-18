using UnityEngine;
using UnityEngine.UI;

public class GachaButton : MonoBehaviour
{
    [Header("Draw Setting")]
    public int drawCount = 1;          // 1 or 10
    public int costPerDraw = 100;      // 1회당 다이아 비용

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickGacha);
    }

    void OnClickGacha()
    {
        button.interactable = false;

        int totalCost = drawCount * costPerDraw;

        if (CurrencyManager.Instance.cash < totalCost)
        {
            Debug.Log("다이아 부족");
            button.interactable = true;
            return;
        }

        CurrencyManager.Instance.SpendCash(totalCost);

        Debug.Log($"{drawCount}회 뽑기 성공 (비용: {totalCost})");

        // GachaManager.Instance.Draw(drawCount);

        button.interactable = true; // 연출 끝날 때 다시 켜는 걸로 변경 예정
    }
}
