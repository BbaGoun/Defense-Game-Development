using UnityEngine;

namespace Sangmin
{
    public interface ISynergy
    {
        // 필요하면 더 쪼갤 수 있음 (OnRoundStart, OnKill 등)
        void OnCombatStart(Unit self);
    }
}