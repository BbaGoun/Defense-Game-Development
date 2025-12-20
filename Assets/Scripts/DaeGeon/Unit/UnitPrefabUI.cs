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
    public Button infoButton;

    private UnitData data;
    private UnitState state;

    public void Setup(UnitData data, UnitState state)
    {
        this.data = data;
        this.state = state;

        icon.sprite = data.icon;
        nameText.text = data.unitName;

        // 등급 이미지 세팅
        gradeImg.sprite = GetGradeSprite(data.grade);

        Refresh();

        infoButton.onClick.RemoveAllListeners();
        infoButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUnitInfoPopup(this.data, this.state);
        });
    }

    void Refresh()
    {
        int need = data.shardsRequiredPerUpgrade;
        int current = state.shards;

        shardText.text = $"{current} / {need}";

        if (state.owned)
        {
            levelText.text = $"Lv.{state.level}";
            icon.color = Color.white;
        }
        else
        {
            levelText.text = "미보유";
            icon.color = Color.black;
        }
    }

    // 등급 → 이미지 매핑
    Sprite GetGradeSprite(UnitGrade grade)
    {
        return UnitManager.Instance.GetGradeSprite(grade);
    }
}
