using UnityEngine;

namespace Sangmin
{
    public class DemaciaSynergy : ISynergy
    {
        private int alliesRequired;
        private float bonusDamagePercent;

        public DemaciaSynergy(int alliesRequired, float bonusDamagePercent)
        {
            this.alliesRequired = alliesRequired;
            this.bonusDamagePercent = bonusDamagePercent;
        }

        public void OnCombatStart(Unit self)
        {
            int demaciaAllies = CountDemaciaAllies(self);

            if (demaciaAllies >= alliesRequired)
            {
                float bonus = self.baseAttackDamage * bonusDamagePercent;
                self.finalAttackDamage = self.baseAttackDamage + bonus;
                Debug.Log($"[데마시아 시너지 발동] {self.name} 공격력 +{bonus}");
            }
            else
            {
                self.finalAttackDamage = self.baseAttackDamage;
            }
        }

        private int CountDemaciaAllies(Unit self)
        {
            // 실제 구현엔 팀/필드 정보가 필요
            return 2; // 예시
        }
    }
}