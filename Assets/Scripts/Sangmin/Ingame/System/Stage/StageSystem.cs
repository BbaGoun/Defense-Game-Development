using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sangmin;
using System;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem.LowLevel;

namespace Sangmin
{
    [Serializable]
    public class StageData
    {
        public string stageName;
        public float bossWaveDuration = 60f; // 보스 웨이브 지속 시간 (60초)
        public float normalWaveDuration = 20f; // 일반 웨이브 지속 시간 (20초)
        public float spawnDuration = 15f; // 스폰 지속 시간 (60초)
        public float spawnInterval = 5f; // 스폰 간격 (0.5초)
        public int enemiesPerSpawn = 1; // 한 번에 스폰되는 적 수
        public int maxEnemyCount = 60; // 최대 적 수 (한계)

        public string normalEnemyName;
        public GameObject normalEnemyPrefab;

        public string bossEnemyName;
        public GameObject bossPrefab;
    }

    [Serializable]
    public class StageDataList
    {
        [ReadOnly(true)]
        public List<StageData> stages = new List<StageData>();
    }

    public class StageSystem : MonoBehaviour
    {
        private static StageSystem _instance;
        public static StageSystem Instance
        {
            get { return _instance; }
        }

        #region 직렬화
        [Serializable]
        private class NameToEnemyPrefabDictionary : SerializableDictionary<String, GameObject> { }
        #endregion

        [Header("스테이지 설정")]
        public TextAsset stageConfigJson;
        public StageData currentStage;
        public List<GameObject> normalEnemyList = new List<GameObject>();
        [SerializeField]
        private NameToEnemyPrefabDictionary normalEnemyDic = new NameToEnemyPrefabDictionary();
        public StageDataList stageList = new StageDataList();

        [Header("현 상황")]
        public int currentWave { get; private set; }
        public float waveStartTime;
        public float currentWaveDuration;
        private Coroutine spawnCoroutine;

        // 현재 활성화된 적 리스트
        private List<Enemy> activeEnemies = new List<Enemy>();

        // 이벤트
        public Action OnGameOver; // 게임 오버 이벤트
        public Action OnFrameUpdate; // 프레임 업데이트 이벤트
        public Action<int> OnWaveStart; // 웨이브 시작 이벤트 (웨이브 번호)
        public Action<int> OnEnemyCountChanged; // 적 수 변경 이벤트 (현재 적 수)

        public int CurrentEnemyCount => activeEnemies.Count;
        public bool IsGameOver { get; private set; }

        public List<Enemy> GetActiveEnemies()
        {
            activeEnemies.RemoveAll(x => x == null);
            return activeEnemies;
        }

        /// <summary>
        /// 현재 웨이브의 남은 시간을 반환합니다 (초 단위)
        /// </summary>
        public float GetRemainingWaveTime()
        {
            if (waveStartTime < 0 || currentWaveDuration <= 0) return 0f;

            float elapsed = Time.time - waveStartTime;
            float remaining = currentWaveDuration - elapsed;
            return Mathf.Max(0f, remaining);
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            foreach (var normalEnemy in normalEnemyList)
            {
                normalEnemyDic.KeyValuePair[normalEnemy.name] = normalEnemy;
            }
        }

