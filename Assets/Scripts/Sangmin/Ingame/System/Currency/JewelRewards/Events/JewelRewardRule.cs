using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 쥬얼 획득 "조건"을 확장 가능하게 만들기 위한 규칙 베이스.
    /// - CreateAssetMenu로 규칙을 추가하고, JewelRewardManager에 등록하면 동작한다.
    /// </summary>
    public abstract class JewelRewardRule : ScriptableObject
    {
        /// <summary>
        /// 규칙에서 상태 추적에 사용하는 고유 키.
        /// - 규칙마다 충돌하지 않게 반드시 고유해야 한다.
        /// </summary>
        [Tooltip("상태 추적에 사용하는 규칙 고유 키(충돌 금지)")]
        public string ruleId = "rule";

        public abstract void Install(JewelRewardContext context);
        public abstract void Uninstall(JewelRewardContext context);
    }
}

