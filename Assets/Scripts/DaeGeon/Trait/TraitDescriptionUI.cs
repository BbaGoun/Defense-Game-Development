using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TraitDescriptionUI : MonoBehaviour
{
    public static TraitDescriptionUI Instance;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button confirmButton;

    private TraitNode selectedNode;

    private void Awake() => Instance = this;

    public void OpenDescription(TraitData data, TraitNode node, bool isUnlocked)
    {
        selectedNode = node;
        titleText.text = data.traitName;
        descText.text = data.description;
        costText.text = $"소모 포인트: {data.unlockCost}";

        confirmButton.interactable = !isUnlocked;
        confirmButton.GetComponentInChildren<TMP_Text>().text = isUnlocked ? "해금 완료" : "해금하기";
    }

    public void OnClickConfirm()
    {
        if (selectedNode != null) selectedNode.ConfirmUnlock();
    }
}