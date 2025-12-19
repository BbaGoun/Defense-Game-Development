using UnityEngine;
using UnityEngine.UI;

public class GachaButton : MonoBehaviour
{
    public int drawCount = 1;
    public int costPerDraw = 100;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickGacha);
    }

    private void OnClickGacha()
    {
        int totalCost = drawCount * costPerDraw;

        if (CurrencyManager.Instance.cash < totalCost)
        {
            Debug.Log("다이아 부족");
            return;
        }

        CurrencyManager.Instance.SpendCash(totalCost);
        GachaManager.Instance.Draw(drawCount);
    }
}
