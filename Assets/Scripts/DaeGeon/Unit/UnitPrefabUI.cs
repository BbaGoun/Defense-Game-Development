using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitPrefabUI : MonoBehaviour
{
    [Header("UI 요소")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text gradeText;
    public TMP_Text shardText;
    public Button upgradeButton;

    private UnitData data;
    private int shardCount = 0;

    // UI Prefab 초기화
    public void Setup(UnitData unitData, int initialShards = 0)
    {
        data = unitData;
        shardCount = initialShards;

        if (icon != null) icon.sprite = unitData.icon;
        if (nameText != null) nameText.text = unitData.unitName;
        if (gradeText != null) gradeText.text = unitData.grade.ToString();
        if (shardText != null) shardText.text = shardCount.ToString();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClick);
        }
    }

    // 강화 버튼 클릭
    private void OnUpgradeClick()
    {
        if (shardCount >= data.shardsRequiredPerUpgrade)
        {
            shardCount -= data.shardsRequiredPerUpgrade;
            shardText.text = shardCount.ToString();
        }
    }

    // 외부에서 조각 추가 가능
    public void AddShards(int amount)
    {
        shardCount += amount;
        if (shardText != null) shardText.text = shardCount.ToString();
    }
}
