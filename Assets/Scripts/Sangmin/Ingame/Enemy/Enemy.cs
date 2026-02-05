using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sangmin
{
    [RequireComponent(typeof(PoolAble), typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class Enemy : MonoBehaviour
    {

        [Header("Stats")]
        [SerializeField] private float moveSpeed = 3f; // 이동 속도
        [SerializeField] private float maxHealth = 100f; // 최대 체력
        private float currentHealth; // 현재 체력
        public bool isBoss { get; set; } // 보스 여부

        [Header("UI")]
        [SerializeField] private RectTransform healthBarRect;
        [SerializeField] private RectTransform healthBarFill;

        private float canvasWidth;

        private PoolAble poolAble;
        private Coroutine moveCoroutine;

        // 몬스터 생성 시점의 웨이브와 골드 보상 (악용 방지)
        private int spawnWave = -1;
        private int goldReward = 0;


        void Awake()
        {
            canvasWidth = healthBarRect.rect.width;

            // 체력 초기화
            currentHealth = maxHealth;
            UpdateHealthBar();
            poolAble = GetComponent<PoolAble>();
        }

        void OnEnable()
        {
            if (EnemyMoveRoute.Instance != null)
            {
                EnemyMoveRoute.Instance.OnGenerateRoute += OnGenerateRoute;
            }

            transform.position = EnemyMoveRoute.Instance.startPosition;

            moveCoroutine = StartCoroutine(MoveAlongRoute(EnemyMoveRoute.Instance.WorldRoute));

            // 오브젝트 풀링 사용 시 OnEnable에서도 체력 초기화
            currentHealth = maxHealth;
            UpdateHealthBar();

            // isBoss는 스폰 시 설정되므로 여기서는 초기화만 (기본값 false)
            isBoss = false;

            // 몬스터 생성 시점의 웨이브를 저장하고 골드 보상을 계산 (악용 방지)
            // 일반 몬스터는 여기서 계산, 보스는 SpawnBossEnemy()에서 isBoss 설정 후 재계산됨
            if (StageSystem.Instance != null)
            {
                spawnWave = StageSystem.Instance.currentWave;
                // 기본적으로 일반 몬스터 골드 계산 (보스는 나중에 재계산됨)
                goldReward = CalculateGoldReward(spawnWave);
            }
            else
            {
                spawnWave = -1;
                goldReward = 0;
            }
        }

        /// <summary>
        /// 데미지를 받습니다.
        /// </summary>
        /// <param name="damage">받을 데미지</param>
        public bool TakeDamage(float damage)
        {
            if (damage <= 0) return true;
            if (currentHealth <= 0) return true;

            //Debug.Log($"맞기 전: {gameObject.name}, hp:{currentHealth}");

            currentHealth -= damage;
            currentHealth = Mathf.Max(0f, currentHealth);

            //Debug.Log($"맞은 후: {gameObject.name}, hp:{currentHealth}");

            UpdateHealthBar();

            // 체력이 0 이하가 되면 처치
            if (currentHealth <= 0f)
            {
                Die();
            }

            return false;
        }

        /// <summary>
        /// 적을 처치합니다.
        /// </summary>
        private void Die()
        {
            // 골드 보상 지급 (디펜스 게임 내부 골드)
            // 생성 시점에 계산된 골드를 사용하여 악용 방지
            if (IngameCurrencyManager.Instance != null && goldReward > 0)
            {
                IngameCurrencyManager.Instance.AddGold(goldReward);
                string enemyType = isBoss ? "보스" : "일반 몬스터";
                Debug.Log($"{enemyType} 처치! 골드 +{goldReward} (생성 웨이브: {spawnWave}, 현재 웨이브: {(StageSystem.Instance != null ? StageSystem.Instance.currentWave.ToString() : "N/A")})");
            }

            // 쥬얼 보상 시스템 이벤트: 보스 처치
            if (isBoss && spawnWave > 0)
            {
                JewelEventBus.RaiseBossKilled(spawnWave);
            }

            if (poolAble != null)
            {
                poolAble.ReleaseObject();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 일반 몬스터의 웨이브에 따른 골드 보상을 계산합니다.
        /// 5스테이지마다 골드가 증가하는 방식입니다.
        /// </summary>
        /// <param name="wave">웨이브 번호</param>
        /// <returns>보상 골드량</returns>
        private int CalculateGoldReward(int wave)
        {
            // 웨이브가 0 이하이면 기본 골드 반환
            if (wave <= 0)
            {
                return 3;
            }

            // branch_index = (wave - 1) / 5
            int branchIndex = (wave - 1) / 5;

            // gold_per_monster = 3 + (branch_index * 2)
            int reward = 3 + (branchIndex * 2);

            return reward;
        }

        /// <summary>
        /// 보스의 웨이브에 따른 골드 보상을 계산합니다.
        /// 보스 타이밍: 10, 20, 30, 40, 50, 60, 65
        /// </summary>
        /// <param name="wave">웨이브 번호</param>
        /// <returns>보상 골드량</returns>
        private int CalculateBossGoldReward(int wave)
        {
            // 보스는 10의 배수 웨이브 또는 65에서 등장
            // 특별 케이스: 65 웨이브
            if (wave == 65)
            {
                return 2520;
            }

            // 10의 배수 웨이브가 아니면 기본값 반환 (보스가 아닐 수도 있음)
            if (wave % 10 != 0)
            {
                return 0;
            }

            // 10의 배수 웨이브: 540 + (wave/10 - 1) * 360
            // wave 10: 540 + (1 - 1) * 360 = 540
            // wave 20: 540 + (2 - 1) * 360 = 900
            // wave 30: 540 + (3 - 1) * 360 = 1260
            // wave 40: 540 + (4 - 1) * 360 = 1620
            // wave 50: 540 + (5 - 1) * 360 = 1980
            // wave 60: 540 + (6 - 1) * 360 = 2340
            int bossIndex = wave / 10;
            int reward = 540 + (bossIndex - 1) * 360;

            return reward;
        }

        /// <summary>
        /// 현재 체력을 반환합니다.
        /// </summary>
        public float CurrentHealth => currentHealth;

        /// <summary>
        /// 최대 체력을 반환합니다.
        /// </summary>
        public float MaxHealth => maxHealth;

        /// <summary>
        /// 체력 비율을 반환합니다 (0~1).
        /// </summary>
        public float HealthRatio => maxHealth > 0 ? currentHealth / maxHealth : 0f;

        /// <summary>
        /// 보스 골드 보상을 재계산합니다. (SpawnBossEnemy()에서 호출)
        /// </summary>
        public void RecalculateBossGoldReward()
        {
            if (isBoss && StageSystem.Instance != null && spawnWave >= 0)
            {
                goldReward = CalculateBossGoldReward(spawnWave);
            }
        }

        void OnDisable()
        {
            if (EnemyMoveRoute.Instance != null)
            {
                EnemyMoveRoute.Instance.OnGenerateRoute -= OnGenerateRoute;
            }

            // 코루틴 중지
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // StageSystem에 적 제거 알림 (OnDisable에서 처리되지 않은 경우)
            if (StageSystem.Instance != null)
            {
                StageSystem.Instance.OnEnemyDestroyed(this);
            }
        }

        void OnGenerateRoute(Vector3[] route)
        {
            if (route == null || route.Length == 0)
            {
                Debug.LogWarning("Enemy: 경로가 비어있습니다.");
                return;
            }

            // 이전 이동 코루틴이 실행 중이면 중지
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = StartCoroutine(MoveAlongRoute(EnemyMoveRoute.Instance.WorldRoute));
        }

        private IEnumerator MoveAlongRoute(Vector3[] route)
        {
            if (route == null || route.Length == 0)
                yield break;

            // 현재 enemy 위치와 가장 가까운 route의 요소를 찾아 인덱스를 그 요소에 맞춤
            int closestIndex = 0;
            float minDistance = float.MaxValue;
            Vector3 currentPos = transform.position;

            for (int i = 0; i < route.Length; i++)
            {
                float dist = Vector3.Distance(currentPos, route[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestIndex = i;
                }
            }
            // 이동 시작 인덱스를 closestIndex로 맞춤
            transform.position = route[closestIndex];

            // 경로를 따라 순차적으로 이동
            for (int i = closestIndex + 1; i < route.Length; i++)
            {
                Vector3 startPos = route[i - 1];
                Vector3 endPos = route[i];
                float distance = Vector3.Distance(startPos, endPos);

                if (distance > 0.01f) // 거리가 충분히 클 때만 이동
                {
                    float travelTime = distance / moveSpeed;
                    float elapsedTime = 0f;

                    while (elapsedTime < travelTime)
                    {
                        elapsedTime += Time.deltaTime;
                        float t = Mathf.Clamp01(elapsedTime / travelTime);
                        transform.position = Vector3.Lerp(startPos, endPos, t);
                        yield return null;
                    }
                }

                // 정확한 위치로 설정
                transform.position = endPos;
            }

            // 경로가 원형이면 처음으로 돌아가기
            if (route.Length > 1)
            {
                Vector3 lastPos = route[route.Length - 1];
                Vector3 firstPos = route[0];
                float distance = Vector3.Distance(lastPos, firstPos);

                if (distance > 0.01f)
                {
                    float travelTime = distance / moveSpeed;
                    float elapsedTime = 0f;

                    while (elapsedTime < travelTime)
                    {
                        elapsedTime += Time.deltaTime;
                        float t = Mathf.Clamp01(elapsedTime / travelTime);
                        transform.position = Vector3.Lerp(lastPos, firstPos, t);
                        yield return null;
                    }

                    transform.position = firstPos;
                }

                // 무한 루프로 경로를 계속 따라가기
                moveCoroutine = StartCoroutine(MoveAlongRoute(route));
            }
        }

        private void UpdateHealthBar()
        {
            float hpRatio = currentHealth / maxHealth;
            healthBarFill.offsetMax = new Vector2(-canvasWidth * (1 - hpRatio), healthBarFill.offsetMax.y);
        }
    }
}
