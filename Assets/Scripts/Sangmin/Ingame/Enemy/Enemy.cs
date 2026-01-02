using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Sangmin
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 3f; // 이동 속도

        private Coroutine moveCoroutine;


        void Awake()
        {

        }

        void Start()
        {
            if (EnemyMoveRoute.Instance != null)
            {
                EnemyMoveRoute.Instance.OnGenerateRoute += OnGenerateRoute;
            }

            transform.position = EnemyMoveRoute.Instance.startPosition;

            moveCoroutine = StartCoroutine(MoveAlongRoute(EnemyMoveRoute.Instance.WorldRoute));
        }

        void OnDestroy()
        {
            // 이벤트 구독 해제
            if (EnemyMoveRoute.Instance != null)
            {
                EnemyMoveRoute.Instance.OnGenerateRoute -= OnGenerateRoute;
            }

            // 코루틴 중지
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
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
    }
}
