using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels;

    public int defaultIndex = 0;

    void Awake()
    {
        ShowPanel(defaultIndex);
    }

    public void ShowPanel(int index)
    {
        if (panels == null || panels.Length == 0) return;

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
                panels[i].SetActive(i == index);
        }
    }
}
