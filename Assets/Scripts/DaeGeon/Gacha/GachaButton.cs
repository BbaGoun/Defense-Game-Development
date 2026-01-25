using UnityEngine;
using UnityEngine.UI;

namespace DaeGeon
{
    // 어떤 재화를 사용할지 선택하기 위한 열거형
    public enum CurrencyType { Cash, Gold }
    public enum GachaType { Unit, Equipment }

    public class GachaButton : MonoBehaviour
{
    [Header("가챠 종류 설정")]
    public GachaType gachaType;      // Unit 또는 Equipment (아까 만든 enum)
    
    [Header("재화 설정")]
    public CurrencyType currencyType; // Cash 또는 Gold 선택
    public int drawCount = 1;         // 몇 회 뽑기인지
    public int costPerDraw = 100;     // 1회당 비용

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickGacha);
    }

    private void OnClickGacha()
    {
        int totalCost = drawCount * costPerDraw;

        // 1. 선택한 재화가 충분한지 확인
        if (!HasEnoughCurrency(totalCost))
        {
            Debug.Log($"{currencyType} 부족!");
            return;
        }

        // 2. 재화 소모
        SpendCurrency(totalCost);

        // 3. 가챠 실행
        if (gachaType == GachaType.Unit)
        {
            GachaManager.Instance.Draw(drawCount);
        }
        else if (gachaType == GachaType.Equipment)
        {
            // 장비 가챠 전용 함수 호출
            GachaManager.Instance.DrawEquipment(drawCount);
        }
    }

    // 재화 확인 로직 분리
    private bool HasEnoughCurrency(int amount)
    {
        if (currencyType == CurrencyType.Cash)
            return CurrencyManager.Instance.cash >= amount;
        else
            return CurrencyManager.Instance.gold >= amount; // CurrencyManager에 gold 변수가 있다고 가정
    }

    // 재화 소모 로직 분리
    private void SpendCurrency(int amount)
    {
        if (currencyType == CurrencyType.Cash)
            CurrencyManager.Instance.SpendCash(amount);
        else
            CurrencyManager.Instance.SpendGold(amount); // CurrencyManager에 SpendGold 함수가 있다고 가정
    }
}
}