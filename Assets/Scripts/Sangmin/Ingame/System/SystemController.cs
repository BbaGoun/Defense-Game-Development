using Sangmin;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    void Start()
    {
        // 플레이어가 갖고있는 유닛 목록 prefab 세팅
        UnitHaveSystem.Instance.Init();

        // Stage에 필요한 prefab 세팅
        StageSystem.Instance.Init();

        // prefab 추가가 끝난 후 ObjectPool 생성
        ObjectPoolManager.Instance.Init();

        // ObjectPool 등록 후 사이클 실행
        StageSystem.Instance.StartCode();
    }
}
