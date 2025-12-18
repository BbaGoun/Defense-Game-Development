using UnityEngine;

namespace Sangmin
{
    public class SingleAttackBehaviour : IAttackBehaviour
    {
        public void Attack(Unit self, Unit mainTarget)
        {
            // 1) 메인 타깃에게 데미지
            DealDamage(self, mainTarget);
        }

        private void DealDamage(Unit self, Unit target)
        {
            float dmg = self.finalAttackDamage;
            // 여기서 데마시아 시너지 등으로 최종 수치가 이미 반영됐다고 가정
            Debug.Log($"[{self.name}]가 [{target.name}]에게 {dmg} 광역 피해");
        }
    }
}