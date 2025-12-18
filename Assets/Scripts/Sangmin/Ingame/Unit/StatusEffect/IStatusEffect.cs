using UnityEngine;

namespace Sangmin
{
    public interface IStatusEffect
    {
        void Apply(Unit self, Unit target);
    }
}