using System;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    [SerializeField] private UserVisualDatabase visualDB;

    public UserData Data { get; private set; }
    
    // 실제 데이터가 적용/저장되었을 때 (프로필바 등 메인 UI 갱신용)
    public event Action OnUserDataChanged;
    
    // 리스트에서 클릭만 해서 선택이 바뀌었을 때 (미리보기 패널 갱신용)
    public event Action OnSelectionChanged;

    // 임시 저장용 ID (체크 표시 및 미리보기용)
    public string SelectedIconId { get; private set; }
    public string SelectedFrameId { get; private set; }

    // 미리보기 패널에 띄워줄 마지막 선택 아이템의 설명
    public string LastSelectedDesc { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitDummy();
    }

    void InitDummy()
    {
        // 더미 데이터 초기화
        Data = new UserData { userName = "Player", iconId = "default_icon", frameId = "default_frame" };
        
        // 초기 임시 선택값 설정
        SelectedIconId = Data.iconId;
        SelectedFrameId = Data.frameId;

        // 초기 설명 설정 (현재 장착 중인 아이콘의 설명으로 시작)
        var entry = visualDB.GetIconEntry(Data.iconId);
        if (entry != null) LastSelectedDesc = entry.acquireDesc;
    }

    public void SetTempIcon(string id)
    {
        SelectedIconId = id;

        var entry = visualDB.GetIconEntry(id);
        if (entry != null)
            LastSelectedDesc = entry.acquireDesc;

        OnSelectionChanged?.Invoke();
    }

    public void SetTempFrame(string id)
    {
        SelectedFrameId = id;

        var entry = visualDB.GetFrameEntry(id);
        if (entry != null)
            LastSelectedDesc = entry.acquireDesc;

        OnSelectionChanged?.Invoke();
    }

    // [적용] 버튼을 눌렀을 때 호출 (임시 선택을 실제 데이터로 확정)
    public void ApplyChanges()
    {
        Data.iconId = SelectedIconId;
        Data.frameId = SelectedFrameId;
        
        Debug.Log($"데이터 적용됨: Icon={Data.iconId}, Frame={Data.frameId}");
        
        // 실제 데이터가 바뀌었음을 알림
        OnUserDataChanged?.Invoke(); 
    }

    // 미리보기 패널이 참조할 '임시 선택' 데이터 가져오기
    public UserIconEntry GetSelectedIcon() => visualDB.GetIconEntry(SelectedIconId);
    public UserFrameEntry GetSelectedFrame() => visualDB.GetFrameEntry(SelectedFrameId);
    
    // 프로필 바 등이 참조할 '실제 적용된' 데이터 가져오기
    public UserIconEntry GetCurrentIcon() => visualDB.GetIconEntry(Data.iconId);
    public UserFrameEntry GetCurrentFrame() => visualDB.GetFrameEntry(Data.frameId);
}