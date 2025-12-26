[System.Serializable]
public class PlayerStatus
{
    public int strength;
    public int agility;
    public int intelligence;
    public int mana;

    // 두 스탯 객체를 더하는 로직
    public static PlayerStatus operator +(PlayerStatus a, PlayerStatus b)
    {
        if (a == null) return b;
        if (b == null) return a;

        return new PlayerStatus
        {
            strength = a.strength + b.strength,
            agility = a.agility + b.agility,
            intelligence = a.intelligence + b.intelligence,
            mana = a.mana + b.mana
        };
    }
}