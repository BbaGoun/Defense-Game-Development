using UnityEngine;

public abstract class BaseUserVisualUI : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        if (UserManager.Instance != null)
        {
            UserManager.Instance.OnUserDataChanged += Refresh;
            Refresh();
        }
    }

    protected virtual void OnDisable()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnUserDataChanged -= Refresh;
    }

    protected abstract void Refresh();
}
