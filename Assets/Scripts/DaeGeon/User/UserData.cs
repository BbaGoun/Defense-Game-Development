using System; 

[Serializable]
public class UserData
{
    public string userName;
    public string iconId;
    public string frameId;

    // 필요한 경우 레벨/경험치 추가
    public int level;
    public int exp;
}