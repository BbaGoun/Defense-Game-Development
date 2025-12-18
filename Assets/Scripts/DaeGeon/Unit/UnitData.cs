using UnityEngine;

public enum UnitGrade
{
    NORMAL,
    RARE,
    UNIQUE,
    LEGEND,
    MYTHIC
}

public enum UnitType
{
    Melee,
    Ranged,
    Magic
}

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public GameObject prefab;  //해당 구조
    
    [Header("Identity")]
    public int unitId;
    public string unitName;

    [Header("Visual")]
    public Sprite icon;
    public Sprite gradeBackground;

    [Header("Classification")]
    public UnitGrade grade;
    public UnitType type;
    public string race;      // 종족
    public string faction;   // 진영

    [Header("Description")]
    [TextArea]
    public string description;

    [Header("Base Stats")]
    public float baseAttack;
    public float baseAttackSpeed;
    public float baseRange;

    [Header("Upgrade")]
    public int shardsRequiredPerUpgrade = 10;
    public float upgradeMultiplier = 1.2f;
}
