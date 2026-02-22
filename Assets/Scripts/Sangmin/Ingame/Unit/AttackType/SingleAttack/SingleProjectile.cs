using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    [RequireComponent(typeof(PoolAble))]
    public class SingleProjectile : MonoBehaviour
    {
        private Unit owner;
        [SerializeField] private Enemy target;
        private float damage;
        private float speed;
        //private float hitRadius;
        private float maxLifetime;
        private float lifeTimer;
        private PoolAble poolAble;
        private GameObject attackEffect;

        private void Awake()
        {
            poolAble = GetComponent<PoolAble>();
        }

        public void Launch(Unit owner, Enemy target, float damage, float speed, float maxLifetime, GameObject attackEffectPrefab)
        {
            this.owner = owner;
            this.target = target;
            this.damage = damage;
            this.speed = speed;
            //this.hitRadius = hitRadius;
            this.maxLifetime = maxLifetime;
            lifeTimer = 0f;
            attackEffect = attackEffectPrefab;
        }

        private void Update()
        {
            // 목표가 사라졌다면 가장 가까운 적으로 타겟 변경
            if (target == null || target.gameObject.activeSelf == false)
            {
                // 가장 가까운 적 찾기
                Enemy nearestEnemy = FindNearestEnemy();

                // 새로운 타겟을 찾았으면 타겟 변경
                if (nearestEnemy != null)
                {
                    target = nearestEnemy;
                }
                else
                {
                    // 활성화된 적이 없을 경우에만 투사체 제거
                    Release();
                    return;
                }
            }

            lifeTimer += Time.deltaTime;
            if (lifeTimer >= maxLifetime)
            {
                Release();
                return;
            }

            Vector3 targetPos = target.transform.position;
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 회전 방향을 맞추고 싶다면 Z축 기준 회전 추가
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            // if (Vector3.SqrMagnitude(targetPos - transform.position) <= hitRadius * hitRadius)
            // {
            //     HitTarget();
            // }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (target != null && collision.gameObject.Equals(target.gameObject))
                HitTarget();
        }

        private void HitTarget()
        {
            if (target != null)
            {
                if (target.TakeDamage(damage))
                    return;
            }
            if (attackEffect != null)
            {
                var attackEffectInstance = ObjectPoolManager.Instance.GetObject(attackEffect);
                attackEffectInstance.transform.position = target.transform.position;
            }
            Release();
        }

        /// <summary>
        /// 투사체 위치에서 가장 가까운 적을 찾습니다.
        /// </summary>
        private Enemy FindNearestEnemy()
        {
            if (StageSystem.Instance == null) return null;

            List<Enemy> activeEnemies = StageSystem.Instance.GetActiveEnemies();
            if (activeEnemies == null || activeEnemies.Count == 0) return null;

            Enemy nearestEnemy = null;
            float nearestDistance = float.MaxValue;
            Vector3 projectilePosition = transform.position;

            foreach (Enemy enemy in activeEnemies)
            {
                if (enemy == null || !enemy.gameObject.activeSelf) continue;

                float distance = Vector2.Distance(projectilePosition, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = enemy;
                    nearestDistance = distance;
                }
            }

            return nearestEnemy;
        }

        private void Release()
        {
            if (poolAble != null)
            {
                poolAble.ReleaseObject();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}