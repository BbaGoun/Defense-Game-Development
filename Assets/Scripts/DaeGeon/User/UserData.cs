using System; // 반드시 파일의 가장 첫 줄(최상단)에 있어야 합니다.

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