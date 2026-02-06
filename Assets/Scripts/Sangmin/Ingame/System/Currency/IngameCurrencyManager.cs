using UnityEngine;
using System;

namespace Sangmin
{
    /// <summary>
    /// 디펜스 게임 내부에서 사용하는 재화(골드, 쥬얼)를 관리하는 매니저입니다.
    /// 골드는 일반 뽑기에, 쥬얼은 특정 등급 뽑기에 사용됩니다.
    /// </summary>
    public class IngameCurrencyManager : MonoBehaviour
    {
        private static IngameCurrencyManager _instance;
        public static IngameCurrencyManager Instance
        {
            get { return _instance; }
        }

        [Header("초기 재화")]
        [SerializeField] private int initialGold = 63;
        [SerializeField] private int initialJewel = 0;

        [Header("일반 뽑기 비용 설정")]
        [SerializeField] private int initialSummonCost = 20; // 초기 뽑기 비용
        [SerializeField] private int costIncreasePerSummon = 1; // 뽑을 때마다 증가하는 비용

        [Header("특정 등급 뽑기 비용 설정")]
        [SerializeField] private int rareSummonCost = 50; // 희귀 등급 뽑기 비용
        [SerializeField] private int UniqueSummonCost = 100; // 영웅 등급 뽑기 비용
        [SerializeField] private int legendSummonCost = 200; // 전설 등급 뽑기 비용

        [Header("유닛 판매 설정")]
        // 판매 가격 = 소환 비용 / 10 * multiplier
        [SerializeField] private int normalUnitSellPriceMultiplier = 1; // 일반 유닛 
        [SerializeField] private int rareUnitSellPriceMultiplier = 3; // 희귀 유닛 
        [SerializeField] private int uniqueUnitSellPriceMultiplier = 9; // 유니크 유닛 
        [SerializeField] private int legendUnitSellPriceMultiplier = 21; // 전설 유닛 
        [SerializeField] private int mythicUnitSellPriceMultiplier = 51; // 신화 유닛 

        private int currentGold;
        private int currentJewel;
        private int currentSummonCost; // 현재 일반 뽑기 비용

        // 재화 변경 이벤트
        public Action<int> OnGoldChanged; // (현재 골드량)
        public Action<int> OnJewelChanged; // (현재 쥬얼량)
        // 뽑기 비용 변경 이벤트
        public Action<int> OnSummonCostChanged; // (현재 뽑기 비용)

        public int Gold => currentGold;
        public int Jewel => currentJewel;
        public int CurrentSummonCost => currentSummonCost;
        public int RareSummonCost => rareSummonCost;
        public int HeroSummonCost => UniqueSummonCost;
        public int LegendSummonCost => legendSummonCost;
        public int NormalUnitSellPrice => (int)(currentSummonCost / 10) * normalUnitSellPriceMultiplier;
        public int RareUnitSellPriceMultiplier => (int)(currentSummonCost / 10) * rareUnitSellPriceMultiplier;
        public int UniqueUnitSellPriceMultiplier => (int)(currentSummonCost / 10) * uniqueUnitSellPriceMultiplier;
        public int LegendUnitSellPriceMultiplier => (int)(currentSummonCost / 10) * legendUnitSellPriceMultiplier;
        public int MythicUnitSellPriceMultiplier => (int)(currentSummonCost / 10) * mythicUnitSellPriceMultiplier;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                currentGold = initialGold;
                currentJewel = initialJewel;
                currentSummonCost = initialSummonCost;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        #region 골드 관리

        /// <summary>
        /// 골드를 추가합니다.
        /// </summary>
        /// <param name="amount">추가할 골드량</param>
        public void AddGold(int amount)
        {
            if (amount <= 0) return;

            currentGold += amount;
            OnGoldChanged?.Invoke(currentGold);
            //Debug.Log($"[IngameCurrency] 골드 추가: +{amount} (현재: {currentGold})");
        }

        /// <summary>
        /// 골드를 소비합니다.
        /// </summary>
        /// <param name="amount">소비할 골드량</param>
        /// <returns>소비 성공 여부</returns>
        public bool SpendGold(int amount)
        {
            if (amount <= 0) return false;
            if (currentGold < amount)
            {
                Debug.LogWarning($"[IngameCurrency] 골드 부족! 필요: {amount}, 현재: {currentGold}");
                return false;
            }

            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            //Debug.Log($"[IngameCurrency] 골드 소비: -{amount} (현재: {currentGold})");
            return true;
        }

        /// <summary>
        /// 골드가 충분한지 확인합니다.
        /// </summary>
        /// <param name="amount">확인할 골드량</param>
        /// <returns>충분한지 여부</returns>
        public bool HasEnoughGold(int amount)
        {
            return currentGold >= amount;
        }

        /// <summary>
        /// 골드를 초기화합니다.
        /// </summary>
        /// <param name="amount">초기 골드량 (기본값: initialGold)</param>
        public void ResetGold(int? amount = null)
        {
            currentGold = amount ?? initialGold;
            OnGoldChanged?.Invoke(currentGold);
            //Debug.Log($"[IngameCurrency] 골드 초기화: {currentGold}");
        }

