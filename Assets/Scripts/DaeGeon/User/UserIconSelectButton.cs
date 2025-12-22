using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserIconSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private Button button;

    private string iconId;

    public void Setup(UserIconEntry entry)
    {
        iconId = entry.id;
        iconImage.sprite = entry.sprite;
        nameText.text = entry.displayName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        RefreshSelected();
    }

    void OnClick()
    {
        UserManager.Instance.ChangeIcon(iconId);
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
        bool isSelected = UserManager.Instance.Data.iconId == iconId;
        selectedMark.SetActive(isSelected);
    }
}
