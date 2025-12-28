using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    public class TimeController : MonoBehaviour
    {
        private static TimeController _instance;
        public static TimeController Instance
        {
            get { return _instance; }
        }

        [SerializeField, Range(0f, 2f)]
        private float timeScale;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this.gameObject);
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