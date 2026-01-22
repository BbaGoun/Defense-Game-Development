using UnityEngine;

namespace Sangmin
{
    public interface IBuffBehaviour
    {
        void Initialize(Unit self);
        void Buff(Unit self, Unit target);
    }
}