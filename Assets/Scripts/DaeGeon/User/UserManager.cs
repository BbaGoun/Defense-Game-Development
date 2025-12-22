using System;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    [SerializeField] private UserVisualDatabase visualDB;

    public UserData Data { get; private set; }
    public event Action OnUserDataChanged;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitDummy();
    }

    void InitDummy()
    {
        Data = new UserData
        {
            userName = "Player",
            iconId = "default_icon",
            frameId = "default_frame",
            level = 1,
            exp = 0
        };
    }

public void ChangeIcon(string id)
{
    Debug.Log($"아이콘 변경 시도: {id}"); // 로그 추가
    if (Data.iconId == id) return;
    Data.iconId = id;
    OnUserDataChanged?.Invoke();
    Debug.Log("이벤트 발생 완료"); 
}

    public void ChangeFrame(string id)
    {
        if (Data.frameId == id) return;
        Data.frameId = id;
        OnUserDataChanged?.Invoke();
    }

    public UserIconEntry GetCurrentIcon()
        => visualDB.GetIconEntry(Data.iconId);

    public UserFrameEntry GetCurrentFrame()
        => visualDB.GetFrameEntry(Data.frameId);
}
