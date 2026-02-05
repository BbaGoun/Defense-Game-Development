using UnityEngine;
using UnityEngine.Events;

namespace Sangmin
{
    public class AnimationEventForwarder : MonoBehaviour
    {
        public UnityEvent attackEvent;

        public void AttackEventForward()
        {
            attackEvent?.Invoke();
        }
    }
}