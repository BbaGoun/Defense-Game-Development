using UnityEngine;

namespace Sangmin
{
    [CreateAssetMenu(fileName = "SingleAttack", menuName = "Scriptable Objects/SingleAttack")]
    public class SingleAttackBehaviour : AttackBehaviour
    {
        public override void Initialize(Unit self)
        {
            base.Initialize(self);
        }

        override public void Attack(Unit self, Transform startPos, Enemy mainTarget)
        {
            // 1) 메인 타깃에게 데미지
            DealDamage(self, mainTarget);
        }

        private void DealDamage(Unit self, Enemy target)
        {
            if (target == null) return;

            float dmg = self.finalAttackDamage;
            // 여기서 데마시아 시너지 등으로 최종 수치가 이미 반영됐다고 가정
            target.TakeDamage(dmg);
            Debug.Log($"[{self.name}]가 [{target.name}]에게 {dmg} 데미지");
        }
    }
}