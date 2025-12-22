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
        button.onClick.AddListener(OnClick);

        RefreshSelected();
    }

    void OnClick()
    {
        UserManager.Instance.ChangeFrame(frameId);
    }

    void OnEnable()
    {
        if (UserManager.Instance != null)
        {
            UserManager.Instance.OnUserDataChanged += RefreshSelected;
            RefreshSelected();
        }
    }

    void OnDisable()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnUserDataChanged -= RefreshSelected;
    }

    void RefreshSelected()
    {
        bool isSelected = UserManager.Instance.Data.frameId == frameId;
        selectedMark.SetActive(isSelected);
    }
}
