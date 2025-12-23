using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserFrameSelectButton : MonoBehaviour
{
    [SerializeField] private Image frameImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private Button button;

    private string frameId;

    public void Setup(UserFrameEntry entry)
    {
        frameId = entry.id;
        frameImage.sprite = entry.sprite;
        nameText.text = entry.displayName;

        button.onClick.RemoveAllListeners();
        // ChangeFrame 대신 SetTempFrame을 호출하도록 수정되었습니다.
        button.onClick.AddListener(OnClick);

        RefreshSelected();
    }

    void OnClick()
    {
        // 수정된 부분: 즉시 데이터가 바뀌지 않고 '임시 선택' 상태가 됩니다.
        UserManager.Instance.SetTempFrame(frameId);
    }

    void OnEnable()
    {
        if (UserManager.Instance != null)
        {
            // 수정된 부분: 데이터 변경이 아닌 '선택 변경' 이벤트를 구독합니다.
            UserManager.Instance.OnSelectionChanged += RefreshSelected;
            RefreshSelected();
        }
    }

    void OnDisable()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnSelectionChanged -= RefreshSelected;
    }

    void RefreshSelected()
    {
        // 수정된 부분: 실제 저장된 데이터가 아닌 '현재 선택 중인 ID'와 비교합니다.
        bool isSelected = UserManager.Instance.SelectedFrameId == frameId;
        selectedMark.SetActive(isSelected);
    }
}