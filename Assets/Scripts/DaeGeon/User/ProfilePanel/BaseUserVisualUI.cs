using UnityEngine;
using System.Collections;

public abstract class BaseUserVisualUI : MonoBehaviour
{
    // 임시 선택 이벤트를 사용할지 여부 (기본 false)
    protected virtual bool UseSelectionEvent => false;

    protected virtual void OnEnable()
    {
        if (UserManager.Instance != null)
        {
            Subscribe();
        }
        else
        {
            StartCoroutine(WaitAndSubscribe());
        }
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (UserManager.Instance == null)
            yield return null;

        Subscribe();
        Debug.Log($"[{gameObject.name}] 지연 구독 성공!");
    }

    private void Subscribe()
    {
        var um = UserManager.Instance;

        // 실제 데이터 변경
        um.OnUserDataChanged -= Refresh;
        um.OnUserDataChanged += Refresh;

        // 임시 선택 변경 (필요한 UI만)
        if (UseSelectionEvent)
        {
            um.OnSelectionChanged -= Refresh;
            um.OnSelectionChanged += Refresh;
        }

        Refresh();
    }

    protected virtual void OnDisable()
    {
        if (UserManager.Instance == null) return;

        var um = UserManager.Instance;

        um.OnUserDataChanged -= Refresh;

        if (UseSelectionEvent)
            um.OnSelectionChanged -= Refresh;
    }

    protected abstract void Refresh();
}
