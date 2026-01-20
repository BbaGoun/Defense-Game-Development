using UnityEngine;

namespace Sangmin
{
    [RequireComponent(typeof(PoolAble))]
    public class SingleProjectile : MonoBehaviour
    {
        private Unit owner;
        private Enemy target;
        private float damage;
        private float speed;
        //private float hitRadius;
        private float maxLifetime;
        private float lifeTimer;
        private PoolAble poolAble;

        private void Awake()
        {
            poolAble = GetComponent<PoolAble>();
        }

        public void Launch(Unit owner, Enemy target, float damage, float speed, float maxLifetime)
        {
            this.owner = owner;
            this.target = target;
            this.damage = damage;
            this.speed = speed;
            //this.hitRadius = hitRadius;
            this.maxLifetime = maxLifetime;
            lifeTimer = 0f;
        }

        private void Update()
        {
            // 목표가 사라졌다면 투사체만 제거
            if (target == null)
            {
                Release();
                return;
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

            // 회전 방향을 맞추고 싶다면 Z축 기준 회전 추가
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            // if (Vector3.SqrMagnitude(targetPos - transform.position) <= hitRadius * hitRadius)
            // {
            //     HitTarget();
            // }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.Equals(target.gameObject))
                HitTarget();
        }

        private void HitTarget()
        {
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            Release();
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