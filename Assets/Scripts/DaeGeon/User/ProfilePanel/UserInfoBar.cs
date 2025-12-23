using UnityEngine;
using UnityEngine.UI;

public class UserInfoBar : BaseUserVisualUI
{
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;

    protected override void Refresh()
    {
        if (UserManager.Instance == null) return;

        var iconEntry = UserManager.Instance.GetCurrentIcon();
        var frameEntry = UserManager.Instance.GetCurrentFrame();

        if (icon != null && iconEntry != null) icon.sprite = iconEntry.sprite;
        if (frame != null && frameEntry != null) frame.sprite = frameEntry.sprite;
    }
}