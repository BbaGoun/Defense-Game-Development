using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUnitPrefab : MonoBehaviour
{
    public Image icon;
    public Image gradeBg;
    public TMP_Text nameText;
    public TMP_Text gradeText;

    public void Setup(UnitData data, UnitState state)
    {
        icon.sprite = data.icon;
        nameText.text = data.unitName;

        // gradeText에 등급 한국어 출력
        gradeText.text = GetGradeName(data.grade);

        gradeBg.sprite = GetGradeSprite(data.grade);
    }

    // 등급 → 이미지 매핑
    Sprite GetGradeSprite(UnitGrade grade)
    {
        return UnitManager.Instance.GetGradeSprite(grade);
    }

    // 등급 → 한국어 이름
    string GetGradeName(UnitGrade grade)
    {
        switch (grade)
        {
            case UnitGrade.NORMAL: return "노말";
            case UnitGrade.RARE:   return "레어";
            case UnitGrade.UNIQUE:   return "에픽";
            default: return "알수없음";
        }
    }
}
