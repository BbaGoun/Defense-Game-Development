using UnityEngine;

public class ProfileEditUI : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private UserVisualDatabase visualDB;

    [Header("Icon Panel")]
    [SerializeField] private Transform iconContent;
    [SerializeField] private UserIconSelectButton iconButtonPrefab;

    [Header("Frame Panel")]
    [SerializeField] private Transform frameContent;
    [SerializeField] private UserFrameSelectButton frameButtonPrefab;

    void Start()
    {
        CreateIconButtons();
        CreateFrameButtons();
    }

    void CreateIconButtons()
    {
        foreach (var entry in visualDB.icons)
        {
            var btn = Instantiate(iconButtonPrefab, iconContent);
            btn.Setup(entry);
        }
    }

    void CreateFrameButtons()
    {
        foreach (var entry in visualDB.frames)
        {
            var btn = Instantiate(frameButtonPrefab, frameContent);
            btn.Setup(entry);
        }
    }
}
