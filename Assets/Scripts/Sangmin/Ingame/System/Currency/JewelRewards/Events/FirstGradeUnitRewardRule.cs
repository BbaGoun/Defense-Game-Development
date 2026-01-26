using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 최초 UNIQUE/LEGEND/MYTHIC 유닛 생성(=배치 성공) 시 쥬얼 지급
    /// - UNIQUE: 10
    /// - LEGEND: 25
    /// - MYTHIC: 50
    /// </summary>
    [CreateAssetMenu(menuName = "Sangmin/Jewel Rewards/Rule - First Grade Unit", fileName = "JewelRule_FirstGradeUnit")]
    public class FirstGradeUnitRewardRule : JewelRewardRule
    {
        [Header("Rewards")]
        public int uniqueReward = 10;
        public int legendReward = 25;
        public int mythicReward = 50;

        private JewelRewardContext _ctx;

        public override void Install(JewelRewardContext context)
        {
            _ctx = context;
            JewelEventBus.OnAnyUnitPlaced += HandleAnyUnitPlaced;
        }

        public override void Uninstall(JewelRewardContext context)
        {
            JewelEventBus.OnAnyUnitPlaced -= HandleAnyUnitPlaced;
            _ctx = null;
        }

        private void HandleAnyUnitPlaced(Unit unit)
        {
            if (_ctx == null) return;
            if (unit == null || unit.unitData == null) return;

            switch (unit.unitData.grade)
            {
                case Grade.UNIQUE:
                    TryGrantOnce("unique", uniqueReward, "최초 유니크 유닛 생성");
                    break;
                case Grade.LEGEND:
                    TryGrantOnce("legend", legendReward, "최초 전설 유닛 생성");
                    break;
                case Grade.MYTHIC:
                    TryGrantOnce("mythic", mythicReward, "최초 신화 유닛 생성");
                    break;
            }
        }

        private void TryGrantOnce(string keySuffix, int amount, string reason)
        {
            if (amount <= 0) return;

            string flagKey = $"first_{keySuffix}";
            if (_ctx.GetBool(ruleId, flagKey, false)) return;

            _ctx.SetBool(ruleId, flagKey, true);
            _ctx.AddJewel(amount, reason);
        }
    }
}

