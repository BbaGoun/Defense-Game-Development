using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitNode : MonoBehaviour
{
    [SerializeField] private TraitData traitData;
    [SerializeField] private TMP_Text levelText;
    private int currentLevel = 0;

    public void OnClickNode()
    {
        // 최대 레벨 체크
        if (currentLevel >= traitData.maxLevel) return;

        // 매니저에게 업그레이드 요청
        bool success = TraitManager.Instance.TryUpgradeTrait(traitData);

        if (success)
        {
            currentLevel++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        levelText.text = $"{currentLevel}/{traitData.maxLevel}";
    }
}