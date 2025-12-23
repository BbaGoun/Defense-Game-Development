using System;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    [SerializeField] private UserVisualDatabase visualDB;

    public UserData Data { get; private set; }
    
    // 실제 데이터가 변경되었을 때 (프로필바, 정보패널 갱신용)
    public event Action OnUserDataChanged;
    
    // 리스트에서 선택만 바뀌었을 때 (체크 표시 갱신용)
    public event Action OnSelectionChanged;

    // 임시 저장용 ID
    public string SelectedIconId { get; private set; }
    public string SelectedFrameId { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitDummy();
    }

    void InitDummy()
    {
        Data = new UserData { userName = "Player", iconId = "default_icon", frameId = "default_frame" };
        SelectedIconId = Data.iconId;
        SelectedFrameId = Data.frameId;
    }

    // 리스트에서 클릭했을 때 호출 (임시 선택)
    public void SetTempIcon(string id)
    {
        SelectedIconId = id;
        OnSelectionChanged?.Invoke(); 
    }

    public void SetTempFrame(string id)
    {
        SelectedFrameId = id;
        OnSelectionChanged?.Invoke();
    }

    // [적용] 버튼을 눌렀을 때 호출
    public void ApplyChanges()
    {
        Data.iconId = SelectedIconId;
        Data.frameId = SelectedFrameId;
        
        Debug.Log("데이터 적용됨: " + Data.iconId);
        OnUserDataChanged?.Invoke(); 
    }

    public UserIconEntry GetCurrentIcon() => visualDB.GetIconEntry(Data.iconId);
    public UserFrameEntry GetCurrentFrame() => visualDB.GetFrameEntry(Data.frameId);
}