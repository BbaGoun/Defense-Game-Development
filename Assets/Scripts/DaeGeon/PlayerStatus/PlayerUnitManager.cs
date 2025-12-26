using System;
using UnityEngine;

public class PlayerUnitManager : MonoBehaviour
{
    public static PlayerUnitManager Instance;

    [Header("Player References")]
    public GameObject playerObject;
    public Player player;
    
    // 이제 직접 변수를 들고 있지 않고, StatManager의 값을 실시간으로 연결합니다.
    public PlayerStatus Status => PlayerStatManager.Instance != null ? PlayerStatManager.Instance.TotalStatus : null;

    public event Action OnPlayerRegistered;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        if (playerObject == null)
        {
            var p = FindAnyObjectByType<Player>();
            if (p != null) RegisterPlayer(p.gameObject);
        }
    }

    public void RegisterPlayer(GameObject playerGo)
    {
        playerObject = playerGo;
        player = playerGo != null ? playerGo.GetComponent<Player>() : null;
        
        // 기존의 status = GetComponent<PlayerStatus>() 코드는 삭제합니다.
        // PlayerStatus가 일반 클래스이므로 GetComponent로 찾을 수 없기 때문입니다.
        
        OnPlayerRegistered?.Invoke();
    }

    public void UnregisterPlayer()
    {
        playerObject = null;
        player = null;
    }

    public Player GetPlayer() => player;
    
    // 이제 StatManager에서 계산된 최종 스탯을 반환합니다.
    public PlayerStatus GetStatus() => Status;
    
    public GameObject GetPlayerObject() => playerObject;
    public bool HasPlayer() => player != null;

    public bool TryGetStatus(out PlayerStatus outStatus)
    {
        outStatus = Status;
        return Status != null;
    }
}