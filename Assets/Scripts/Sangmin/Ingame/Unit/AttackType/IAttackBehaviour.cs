using UnityEngine;

namespace Sangmin
{
    public interface IAttackBehaviour
    {
        void Initialize(Unit self);
        void Attack(Unit self, Enemy target);
    }
}