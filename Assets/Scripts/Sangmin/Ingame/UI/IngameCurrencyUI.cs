using UnityEngine;
using TMPro;

namespace Sangmin
{
    /// <summary>
    /// 디펜스 게임 내부 재화(골드, 쥬얼)와 뽑기 비용을 표시하는 UI 클래스
    /// </summary>
    public class IngameCurrencyUI : MonoBehaviour
    {
        private static IngameCurrencyUI _instance;
        public static IngameCurrencyUI Instance
        {
            get
            {
                return _instance;
            }
        }

        [Header("UI Text References")]
        [Tooltip("현재 골드량을 표시할 텍스트")]
        public TMP_Text goldText;

        [Tooltip("현재 쥬얼량을 표시할 텍스트")]
        public TMP_Text jewelText;

        [Tooltip("일반 뽑기 비용을 표시할 텍스트")]
        public TMP_Text summonCostText;

        [Tooltip("희귀 등급 뽑기 비용을 표시할 텍스트 (선택사항)")]
        public TMP_Text rareSummonCostText;

        [Tooltip("영웅 등급 뽑기 비용을 표시할 텍스트 (선택사항)")]
        public TMP_Text heroSummonCostText;

        [Tooltip("전설 등급 뽑기 비용을 표시할 텍스트 (선택사항)")]
        public TMP_Text legendSummonCostText;

        private IngameCurrencyManager currencyManager;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // IngameCurrencyManager 찾기
            currencyManager = IngameCurrencyManager.Instance;

            if (currencyManager == null)
            {
                Debug.LogError("IngameCurrencyManager.Instance를 찾을 수 없습니다!");
                return;
            }

            // 이벤트 구독
            currencyManager.OnGoldChanged += OnGoldChanged;
            currencyManager.OnJewelChanged += OnJewelChanged;
            currencyManager.OnSummonCostChanged += OnSummonCostChanged;

            // 초기 UI 업데이트
            RefreshAll();
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (currencyManager != null)
            {
                currencyManager.OnGoldChanged -= OnGoldChanged;
                currencyManager.OnJewelChanged -= OnJewelChanged;
                currencyManager.OnSummonCostChanged -= OnSummonCostChanged;
            }

            if (Instance == this)
                _instance = null;
        }

        /// <summary>
        /// 모든 UI 요소를 갱신합니다.
        /// </summary>
        public void RefreshAll()
        {
            if (currencyManager == null) return;

            UpdateGold();
            UpdateJewel();
            UpdateSummonCost();
            UpdateGradeSummonCosts();
        }

        /// <summary>
        /// 골드 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateGold()
        {
            if (goldText == null || currencyManager == null) return;

            goldText.text = $"{currencyManager.Gold}";
        }

        /// <summary>
        /// 쥬얼 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateJewel()
        {
            if (jewelText == null || currencyManager == null) return;

            jewelText.text = $"{currencyManager.Jewel}";
        }

        /// <summary>
        /// 일반 뽑기 비용 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateSummonCost()
        {
            if (summonCostText == null || currencyManager == null) return;

            int cost = currencyManager.CurrentSummonCost;
            bool canAfford = currencyManager.HasEnoughSummonCost();

            // 골드가 부족하면 빨간색으로 표시
            if (canAfford)
            {
                summonCostText.text = $"비용: {cost}";
                summonCostText.color = Color.white;
            }
            else
            {
                summonCostText.text = $"비용: <color=red>{cost}</color>";
                summonCostText.color = Color.red;
            }
        }

        /// <summary>
        /// 등급별 뽑기 비용 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateGradeSummonCosts()
        {
            if (currencyManager == null) return;

            // 희귀 등급
            if (rareSummonCostText != null)
            {
                int cost = currencyManager.RareSummonCost;
                bool canAfford = currencyManager.HasEnoughRareSummonCost();
                rareSummonCostText.text = canAfford
                    ? $"희귀: {cost}"
                    : $"희귀: <color=red>{cost}</color>";
                rareSummonCostText.color = canAfford ? Color.white : Color.red;
            }

            // 영웅 등급
            if (heroSummonCostText != null)
            {
                int cost = currencyManager.HeroSummonCost;
                bool canAfford = currencyManager.HasEnoughHeroSummonCost();
                heroSummonCostText.text = canAfford
                    ? $"영웅: {cost}"
                    : $"영웅: <color=red>{cost}</color>";
                heroSummonCostText.color = canAfford ? Color.white : Color.red;
            }

            // 전설 등급
            if (legendSummonCostText != null)
            {
                int cost = currencyManager.LegendSummonCost;
                bool canAfford = currencyManager.HasEnoughLegendSummonCost();
                legendSummonCostText.text = canAfford
                    ? $"전설: {cost}"
                    : $"전설: <color=red>{cost}</color>";
                legendSummonCostText.color = canAfford ? Color.white : Color.red;
            }
        }

        /// <summary>
        /// 골드 변경 이벤트 핸들러
        /// </summary>
        private void OnGoldChanged(int newGold)
        {
            UpdateGold();
            UpdateSummonCost(); // 골드가 변경되면 뽑기 비용 표시도 업데이트 (색상 변경)
        }

        /// <summary>
        /// 쥬얼 변경 이벤트 핸들러
        /// </summary>
        private void OnJewelChanged(int newJewel)
        {
            UpdateJewel();
            UpdateGradeSummonCosts(); // 쥬얼이 변경되면 등급별 뽑기 비용 표시도 업데이트 (색상 변경)
        }

        /// <summary>
        /// 일반 뽑기 비용 변경 이벤트 핸들러
        /// </summary>
        private void OnSummonCostChanged(int newCost)
        {
            UpdateSummonCost();
        }
    }
}
