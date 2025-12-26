using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileEditPanel : BaseUserVisualUI
{
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text acquireDescText;

    protected override bool UseSelectionEvent => true;

    protected override void Refresh()
    {
        if (UserManager.Instance == null || UserManager.Instance.Data == null)
            return;

        var um = UserManager.Instance;

        // 임시 선택된 엔트리들 가져오기
        var iconEntry = um.GetSelectedIcon();
        var frameEntry = um.GetSelectedFrame();
        var user = um.Data;

        // 1. 이미지 및 이름 갱신
        if (icon != null && iconEntry != null)
            icon.sprite = iconEntry.sprite;

        if (frame != null && frameEntry != null)
            frame.sprite = frameEntry.sprite;

        if (nameText != null)
            nameText.text = user.userName;

        // 2. 설명 갱신
        if (acquireDescText != null)
        {
            acquireDescText.text = um.LastSelectedDesc;
        }
    }
}