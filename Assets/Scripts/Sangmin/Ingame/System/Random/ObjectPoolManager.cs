using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


namespace Sangmin
{
    [System.Serializable]
    public class ObjectInfo
    {
        // 오브젝트 풀에서 관리할 오브젝트
        public GameObject perfab;
        // 몇개를 미리 생성 해놓을건지
        public int count;
    }

    public class ObjectPoolManager : MonoBehaviour
    {
        private static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance
        {
            get { return _instance; }
        }

        // 오브젝트풀 매니저 준비 완료표시
        public bool IsReady { get; private set; }

        [SerializeField]
        private ObjectInfo[] objectInfos = null;

        // 오브젝트풀들을 관리할 딕셔너리
        private Dictionary<GameObject, IObjectPool<GameObject>> objectPoolDic = new Dictionary<GameObject, IObjectPool<GameObject>>();

        private List<PoolAble> poolAbles = new List<PoolAble>();

        private GameObject currentPrefab;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);

            Init();
        }

        private void Init()
        {
            IsReady = false;

            foreach (var objInfo in objectInfos)
            {
                currentPrefab = objInfo.perfab;

                IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, objInfo.count, objInfo.count);

                if (objectPoolDic.ContainsKey(objInfo.perfab))
                {
                    Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objInfo.perfab.name);
                    return;
                }

                objectPoolDic.Add(objInfo.perfab, pool);

                // 미리 오브젝트 생성하기
                for (int i = 0; i < objInfo.count; i++)
                {
                    PoolAble poolAble = CreatePooledItem().GetComponent<PoolAble>();
                    if (poolAble == null)
                    {
                        Debug.LogError($"{currentPrefab.name} Doesn't have PoolAble Script");
                        return;
                    }
                    poolAbles.Add(poolAble);
                    poolAble.pool.Release(poolAble.gameObject);
                }
            }

            Debug.Log("오브젝트풀링 준비 완료");
            IsReady = true;
        }

        public void RecallAll()
        {
            foreach (var poolAble in poolAbles)
            {
                if (poolAble.gameObject.activeSelf)
                    poolAble.ReleaseObject();
            }
        }

        // 생성
        private GameObject CreatePooledItem()
        {
            GameObject pooledObject = Instantiate(currentPrefab);
            pooledObject.GetComponent<PoolAble>().pool = objectPoolDic[currentPrefab];
            return pooledObject;
        }

        // 대여
        private void OnTakeFromPool(GameObject pooledObject)
        {
            if (pooledObject != null)
                pooledObject.SetActive(true);
            else
                Debug.Log($"Pool Get {pooledObject.name} null 오류");
        }

        // 반환
        private void OnReturnedToPool(GameObject pooledObject)
        {
            if (pooledObject != null)
                pooledObject.SetActive(false);
            else
                Debug.Log($"Pool Return {pooledObject.name} null 오류");
        }

        // 삭제
        private void OnDestroyPoolObject(GameObject pooledObject)
        {
            if (pooledObject != null)
                Destroy(pooledObject);
            else
                Debug.Log($"Pool Destroy {pooledObject.name} null 오류");
        }

        public GameObject GetObject(GameObject _prefab)
        {
            this.currentPrefab = _prefab;

            if (objectPoolDic.ContainsKey(_prefab) == false)
            {
                Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", _prefab.name);
                return null;
            }

            return objectPoolDic[_prefab].Get();
        }
    }
}