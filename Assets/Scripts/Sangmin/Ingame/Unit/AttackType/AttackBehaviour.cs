using Sangmin;
using UnityEngine;

namespace Sangmin
{
    public class AttackBehaviour : ScriptableObject, IAttackBehaviour
    {
        virtual public void Attack(Unit self, Transform startPos, Enemy target)
        {
            throw new System.NotImplementedException();
        }

        virtual public void Initialize(Unit self)
        {
            throw new System.NotImplementedException();
        }
    }
}