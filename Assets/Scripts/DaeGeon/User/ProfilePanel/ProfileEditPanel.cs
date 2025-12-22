using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileEditPanel : BaseUserVisualUI
{
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;
    [SerializeField] private TMP_Text nameText;

    protected override void Refresh()
    {
        var user = UserManager.Instance.Data;

        icon.sprite = UserManager.Instance.GetCurrentIcon().sprite;
        frame.sprite = UserManager.Instance.GetCurrentFrame().sprite;
        nameText.text = user.userName;
    }
}
