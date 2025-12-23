using UnityEngine;
using UnityEngine.UI;

public class ProfileEditUI : MonoBehaviour
{
    [SerializeField] private UserVisualDatabase visualDB;
    
    [Header("Icon Settings")]
    [SerializeField] private Transform iconContent;
    [SerializeField] private UserIconSelectButton iconButtonPrefab; // 아이콘용 프리팹

    [Header("Frame Settings")]
    [SerializeField] private Transform frameContent;
    [SerializeField] private UserFrameSelectButton frameButtonPrefab; // 프레임용 전용 프리팹으로 변경

    [Header("Controls")]
    [SerializeField] private Button applyButton;

    void Start()
    {
        // 1. 아이콘 버튼 생성 (UserIconSelectButton 사용)
        foreach (var entry in visualDB.icons)
        {
            var btn = Instantiate(iconButtonPrefab, iconContent);
            btn.Setup(entry);
        }

        // 2. 프레임 버튼 생성 (UserFrameSelectButton 사용)
        foreach (var entry in visualDB.frames)
        {
            var btn = Instantiate(frameButtonPrefab, frameContent);
            btn.Setup(entry); // 이제 UserFrameEntry를 받는 전용 Setup이 호출되므로 에러가 사라집니다.
        }

        // 3. 적용 버튼 연결
        if(applyButton != null)
        {
            applyButton.onClick.RemoveAllListeners();
            applyButton.onClick.AddListener(() => UserManager.Instance.ApplyChanges());
        }
    }
}