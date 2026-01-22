using Sangmin;
using UnityEngine;

namespace Sangmin
{
    public class BuffBehaviour : ScriptableObject, IBuffBehaviour
    {
        virtual public void Buff(Unit self, Unit target)
        {
            throw new System.NotImplementedException();
        }

        virtual public void Initialize(Unit self)
        {
            throw new System.NotImplementedException();
        }
    }
}