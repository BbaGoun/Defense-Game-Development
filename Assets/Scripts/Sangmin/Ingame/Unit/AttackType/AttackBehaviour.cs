using Sangmin;
using UnityEngine;

namespace Sangmin
{
    public class AttackBehaviour : ScriptableObject, IAttackBehaviour
    {
        virtual public void Attack(Unit self, Enemy target)
        {
            throw new System.NotImplementedException();
        }
    }
}