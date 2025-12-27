using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TimeController : MonoBehaviour
    {
        public static TimeController Instance;

        [SerializeField, Range(0f, 2f)]
        private float timeScale;


        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            Time.timeScale = timeScale;
        }

        public void SetTimeScale(float _timeScale)
        {
            timeScale = _timeScale;
        }

        public float GetTimeScale()
        {
            return timeScale;
        }
    }
}