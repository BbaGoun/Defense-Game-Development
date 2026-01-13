using UnityEngine;
using System.Collections.Generic;

namespace Sangmin
{
    [CreateAssetMenu(fileName = "AreaAttack", menuName = "Scriptable Objects/AreaAttack")]
    public class AreaAttackBehaviour : AttackBehaviour
    {
        public float radius;
        public GameObject attackEffect;
        private float realRadius;

        override public void Initialize(Unit self)
        {
            this.realRadius = self.rangeMultiplier * self.transform.localScale.x * radius;
        }

        override public void Attack(Unit self, Enemy mainTarget)
        {
            if (mainTarget == null) return;

            // 메인 타겟에게 데미지
            DealDamage(self, mainTarget);

            GameObject attackEffect = ObjectPoolManager.Instance.GetObject(this.attackEffect);
            attackEffect.transform.position = mainTarget.transform.position;
            var spriteRenderer = attackEffect.GetComponent<SpriteRenderer>();
            spriteRenderer.size = new Vector2(realRadius * 2, realRadius * 2);
            var poolAble = attackEffect.GetComponent<PoolAble>();
            poolAble.ReleaseObjectWithDelay(0.25f);

            // 메인 타겟 주변 적 찾기
            // enemy의 sprite 크기까지 고려하고 싶음 (0.4 scale * 1.3 size / 2)
            List<Enemy> nearbyEnemies = FindEnemiesAround(mainTarget.transform.position, realRadius + 0.26f);

            // 주변 적들에게도 데미지
            foreach (Enemy enemy in nearbyEnemies)
            {
                if (enemy != null && enemy != mainTarget)
                {
                    DealDamage(self, enemy);
                }
            }
        }

        private void DealDamage(Unit self, Enemy target)
        {
            if (target == null) return;

            float dmg = self.finalAttackDamage;
            target.TakeDamage(dmg);
            Debug.Log($"[{self.name}]가 [{target.name}]에게 {dmg} 광역 피해");
        }

        private List<Enemy> FindEnemiesAround(Vector3 pos, float radius)
        {
            List<Enemy> enemies = new List<Enemy>();

            if (StageSystem.Instance == null) return enemies;

            List<Enemy> activeEnemies = StageSystem.Instance.GetActiveEnemies();
            foreach (Enemy enemy in activeEnemies)
            {
                if (enemy == null) continue;

                float distance = Vector2.Distance(pos, enemy.transform.position);
                Debug.Log($"{enemy.name}과의 거리 : {distance}");
                if (distance <= radius)
                {
                    enemies.Add(enemy);
                }
            }

            return enemies;
        }
    }
}