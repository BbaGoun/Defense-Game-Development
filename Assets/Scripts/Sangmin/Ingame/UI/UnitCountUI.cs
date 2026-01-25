using UnityEngine;
using TMPro;

namespace Sangmin
{
    /// <summary>
    /// 현재 유닛 수와 최대 유닛 수를 표시하는 UI 클래스
    /// </summary>
    public class UnitCountUI : MonoBehaviour
    {
        private static UnitCountUI _instance;
        public static UnitCountUI Instance
        {
            get
            {
                return _instance;
            }
        }

        [Header("UI Text References")]
        [Tooltip("현재 유닛 수/최대 유닛 수를 표시할 텍스트")]
        public TMP_Text unitCountText;

        private GridUnitPlacement gridUnitPlacement;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // GridUnitPlacement 찾기
            gridUnitPlacement = GridUnitPlacement.Instance;

            if (gridUnitPlacement == null)
            {
                Debug.LogError("GridUnitPlacement.Instance를 찾을 수 없습니다!");
                return;
            }

            // 이벤트 구독
            gridUnitPlacement.OnUnitCountChanged += OnUnitCountChanged;

            // 초기 UI 업데이트
            RefreshUI();
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (gridUnitPlacement != null)
            {
                gridUnitPlacement.OnUnitCountChanged -= OnUnitCountChanged;
            }

            if (Instance == this)
                _instance = null;
        }

        /// <summary>
        /// UI를 갱신합니다.
        /// </summary>
        public void RefreshUI()
        {
            if (gridUnitPlacement == null) return;

            UpdateUnitCount();
        }

        /// <summary>
        /// 유닛 수 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateUnitCount()
        {
            if (unitCountText == null || gridUnitPlacement == null) return;

            int current = gridUnitPlacement.UnitCount;
            int max = gridUnitPlacement.UnitCountMax;
            bool isLimitReached = gridUnitPlacement.IsUnitLimitReached;

            // 한계에 도달했으면 빨간색으로 표시
            if (isLimitReached)
            {
                string formattedCurrent = current.ToString("D2"); // 두 자리로 포맷 (01, 02, ...)
                unitCountText.text = $"<color=red>{formattedCurrent}/{max}</color>";
                unitCountText.color = Color.red;
            }
            else
            {
                string formattedCurrent = current.ToString("D2"); // 두 자리로 포맷 (01, 02, ...)
                unitCountText.text = $"{formattedCurrent}/{max}";
                unitCountText.color = Color.white;
            }
        }

        /// <summary>
        /// 유닛 수 변경 이벤트 핸들러
        /// </summary>
        private void OnUnitCountChanged(int current, int max)
        {
            UpdateUnitCount();
        }
    }
}
