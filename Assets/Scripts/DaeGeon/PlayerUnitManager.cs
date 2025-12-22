using System;
using UnityEngine;

/// <summary>
/// 플레이어 유닛 정보를 중앙에서 노출하는 싱글턴 매니저입니다.
/// 다른 시스템에서 `PlayerUnitManager.Instance.GetStatus()`처럼 접근하세요.
/// </summary>
public class PlayerUnitManager : MonoBehaviour
{
    public static PlayerUnitManager Instance;

    [Header("Player References")]
    public GameObject playerObject;
    public Player player;
    public PlayerStatus status;

    public event Action OnPlayerRegistered;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // 자동으로 씬에 Player가 있으면 등록 시도
        if (playerObject == null)
        {
            var p = FindAnyObjectByType<Player>();
            if (p != null) RegisterPlayer(p.gameObject);
        }
    }

    /// <summary>
    /// 씬에서 Player 오브젝트가 생성되거나 바뀔 때 호출하세요.
    /// </summary>
    public void RegisterPlayer(GameObject playerGo)
    {
        playerObject = playerGo;
        player = playerGo != null ? playerGo.GetComponent<Player>() : null;
        status = playerGo != null ? playerGo.GetComponent<PlayerStatus>() : null;
        OnPlayerRegistered?.Invoke();
    }

    public void UnregisterPlayer()
    {
        playerObject = null;
        player = null;
        status = null;
    }

    public Player GetPlayer() => player;
    public PlayerStatus GetStatus() => status;
    public GameObject GetPlayerObject() => playerObject;
    public bool HasPlayer() => player != null;

    // 안전한 조회 유틸
    public bool TryGetStatus(out PlayerStatus outStatus)
    {
        outStatus = status;
        return status != null;
    }
}
