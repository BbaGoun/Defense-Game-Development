using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 일반 뽑기 누적 30회마다 쥬얼 지급
    /// - 최초: 8개
    /// - 이후: 4개씩 증가 (30회=8, 60회=12, 90회=16, ...)
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Jewel Rewards/Rule - Normal Summon Milestone", fileName = "JewelRule_NormalSummonMilestone")]
    public class NormalSummonMilestoneRule : JewelRewardRule
    {
        [Header("Rule Params")]
        public int milestone = 30;
        public int firstReward = 8;
        public int rewardIncreasePerMilestone = 4;

        private JewelRewardContext _ctx;

        public override void Install(JewelRewardContext context)
        {
            _ctx = context;
            JewelEventBus.OnNormalSummonPlaced += HandleNormalSummonPlaced;
        }

        public override void Uninstall(JewelRewardContext context)
        {
            JewelEventBus.OnNormalSummonPlaced -= HandleNormalSummonPlaced;
            _ctx = null;
        }

        private void HandleNormalSummonPlaced(Unit unit)
        {
            if (_ctx == null) return;
            if (milestone <= 0) return;

            int count = _ctx.GetInt(ruleId, "normalSummonCount", 0) + 1;
            _ctx.SetInt(ruleId, "normalSummonCount", count);

            // count가 milestone에 도달한 순간에만 지급
            if (count % milestone != 0) return;

            int milestoneIndex = (count / milestone) - 1; // 0-based
            int reward = firstReward + (rewardIncreasePerMilestone * milestoneIndex);
            reward = Mathf.Max(0, reward);

            if (reward > 0)
            {
                _ctx.AddJewel(reward, $"일반 뽑기 {count}회 달성");
            }
        }
    }
}

