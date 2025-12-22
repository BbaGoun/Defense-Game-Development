using UnityEngine;
using UnityEngine.UI;

public class UserInfoBar : BaseUserVisualUI
{
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;

    protected override void Refresh()
    {
        var iconEntry = UserManager.Instance.GetCurrentIcon();
        var frameEntry = UserManager.Instance.GetCurrentFrame();

        icon.sprite = iconEntry.sprite;
        frame.sprite = frameEntry.sprite;
    }
}
