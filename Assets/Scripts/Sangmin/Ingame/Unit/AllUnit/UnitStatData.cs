using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatData", menuName = "Scriptable Objects/UnitStatData")]
public class UnitStatData : ScriptableObject
{
    public float attackDamage;
    // 특정 수를 attackSpeed로 나눈 시간마다 공격이 가능하도록 함
    public float attackSpeed;
    // 칸 수 단위로 셈 ex) 1, 2, 3
    public float attackRange;
    public Grade grade;
    public enum Grade
    {
        NORMAL,
        RARE,
        UNIQUE,
        LEGEND,
        MYTHIC
    }
}
