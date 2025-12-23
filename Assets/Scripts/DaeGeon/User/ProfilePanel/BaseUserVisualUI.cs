using UnityEngine;
using System.Collections;

public abstract class BaseUserVisualUI : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        // Manager가 있으면 즉시 구독
        if (UserManager.Instance != null)
        {
            Subscribe();
        }
        else
        {
            // 아직 Manager가 없다면, 준비될 때까지 조금 기다렸다가 구독 시도 (코루틴)
            StartCoroutine(WaitAndSubscribe());
        }
    }

    private IEnumerator WaitAndSubscribe()
    {
        // UserManager.Instance가 생길 때까지 대기
        while (UserManager.Instance == null)
        {
            yield return null; 
        }
        
        Subscribe();
        Debug.Log($"[{gameObject.name}] 지연 구독 성공!");
    }

    private void Subscribe()
    {
        UserManager.Instance.OnUserDataChanged -= Refresh; // 중복 방지
        UserManager.Instance.OnUserDataChanged += Refresh;
        Refresh();
    }

    protected virtual void OnDisable()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnUserDataChanged -= Refresh;
    }

    protected abstract void Refresh();
}