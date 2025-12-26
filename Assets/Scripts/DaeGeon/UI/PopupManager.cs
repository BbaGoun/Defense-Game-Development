using UnityEngine;

public class SimpleToggle : MonoBehaviour
{
    public void Toggle(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    public void Show(GameObject panel)
    {
        if (panel != null) panel.SetActive(true);
    }

    public void Hide(GameObject panel)
    {
        if (panel != null) panel.SetActive(false);
    }
}