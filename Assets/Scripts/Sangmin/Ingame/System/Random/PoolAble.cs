using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Sangmin
{
    public class PoolAble : MonoBehaviour
    {
        public IObjectPool<GameObject> pool { get; set; }
        private Animator animator;

        private void Awake()
        {
            animator = gameObject.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (animator != null)
                animator.enabled = true;
        }

        private void OnDisable()
        {
            if (animator != null)
                animator.enabled = false;
        }

        public virtual void ReleaseObject()
        {
            if(gameObject.activeSelf)
                pool.Release(gameObject);
        }

        // 시간 말고 bool로 체크해서 true가 되면 release 시키는 방식으로도 지연 가능

        public virtual void ReleaseObjectWithDelay(float delay)
        {
            StartCoroutine(_ReleaseObjectWithDelay(delay));
        }

        IEnumerator _ReleaseObjectWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReleaseObject();
        }
    }
}
