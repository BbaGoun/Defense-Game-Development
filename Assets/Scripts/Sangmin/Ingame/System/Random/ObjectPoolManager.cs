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
        public GameObject prefab;
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

        // 생성된 오브젝트가 어떤 프리팹 정보로부터 비롯되었나 저장
        private Dictionary<GameObject, ObjectInfo> goToObjectInfo = new Dictionary<GameObject, ObjectInfo>();


        private List<PoolAble> poolAbles = new List<PoolAble>();

        private GameObject currentPrefab;

        // Hierarchy 정리를 위한 부모 오브젝트
        private Transform memoryPoolsParent;

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

            // MemoryPools 부모 오브젝트 생성 또는 찾기
            GameObject memoryPoolsObj = GameObject.Find("MemoryPools");
            if (memoryPoolsObj == null)
            {
                memoryPoolsObj = new GameObject("MemoryPools");
            }
            memoryPoolsParent = memoryPoolsObj.transform;

            foreach (var objInfo in objectInfos)
            {
                currentPrefab = objInfo.prefab;

                IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, objInfo.count, int.MaxValue);

                if (objectPoolDic.ContainsKey(objInfo.prefab))
                {
                    Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objInfo.prefab.name);
                    return;
                }

                objectPoolDic.Add(objInfo.prefab, pool);

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

            //Debug.Log("오브젝트풀링 준비 완료");
            IsReady = true;
        }

        public void RecallAll()
        {
            foreach (var poolAble in poolAbles)
            {
                if (poolAble != null && poolAble.gameObject.activeSelf)
                    poolAble.ReleaseObject();
            }
        }

        /// <summary>
        /// poolAbles 리스트에서 PoolAble을 제거합니다.
        /// DestroyObject() 호출 시 사용됩니다.
        /// </summary>
        public void PreparationRemove(PoolAble poolAble)
        {
            if (poolAble != null && poolAbles.Contains(poolAble))
            {
                poolAbles.Remove(poolAble);
            }
            if (goToObjectInfo.ContainsKey(poolAble.gameObject))
            {
                goToObjectInfo.Remove(poolAble.gameObject);
            }
        }

        // 생성
        private GameObject CreatePooledItem()
        {
            GameObject pooledObject = Instantiate(currentPrefab, memoryPoolsParent);
            pooledObject.GetComponent<PoolAble>().pool = objectPoolDic[currentPrefab];
            foreach (var objInfo in objectInfos)
            {
                if (currentPrefab == objInfo.prefab)
                    goToObjectInfo.Add(pooledObject, objInfo);
            }
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

            IObjectPool<GameObject> pool = objectPoolDic[_prefab];


            // Unity의 ObjectPool.Get()은 풀이 비어있을 때 자동으로 CreatePooledItem을 호출하여 새 오브젝트 생성
            GameObject obj = pool.Get();

            // 새로 생성된 오브젝트를 poolAbles 리스트에 추가 (처음 생성된 경우)
            if (obj != null)
            {
                PoolAble poolAble = obj.GetComponent<PoolAble>();
                if (poolAble != null && !poolAbles.Contains(poolAble))
                {
                    poolAbles.Add(poolAble);
                }
            }

            return obj;
        }

        public int GetInitialCountOfPrefab(GameObject gameObject)
        {
            if (goToObjectInfo.ContainsKey(gameObject))
                return goToObjectInfo[gameObject].count;
            else
            {
                Debug.Log($"GameObject -> ObjectInfo 기록 안 됨 {gameObject.name}");
                return int.MaxValue;
            }
        }
    }
}