        private void Start()
        {
            if (stageConfigJson != null)
            {
                try
                {
                    stageList = JsonUtility.FromJson<StageDataList>(stageConfigJson.text);

                    foreach (var stage in stageList.stages)
                    {
                        if (normalEnemyDic.KeyValuePair.ContainsKey(stage.normalEnemyName))
                        {
                            stage.normalEnemyPrefab = normalEnemyDic.KeyValuePair[stage.normalEnemyName];
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Stage Json 파싱 실패: {e.Message}");
                    stageList = new StageDataList();
                }
            }

            currentWave = 0;
            waveStartTime = 0f;
            currentWaveDuration = 0f;
            IsGameOver = false;

            currentStage = GetStageData("1-1");

            StartNextWave();
        }

        private StageData GetStageData(string _stageName)
        {
            return stageList.stages.Find(x => x.stageName == _stageName);
        }

        public void StartNextWave()
        {
            if (IsGameOver) return;

            currentWave++;
            waveStartTime = Time.time;

            // 웨이브 타입에 따른 지속 시간 설정
            if (currentWave % 5 == 0)
            {
                currentWaveDuration = currentStage.bossWaveDuration;
            }
            else
            {
                currentWaveDuration = currentStage.normalWaveDuration;
            }

            OnWaveStart?.Invoke(currentWave);

            SpawnWave();
        }

        public void SpawnWave()
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            // 일반 웨이브
            if (currentWave % 5 != 0)
                spawnCoroutine = StartCoroutine(IENormalWave());
            // 보스 웨이브
            else
                spawnCoroutine = StartCoroutine(IEBossWave());
        }

        IEnumerator IENormalWave()
        {
            float elapsedTime = 0f;
            float spawnElapsedTime = 0f;

            Debug.Log($"일반 웨이브 {currentWave} 시작");

            // 웨이브 전체 시간 동안 반복
            while (elapsedTime < currentStage.normalWaveDuration && !IsGameOver)
            {
                OnFrameUpdate?.Invoke();

                elapsedTime += Time.deltaTime;

                // 처음 지정한 시간 동안만 스폰
                if (elapsedTime < currentStage.spawnDuration)
                {
                    spawnElapsedTime += Time.deltaTime;

                    // 스폰 간격마다 적 스폰
                    if (spawnElapsedTime >= currentStage.spawnInterval)
                    {
                        spawnElapsedTime = 0f;

                        // 한 번에 지정된 수만큼 스폰
                        for (int i = 0; i < currentStage.enemiesPerSpawn; i++)
                        {
                            SpawnEnemy();

                            // 게임 오버 체크
                            if (CheckGameOver())
                            {
                                yield break;
                            }
                        }
                    }
                }

                yield return null;
            }

            // 웨이브 종료 - 다음 웨이브로 진행
            if (!IsGameOver)
            {
                Debug.Log($"일반 웨이브 {currentWave} 종료. 다음 웨이브 준비...");
                StartNextWave();
            }
        }

        IEnumerator IEBossWave()
        {
            Debug.Log($"보스 웨이브 {currentWave} 시작");
            yield return null;
        }

        /// <summary>
        /// 적을 스폰합니다.
        /// </summary>
        private void SpawnEnemy()
        {
            if (currentStage.normalEnemyPrefab == null)
            {
                Debug.LogError("Enemy Prefab이 할당되지 않았습니다!");
                return;
            }

            if (EnemyMoveRoute.Instance == null)
            {
                Debug.LogError("EnemyMoveRoute.Instance가 없습니다!");
                return;
            }

            GameObject enemyObject = null;

            // 오브젝트 풀링 사용 시도
            if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.IsReady)
            {
                enemyObject = ObjectPoolManager.Instance.GetObject(currentStage.normalEnemyPrefab);
            }

            if (enemyObject == null)
            {
                Debug.LogError("적 생성 실패!");
                return;
            }

            // 스폰 위치 설정
            enemyObject.transform.position = EnemyMoveRoute.Instance.startPosition;

            var enemy = enemyObject.GetComponent<Enemy>();

            // 활성 적 리스트에 추가
            activeEnemies.Add(enemy);
            OnEnemyCountChanged?.Invoke(activeEnemies.Count);

            //Debug.Log($"적 스폰: 현재 적 수 = {activeEnemies.Count}");
        }

        /// <summary>
        /// 적이 제거되었을 때 호출합니다.
        /// </summary>
        public void OnEnemyDestroyed(Enemy enemy)
        {
            if (enemy == null) return;

            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
                OnEnemyCountChanged?.Invoke(activeEnemies.Count);
                Debug.Log($"적 제거: 현재 적 수 = {activeEnemies.Count}");
            }
        }

        /// <summary>
        /// 게임 오버 조건을 체크합니다.
        /// </summary>
        private bool CheckGameOver()
        {
            if (IsGameOver) return true;

            // 일반 웨이브일 때만 적 수 체크
            if (currentWave % 5 != 0)
            {
                if (activeEnemies.Count >= currentStage.maxEnemyCount)
                {
                    GameOver();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 게임 오버 처리
        /// </summary>
        private void GameOver()
        {
            if (IsGameOver) return;

            IsGameOver = true;
            Debug.Log($"게임 오버! 현재 적 수: {activeEnemies.Count}, 한계: {currentStage.maxEnemyCount}");

            // 스폰 코루틴 중지
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }

            // 게임 오버 이벤트 발생
            OnGameOver?.Invoke();
        }

        /// <summary>
        /// 게임을 재시작합니다.
        /// </summary>
        public void RestartGame()
        {
            IsGameOver = false;
            currentWave = 0;

            // 모든 활성 적 제거
            ClearAllEnemies();

            Debug.Log("게임 재시작");
        }

        /// <summary>
        /// 모든 적을 제거합니다.
        /// </summary>
        private void ClearAllEnemies()
        {
            // 리스트 복사 후 제거 (순회 중 수정 방지)
            List<Enemy> enemiesToRemove = new List<Enemy>(activeEnemies);

            foreach (var enemy in enemiesToRemove)
            {
                if (enemy != null && enemy.gameObject != null)
                {
                    // 오브젝트 풀링 사용 시도
                    PoolAble poolAble = enemy.GetComponent<PoolAble>();
                    if (poolAble != null)
                    {
                        poolAble.ReleaseObject();
                    }
                    else
                    {
                        Destroy(enemy.gameObject);
                    }
                }
            }

            activeEnemies.Clear();
            OnEnemyCountChanged?.Invoke(0);
        }
    }
}