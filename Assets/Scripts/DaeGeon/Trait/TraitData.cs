using UnityEngine;

[CreateAssetMenu(fileName = "NewTrait", menuName = "Traits/TraitData")]
public class TraitData : ScriptableObject
{
    [Header("기본 정보")]
    public string traitName;
    [TextArea] public string description;
    public int unlockCost = 1;
    public Sprite icon;

    [Header("고정 수치 추가 (합연산)")]
    public int addStrength;
    public int addAgility;
    public int addIntelligence;
    public int addMana;

    [Header("배수 추가 (0.05 = 5% 증가)")]
    public float addStrMultiplier;
    public float addAgiMultiplier;
    public float addIntMultiplier;
    public float addManaMultiplier;
}