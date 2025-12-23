using UnityEngine;

namespace Sangmin
{
    public interface ISynergy
    {
        int count {get; set;}
        // 필요하면 더 쪼갤 수 있음 (OnRoundStart, OnKill 등)
        void OnCombatStart(Unit self);

        void OnChangeOnce(Unit self);

        void OnAttack(Unit self);

        void OnStack(Unit self);

        void OnStackFull(Unit self);

        void OnCooldownUp(Unit self);
    }
}