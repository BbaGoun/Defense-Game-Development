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
            {
                animator.enabled = true;
                animator.Rebind();
            }
        }

        private void OnDisable()
        {
            if (animator != null)
            {
                animator.enabled = false;
                animator.Rebind();
            }
        }

        public virtual void ReleaseObject()
        {
            if (gameObject.activeSelf)
            {
                if (ObjectPoolManager.Instance.GetInitialCountOfPrefab(this.gameObject) <= pool.CountInactive)
                {
                    DestroyObject();
                }
                else
                    pool.Release(gameObject);
            }
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

        /// <summary>
        /// 풀에서 Release하지 않고 완전히 Destroy합니다.
        /// Unity의 ObjectPool은 객체가 파괴되면 자동으로 풀에서 제거됩니다.
        /// </summary>
        public virtual void DestroyObject()
        {
            // ObjectPoolManager의 poolAbles 리스트에서 제거
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.PreparationRemove(this);
            }

            // 객체 파괴 (풀은 자동으로 이를 감지하고 제거함)
            Destroy(gameObject);
        }

        /// <summary>
        /// 지연 후 Destroy합니다.
        /// </summary>
        public virtual void DestroyObjectWithDelay(float delay)
        {
            StartCoroutine(_DestroyObjectWithDelay(delay));
        }

        IEnumerator _DestroyObjectWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            DestroyObject();
        }
    }
}
