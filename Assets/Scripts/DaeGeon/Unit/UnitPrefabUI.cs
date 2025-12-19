using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitPrefabUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public Image gradeImg;
    public TMP_Text shardText;
    public TMP_Text levelText;
    public Button upgradeButton;

    private UnitData data;
    private UnitState state;

    public void Setup(UnitData data, UnitState state)
    {
        this.data = data;
        this.state = state;

        icon.sprite = this.data.icon;
        nameText.text = this.data.unitName;

        // 등급 이미지 세팅
        gradeImg.sprite = GetGradeSprite(this.data.grade);

        Refresh();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClick);
    }

    void Refresh()
    {
        shardText.text = state.shards.ToString();
        if (state.owned)
            levelText.text = $"Lv.{state.level}";
        else
            levelText.text = $"미보유";

        // 🔑 보유 + 조각 충분할 때만 강화 가능
        upgradeButton.interactable =
            state.owned && state.shards >= data.shardsRequiredPerUpgrade;
    }

    void OnUpgradeClick()
    {
        bool success = UnitManager.Instance.TryUpgrade(data.unitId);
        if (success)
            Refresh();
    }

    // 등급 → 이미지 매핑
    Sprite GetGradeSprite(UnitGrade grade)
    {
        return UnitManager.Instance.GetGradeSprite(grade);
    }
}
