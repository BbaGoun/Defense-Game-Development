using Sangmin;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    void Start()
    {
        StageSystem.Instance.Init();

        // 플레이어가 갖고있는 유닛 목록도 가져오기

        // prefab 추가가 끝난 후 ObjectPool 생성
        ObjectPoolManager.Instance.Init();

        // ObjectPool 등록 후 사이클 실행
        StageSystem.Instance.StartCode();
    }
}
