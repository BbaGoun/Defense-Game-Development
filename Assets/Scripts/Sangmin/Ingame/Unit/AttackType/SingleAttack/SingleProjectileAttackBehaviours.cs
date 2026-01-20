using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 투사체가 목표까지 이동한 뒤 접촉 시 데미지를 주는 단일 공격.
    /// </summary>
    [CreateAssetMenu(fileName = "SingleProjectileAttackBehaviours", menuName = "Scriptable Objects/SingleProjectileAttack")]
    public class SingleProjectileAttackBehaviours : AttackBehaviour
    {
        private GameObject projectilePrefab;

        public override void Initialize(Unit self)
        {
            // 프리팹을 미지정한 경우 UnitData의 attackEffect를 사용해준다.
            if (projectilePrefab == null)
            {
                projectilePrefab = self.unitData.projectilePrefab;
            }
        }

        public override void Attack(Unit self, Enemy mainTarget)
        {
            if (self == null || mainTarget == null) return;
            if (projectilePrefab == null)
            {
                Debug.LogWarning("Projectile prefab이 지정되지 않았습니다.");
                return;
            }

            GameObject projectileGo = TryGetProjectileInstance();
            if (projectileGo == null)
            {
                Debug.LogWarning("Projectile 생성에 실패했습니다.");
                return;
            }

            projectileGo.transform.position = self.transform.position;

            var projectile = projectileGo.GetComponent<SingleProjectile>();
            if (projectile == null)
            {
                projectile = projectileGo.AddComponent<SingleProjectile>();
            }

            projectile.Launch(
                owner: self,
                target: mainTarget,
                damage: self.finalAttackDamage,
                speed: self.unitData.projectileSpeed,
                maxLifetime: self.unitData.maxLifetime);
        }

        private GameObject TryGetProjectileInstance()
        {
            // ObjectPool이 준비되어 있으면 풀에서 꺼내고, 아니면 새로 생성한다.
            if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.IsReady)
            {
                return ObjectPoolManager.Instance.GetObject(projectilePrefab);
            }

            return GameObject.Instantiate(projectilePrefab);
        }
    }
}
