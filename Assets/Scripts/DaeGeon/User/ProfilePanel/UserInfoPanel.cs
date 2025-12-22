using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInfoPanel : BaseUserVisualUI
{
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;

    protected override void Refresh()
    {
        var user = UserManager.Instance.Data;

        icon.sprite = UserManager.Instance.GetCurrentIcon().sprite;
        frame.sprite = UserManager.Instance.GetCurrentFrame().sprite;
    }
}
