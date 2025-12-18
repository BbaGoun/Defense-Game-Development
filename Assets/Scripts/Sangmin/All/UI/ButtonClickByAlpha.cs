using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sangmin
{
    [RequireComponent(typeof(Image)), RequireComponent(typeof(Button))]
    public class ButtonClickByAlpha : MonoBehaviour
    {
        void Awake()
        {
            gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}
