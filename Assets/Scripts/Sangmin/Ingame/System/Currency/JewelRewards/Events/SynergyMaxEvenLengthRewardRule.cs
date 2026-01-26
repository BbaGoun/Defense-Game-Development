using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 시너지 최대 길이 최초 달성 보상
    /// - 길이 4 이상의 "짝수"를 최초 달성할 때마다 지급
    /// - 보상 = 길이 * 6
    /// </summary>
    [CreateAssetMenu(menuName = "Sangmin/Jewel Rewards/Rule - Synergy Max Even Length", fileName = "JewelRule_SynergyMaxEvenLength")]
    public class SynergyMaxEvenLengthRewardRule : JewelRewardRule
    {
        [Header("Rule Params")]
        public int minEvenLength = 4;
        public int rewardMultiplier = 6;

        private JewelRewardContext _ctx;

        public override void Install(JewelRewardContext context)
        {
            _ctx = context;
            JewelEventBus.OnSynergyMaxLengthUpdated += HandleSynergyMaxLengthUpdated;
        }

        public override void Uninstall(JewelRewardContext context)
        {
            JewelEventBus.OnSynergyMaxLengthUpdated -= HandleSynergyMaxLengthUpdated;
            _ctx = null;
        }

        private void HandleSynergyMaxLengthUpdated(int maxLength)
        {
            if (_ctx == null) return;
            if (maxLength < minEvenLength) return;

            // 지금까지 달성한 최대 길이를 기억해두면, 중복 체크 비용이 줄어든다.
            int recordedMax = _ctx.GetInt(ruleId, "recordedMax", 0);
            if (maxLength <= recordedMax) return;

            // recordedMax+1 ~ maxLength 사이에서 "짝수 >= minEvenLength"를 처음 달성한 것들을 모두 지급
            for (int len = recordedMax + 1; len <= maxLength; len++)
            {
                if (len < minEvenLength) continue;
                if (len % 2 != 0) continue;

                string flagKey = $"reached_{len}";
                if (_ctx.GetBool(ruleId, flagKey, false)) continue;

                _ctx.SetBool(ruleId, flagKey, true);

                int reward = Mathf.Max(0, len * rewardMultiplier);
                if (reward > 0)
                {
                    _ctx.AddJewel(reward, $"시너지 길이 {len} 최초 달성");
                }
            }

            _ctx.SetInt(ruleId, "recordedMax", maxLength);
        }
    }
}