        #endregion

        #region 쥬얼 관리

        /// <summary>
        /// 쥬얼을 추가합니다.
        /// </summary>
        /// <param name="amount">추가할 쥬얼량</param>
        public void AddJewel(int amount)
        {
            if (amount <= 0) return;

            currentJewel += amount;
            OnJewelChanged?.Invoke(currentJewel);
            //Debug.Log($"[IngameCurrency] 쥬얼 추가: +{amount} (현재: {currentJewel})");
        }

        /// <summary>
        /// 쥬얼을 소비합니다.
        /// </summary>
        /// <param name="amount">소비할 쥬얼량</param>
        /// <returns>소비 성공 여부</returns>
        public bool SpendJewel(int amount)
        {
            if (amount <= 0) return false;
            if (currentJewel < amount)
            {
                Debug.LogWarning($"[IngameCurrency] 쥬얼 부족! 필요: {amount}, 현재: {currentJewel}");
                return false;
            }

            currentJewel -= amount;
            OnJewelChanged?.Invoke(currentJewel);
            //Debug.Log($"[IngameCurrency] 쥬얼 소비: -{amount} (현재: {currentJewel})");
            return true;
        }

        /// <summary>
        /// 쥬얼이 충분한지 확인합니다.
        /// </summary>
        /// <param name="amount">확인할 쥬얼량</param>
        /// <returns>충분한지 여부</returns>
        public bool HasEnoughJewel(int amount)
        {
            return currentJewel >= amount;
        }

        /// <summary>
        /// 쥬얼을 초기화합니다.
        /// </summary>
        /// <param name="amount">초기 쥬얼량 (기본값: initialJewel)</param>
        public void ResetJewel(int? amount = null)
        {
            currentJewel = amount ?? initialJewel;
            OnJewelChanged?.Invoke(currentJewel);
            //Debug.Log($"[IngameCurrency] 쥬얼 초기화: {currentJewel}");
        }

        #endregion

        #region 일반 뽑기 (골드 사용)

        /// <summary>
        /// 일반 뽑기 비용을 지불하고 다음 뽑기 비용을 증가시킵니다.
        /// </summary>
        /// <returns>뽑기 성공 여부</returns>
        public bool SpendSummonCost()
        {
            if (!SpendGold(currentSummonCost))
            {
                return false;
            }

            // 뽑기 비용 증가
            currentSummonCost += costIncreasePerSummon;
            OnSummonCostChanged?.Invoke(currentSummonCost);
            //Debug.Log($"[IngameCurrency] 일반 뽑기 완료! 다음 뽑기 비용: {currentSummonCost}");

            return true;
        }

        /// <summary>
        /// 일반 뽑기 비용을 초기화합니다.
        /// </summary>
        public void ResetSummonCost()
        {
            currentSummonCost = initialSummonCost;
            OnSummonCostChanged?.Invoke(currentSummonCost);
            //Debug.Log($"[IngameCurrency] 일반 뽑기 비용 초기화: {currentSummonCost}");
        }

        /// <summary>
        /// 일반 뽑기 비용이 충분한지 확인합니다.
        /// </summary>
        /// <returns>충분한지 여부</returns>
        public bool HasEnoughSummonCost()
        {
            return HasEnoughGold(currentSummonCost);
        }

        #endregion

        #region 특정 등급 뽑기 (쥬얼 사용)

        /// <summary>
        /// 희귀 등급 뽑기 비용을 지불합니다.
        /// </summary>
        /// <returns>뽑기 성공 여부</returns>
        public bool SpendRareSummonCost()
        {
            return SpendJewel(rareSummonCost);
        }

        /// <summary>
        /// 영웅 등급 뽑기 비용을 지불합니다.
        /// </summary>
        /// <returns>뽑기 성공 여부</returns>
        public bool SpendUniqueSummonCost()
        {
            return SpendJewel(UniqueSummonCost);
        }

        /// <summary>
        /// 전설 등급 뽑기 비용을 지불합니다.
        /// </summary>
        /// <returns>뽑기 성공 여부</returns>
        public bool SpendLegendSummonCost()
        {
            return SpendJewel(legendSummonCost);
        }

        /// <summary>
        /// 희귀 등급 뽑기 비용이 충분한지 확인합니다.
        /// </summary>
        /// <returns>충분한지 여부</returns>
        public bool HasEnoughRareSummonCost()
        {
            return HasEnoughJewel(rareSummonCost);
        }

        /// <summary>
        /// 영웅 등급 뽑기 비용이 충분한지 확인합니다.
        /// </summary>
        /// <returns>충분한지 여부</returns>
        public bool HasEnoughHeroSummonCost()
        {
            return HasEnoughJewel(UniqueSummonCost);
        }

        /// <summary>
        /// 전설 등급 뽑기 비용이 충분한지 확인합니다.
        /// </summary>
        /// <returns>충분한지 여부</returns>
        public bool HasEnoughLegendSummonCost()
        {
            return HasEnoughJewel(legendSummonCost);
        }

        #endregion
    }
}
