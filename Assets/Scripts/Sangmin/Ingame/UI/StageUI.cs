using UnityEngine;
using TMPro;

namespace Sangmin
{
    /// <summary>
    /// 스테이지 정보를 표시하는 UI 클래스
    /// currentWave, Timer, currentEnemyCount를 표시합니다.
    /// </summary>
    public class StageUI : MonoBehaviour
    {
        public static StageUI Instance;

        [Header("UI Text References")]
        [Tooltip("현재 웨이브를 표시할 텍스트")]
        public TMP_Text waveText;

        [Tooltip("타이머를 표시할 텍스트")]
        public TMP_Text timerText;

        [Tooltip("현재 적 수를 표시할 텍스트")]
        public TMP_Text enemyCountText;

        private StageSystem stageSystem;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // StageSystem 찾기
            stageSystem = StageSystem.Instance;

            if (stageSystem == null)
            {
                Debug.LogError("StageSystem.Instance를 찾을 수 없습니다!");
                return;
            }

            // 이벤트 구독
            stageSystem.OnFrameUpdate += OnFrameUpdate;
            stageSystem.OnWaveStart += OnWaveStart;
            stageSystem.OnEnemyCountChanged += OnEnemyCountChanged;

            // 초기 UI 업데이트
            RefreshAll();
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (stageSystem != null)
            {
                stageSystem.OnFrameUpdate -= OnFrameUpdate;
                stageSystem.OnWaveStart -= OnWaveStart;
                stageSystem.OnEnemyCountChanged -= OnEnemyCountChanged;
            }
        }

        /// <summary>
        /// 모든 UI 요소를 갱신합니다.
        /// </summary>
        public void RefreshAll()
        {
            if (stageSystem == null) return;

            UpdateWave();
            UpdateTimer();
            UpdateEnemyCount();
        }

        /// <summary>
        /// 웨이브 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateWave()
        {
            if (waveText == null || stageSystem == null) return;

            waveText.text = $"Wave: {stageSystem.currentWave}";
        }

        /// <summary>
        /// 타이머 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateTimer()
        {
            if (timerText == null || stageSystem == null) return;

            float remainingTime = stageSystem.GetRemainingWaveTime();

            // 시간을 분:초 형식으로 표시
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// 적 수 텍스트를 업데이트합니다.
        /// </summary>
        public void UpdateEnemyCount()
        {
            if (enemyCountText == null || stageSystem == null) return;

            enemyCountText.text = $"{stageSystem.CurrentEnemyCount}/{stageSystem.currentStage.maxEnemyCount}";
        }

        /// <summary>
        /// 프레임마다 이벤트
        /// </summary>
        private void OnFrameUpdate()
        {
            UpdateTimer();
        }

        /// <summary>
        /// 웨이브 시작 이벤트 핸들러
        /// </summary>
        private void OnWaveStart(int waveNumber)
        {
            UpdateWave();
            UpdateTimer();
        }

        /// <summary>
        /// 적 수 변경 이벤트 핸들러
        /// </summary>
        private void OnEnemyCountChanged(int count)
        {
            UpdateEnemyCount();
        }
    }
}
