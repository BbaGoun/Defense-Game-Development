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
        if (UserManager.Instance == null || UserManager.Instance.Data == null) return;

        var user = UserManager.Instance.Data;
        var iconEntry = UserManager.Instance.GetCurrentIcon();
        var frameEntry = UserManager.Instance.GetCurrentFrame();

        // 각각의 UI 요소가 할당되어 있을 때만 실행하여 Null 에러 방지
        if (icon != null && iconEntry != null) icon.sprite = iconEntry.sprite;
        if (frame != null && frameEntry != null) frame.sprite = frameEntry.sprite;
        if (nameText != null) nameText.text = user.userName;

        // 어디가 비어있는지 알려주는 디버그 로그 (연결 안 됐을 경우에만 뜸)
        if (icon == null) Debug.LogWarning($"[{gameObject.name}] Icon Image가 연결되지 않았습니다.");
        if (frame == null) Debug.LogWarning($"[{gameObject.name}] Frame Image가 연결되지 않았습니다.");
        if (nameText == null) Debug.LogWarning($"[{gameObject.name}] Name Text가 연결되지 않았습니다.");
    }
}