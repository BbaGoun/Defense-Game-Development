using UnityEngine;
using System.Collections.Generic;

namespace Sangmin
{
    public class AreaAttackBehaviour : IAttackBehaviour
    {
        private float radius;

        public AreaAttackBehaviour(float radius)
        {
            this.radius = radius;
        }

        public void Attack(Unit self, Unit mainTarget)
        {
            // 메인 타겟 정하기
            // 메인 타겟 주변 적 찾기
            // 데미지 주기
        }

        private void DealDamage(Unit self, Unit target)
        {
        }

        private List<Unit> FindEnemiesAround(Vector3 pos, float radius)
        {
            return null;
        }
    }
}