using UnityEngine;
using System.Collections.Generic;

namespace Sangmin
{
    [CreateAssetMenu(fileName = "AreaAttack", menuName = "Scriptable Objects/AreaAttack")]
    public class AreaAttackBehaviour : AttackBehaviour
    {
        private float radius;

        public AreaAttackBehaviour(float radius)
        {
            this.radius = radius;
        }

        override public void Attack(Unit self, Enemy mainTarget)
        {
            if (mainTarget == null) return;

            // 메인 타겟에게 데미지
            DealDamage(self, mainTarget);

            // 메인 타겟 주변 적 찾기
            List<Enemy> nearbyEnemies = FindEnemiesAround(mainTarget.transform.position, radius);

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

                float distance = Vector3.Distance(pos, enemy.transform.position);
                if (distance <= radius)
                {
                    enemies.Add(enemy);
                }
            }

            return enemies;
        }
    }
}