// UserIconSelectButton.cs
using UnityEngine;
using UnityEngine.UI;

public class UserIconSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private Button button;

    private string iconId;

    public void Setup(UserIconEntry entry)
    {
        iconId = entry.id;
        iconImage.sprite = entry.sprite;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => UserManager.Instance.SetTempIcon(iconId));
        RefreshSelected();
    }

    void OnEnable()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnSelectionChanged += RefreshSelected;
    }

    void OnDisable()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnSelectionChanged -= RefreshSelected;
    }

    void RefreshSelected()
    {
        selectedMark.SetActive(UserManager.Instance.SelectedIconId == iconId);
    }
}