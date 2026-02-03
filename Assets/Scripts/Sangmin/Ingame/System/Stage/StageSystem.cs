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
        public float breakTime = 4f;
        public float bossWaveDuration = 60f; // 보스 웨이브 지속 시간 (60초)
        public float normalWaveDuration = 20f; // 일반 웨이브 지속 시간 (20초)
        public float spawnDuration = 15f; // 스폰 지속 시간 (60초)
        public float spawnInterval = 0.25f; // 스폰 간격 (0.5초)
        public int enemiesPerSpawn = 1; // 한 번에 스폰되는 적 수
        public int maxEnemyCount = 60; // 최대 적 수 (한계)

        public string normalEnemyPath;
        public GameObject normalEnemyPrefab;

        public string bossEnemyPath;
        public GameObject bossEnemyPrefab;
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

        [Header("스테이지 설정")]
        public String stageName;
        public TextAsset stageConfigJson;
        public StageData currentStage;
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
        public float remainingWaveTime
        {
            get
            {
                if (waveStartTime < 0 || currentWaveDuration <= 0) return 0f;

                float elapsed = Time.time - waveStartTime;
                float remaining = currentWaveDuration - elapsed;
                return Mathf.Max(0f, remaining);
            }
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
        }

        public void Init()
        {
            if (stageConfigJson != null)
            {
                try
                {
                    stageList = JsonUtility.FromJson<StageDataList>(stageConfigJson.text);

                    currentStage = GetStageData(stageName);

                    if (!string.IsNullOrEmpty(currentStage.normalEnemyPath))
                    {
                        currentStage.normalEnemyPrefab = LoadEnemyPrefabByPath(currentStage.normalEnemyPath);
                        ObjectPoolManager.Instance.AddObjectInfo(currentStage.normalEnemyPrefab, currentStage.maxEnemyCount / 2);
                    }
                    if (!string.IsNullOrEmpty(currentStage.bossEnemyPath))
                    {
                        currentStage.bossEnemyPrefab = LoadEnemyPrefabByPath(currentStage.bossEnemyPath);
                        ObjectPoolManager.Instance.AddObjectInfo(currentStage.bossEnemyPrefab, 1);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Stage Json 파싱 실패: {e.Message}");
                    stageList = new StageDataList();
                }
            }
        }

        public void StartCode()
        {
            currentWave = -1;
            waveStartTime = 0f;
            currentWaveDuration = 0f;
            IsGameOver = false;

            StartNextWave();
        }

        private StageData GetStageData(string _stageName)
        {
            return stageList.stages.Find(x => x.stageName == _stageName);
        }

        /// <summary>
        /// Resources 경로를 이용해 Enemy 프리팹을 동적으로 로드합니다.
        /// </summary>
        /// <param name="enemyPath">Resources 기준 경로 또는 프리팹 이름</param>
        /// <returns>찾은 GameObject 프리팹, 실패 시 null</returns>
        private GameObject LoadEnemyPrefabByPath(string enemyPath)
        {
            if (string.IsNullOrEmpty(enemyPath))
            {
                Debug.LogError("Enemy 프리팹 경로가 비어 있습니다.");
                return null;
            }

            // 1. Resources.Load 를 사용하여 프리팹을 로드합니다.
            //    enemyPath 에 "Prefabs/Enemies/Normal/Slime" 처럼 경로를 넣으면,
            //    Assets/Resources/Prefabs/Enemies/Normal/Slime.prefab 를 찾습니다.
            GameObject prefab = Resources.Load<GameObject>(enemyPath);

            if (prefab == null)
            {
                Debug.LogError($"Enemy 프리팹을 찾지 못했습니다. 경로: {enemyPath}  (Resources 폴더 위치와 이름을 확인하세요)");
            }

            return prefab;
        }

        public void StartNextWave()
        {
            if (IsGameOver) return;

            currentWave++;
            waveStartTime = Time.time;

            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            if (currentWave == 0)
            {
                currentWaveDuration = currentStage.breakTime;
                spawnCoroutine = StartCoroutine(IEBreakTime());
                return;
            }
            else
            {
                // 웨이브 타입에 따른 지속 시간 설정
                if (currentWave % 5 == 0)
                {
                    currentWaveDuration = currentStage.bossWaveDuration;
                    spawnCoroutine = StartCoroutine(IEBossWave());
                }
                else
                {
                    currentWaveDuration = currentStage.normalWaveDuration;
                    spawnCoroutine = StartCoroutine(IENormalWave());
                }
            }

            OnWaveStart?.Invoke(currentWave);
        }

        IEnumerator IEBreakTime()
        {
            float elapsedTime = 0f;

            Debug.Log($"쉬는 시간 {currentWave} 시작");

            // 웨이브 전체 시간 동안 반복
            while (elapsedTime <= currentStage.breakTime && !IsGameOver)
            {
                OnFrameUpdate?.Invoke();

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // 웨이브 종료 - 다음 웨이브로 진행
            if (!IsGameOver)
            {
                //Debug.Log($"쉬는 시간 {currentWave} 종료. 다음 웨이브 준비...");
                StartNextWave();
            }
        }

        IEnumerator IENormalWave()
        {
            float elapsedTime = 0f;
            float spawnElapsedTime = currentStage.spawnInterval;

            // Debug.Log($"일반 웨이브 {currentWave} 시작");

            // 웨이브 전체 시간 동안 반복
            while (elapsedTime < currentStage.normalWaveDuration && !IsGameOver)
            {
                OnFrameUpdate?.Invoke();

                elapsedTime += Time.deltaTime;

                if (elapsedTime <= currentStage.spawnDuration)
                {
                    spawnElapsedTime += Time.deltaTime;

                    // 스폰 간격마다 적 스폰
                    if (spawnElapsedTime >= currentStage.spawnInterval)
                    {
                        spawnElapsedTime = 0f;

                        // 한 번에 지정된 수만큼 스폰
                        for (int i = 0; i < currentStage.enemiesPerSpawn; i++)
                        {
                            SpawnNormalEnemy();

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
            float elapsedTime = 0f;
            float spawnElapsedTime = 0f;

            SpawnBossEnemy();

            Debug.Log($"보스 웨이브 {currentWave} 시작");

            // 웨이브 전체 시간 동안 반복
            while (elapsedTime <= currentStage.bossWaveDuration && !IsGameOver)
            {
                OnFrameUpdate?.Invoke();

                elapsedTime += Time.deltaTime;

                if (elapsedTime <= currentStage.spawnDuration * 2)
                {
                    spawnElapsedTime += Time.deltaTime;

                    // 스폰 간격마다 적 스폰
                    if (spawnElapsedTime >= currentStage.spawnInterval * 2)
                    {
                        spawnElapsedTime = 0f;

                        // 한 번에 지정된 수만큼 스폰
                        for (int i = 0; i < currentStage.enemiesPerSpawn; i++)
                        {
                            SpawnNormalEnemy();

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
                Debug.Log($"보스 웨이브 {currentWave} 종료. 다음 웨이브 준비...");
                StartNextWave();
            }
        }

        private void SpawnNormalEnemy()
        {
            if (currentStage.normalEnemyPrefab == null)
            {
                Debug.LogError("Normal Enemy Prefab이 할당되지 않았습니다!");
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

            // 일반 몬스터로 설정
            if (enemy != null)
            {
                enemy.isBoss = false;
            }

            // 활성 적 리스트에 추가
            activeEnemies.Add(enemy);
            OnEnemyCountChanged?.Invoke(activeEnemies.Count);

            //Debug.Log($"적 스폰: 현재 적 수 = {activeEnemies.Count}");
        }

        private void SpawnBossEnemy()
        {
            if (currentStage.bossEnemyPrefab == null)
            {
                Debug.LogError("Boss Enemy Prefab이 할당되지 않았습니다!");
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
                enemyObject = ObjectPoolManager.Instance.GetObject(currentStage.bossEnemyPrefab);
            }

            if (enemyObject == null)
            {
                Debug.LogError("적 생성 실패!");
                return;
            }

            // 스폰 위치 설정
            enemyObject.transform.position = EnemyMoveRoute.Instance.startPosition;

            var enemy = enemyObject.GetComponent<Enemy>();

            // 보스 몬스터로 설정 및 골드 보상 계산
            if (enemy != null)
            {
                enemy.isBoss = true;
                enemy.RecalculateBossGoldReward();
            }

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
                //Debug.Log($"적 제거: 현재 적 수 = {activeEnemies.Count}");
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