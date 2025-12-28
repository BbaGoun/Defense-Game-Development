using UnityEngine;
using UnityEngine.AI;

namespace Sangmin
{
    public class Enemy : MonoBehaviour
    {
        [Header("Waypoints, 목표는 왼쪽 아래부터 반 시계 방향으로")]
        [SerializeField] private Transform[] waypoints;
        private int currentWaypointIndex = 0;

        NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        void Start()
        {
            if (waypoints[0] != null)
                agent.SetDestination(waypoints[0].position);
        }

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0 || agent == null)
                return;

            // 목적지까지 충분히 가까워졌으면 다음 지점으로
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // 다음 웨이포인트로 인덱스 이동(원형)
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                Transform nextTarget = waypoints[currentWaypointIndex];
                if (nextTarget != null)
                {
                    agent.SetDestination(nextTarget.position);
                }
            }
        }
    }
}
