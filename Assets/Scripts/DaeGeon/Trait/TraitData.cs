using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(menuName = "Character/TraitData")]
public class TraitData : ScriptableObject
{
    public string traitId;        // 고유 ID
    public string traitName;      // 특성 이름
    [TextArea] public string desc; // 설명
    public Sprite icon;           // 아이콘 이미지

    // 능력치 설정
    public StatType targetStat;   // 어떤 스탯을 올릴 것인가? (Enum)
    public float value;           // 증가 수치
    
    public int maxLevel = 5;      // 최대 레벨
}

public enum StatType { Health, Attack, Defense, Speed }