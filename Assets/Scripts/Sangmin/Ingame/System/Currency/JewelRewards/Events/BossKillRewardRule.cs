using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 보스 처치 퀘스트 보상
    /// - 보스 처치 시 보스 웨이브만큼 지급 (예: 60웨이브 보스 처치 => 60 쥬얼)
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Objects/Jewel Rewards/Rule - Boss Kill", fileName = "JewelRule_BossKill")]
    public class BossKillRewardRule : JewelRewardRule
    {
        private JewelRewardContext _ctx;

        public override void Install(JewelRewardContext context)
        {
            _ctx = context;
            JewelEventBus.OnBossKilled += HandleBossKilled;
        }

        public override void Uninstall(JewelRewardContext context)
        {
            JewelEventBus.OnBossKilled -= HandleBossKilled;
            _ctx = null;
        }

        private void HandleBossKilled(int wave)
        {
            if (_ctx == null) return;
            if (wave <= 0) return;

            // 웨이브당 1회만 지급 (중복 지급 방지)
            string flagKey = $"boss_killed_{wave}";
            if (_ctx.GetBool(ruleId, flagKey, false)) return;

            _ctx.SetBool(ruleId, flagKey, true);
            _ctx.AddJewel(wave, $"보스 처치 (웨이브 {wave})");
        }
    }
}

