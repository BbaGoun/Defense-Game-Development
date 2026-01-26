using System;

namespace Sangmin
{
    /// <summary>
    /// 쥬얼 획득 조건(규칙)들이 구독하는 이벤트 버스.
    /// - 각 시스템(뽑기/유닛배치/시너지/보스 처치)에서 "사실"만 발행하고
    /// - 쥬얼 보상 로직은 규칙(ScriptableObject)에서 처리한다.
    /// </summary>
    public static class JewelEventBus
    {
        /// <summary>
        /// 일반 뽑기(골드 소모)로 유닛을 "성공적으로 배치"했을 때 호출.
        /// </summary>
        public static event Action<Unit> OnNormalSummonPlaced;

        /// <summary>
        /// 어떤 방식이든 유닛이 "성공적으로 배치"되었을 때 호출.
        /// </summary>
        public static event Action<Unit> OnAnyUnitPlaced;

        /// <summary>
        /// 현재 필드에서 "연결 컴포넌트 최대 길이(=시너지 최대 길이)"가 갱신될 때 호출.
        /// </summary>
        public static event Action<int> OnSynergyMaxLengthUpdated;

        /// <summary>
        /// 보스가 처치되었을 때 호출. (wave: 보스가 생성된 웨이브)
        /// </summary>
        public static event Action<int> OnBossKilled;

        public static void RaiseNormalSummonPlaced(Unit unit)
        {
            if (unit == null) return;
            OnNormalSummonPlaced?.Invoke(unit);
        }

        public static void RaiseAnyUnitPlaced(Unit unit)
        {
            if (unit == null) return;
            OnAnyUnitPlaced?.Invoke(unit);
        }

        public static void RaiseSynergyMaxLengthUpdated(int maxLength)
        {
            if (maxLength <= 0) return;
            OnSynergyMaxLengthUpdated?.Invoke(maxLength);
        }

        public static void RaiseBossKilled(int wave)
        {
            if (wave <= 0) return;
            OnBossKilled?.Invoke(wave);
        }
    }
}

