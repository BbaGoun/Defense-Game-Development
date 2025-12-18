using UnityEngine;

namespace Sangmin
{
    public interface IAttackBehaviour
    {
        void Attack(Unit self, Unit target);
    }
}