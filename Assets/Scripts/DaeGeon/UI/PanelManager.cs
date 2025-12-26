using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels;

    public int defaultIndex = 0;
    // 중앙 저장/불러오기 버튼 (Inspector에 연결)
    public Button saveButton;
    public Button loadButton;

    void Awake()
    {
        // Save/Load 버튼 연결
        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(() => { if (SaveManager.Instance != null) SaveManager.Instance.Save(); });
        }
        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => { if (SaveManager.Instance != null) SaveManager.Instance.Load(); });
        }
        ShowPanel(defaultIndex);
    }

    public void ShowPanel(int index)
    {
        if (panels == null || panels.Length == 0) return;

        // 패널 전환 시 미리보기를 초기화
        if (Player.Instance != null) Player.Instance.ClearPreview();

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
                panels[i].SetActive(i == index);
        }
    }
}
