[System.Serializable]
public class ItemInstance
{
    public string instanceID;   // 개별 아이템 식별자
    public ItemData data;       // 원본 SO 참조
    
    public int attack;          // 생성 시 결정된 실제 공격력
    public int defense;         // 생성 시 결정된 실제 방어력
    public bool isEquipped;

    // 아이템 생성 시 호출 (가챠 등)
    public ItemInstance(ItemData sourceData)
    {
        data = sourceData;
        instanceID = System.Guid.NewGuid().ToString(); // 고유 ID 부여
        
        // 원본 데이터의 범위 내에서 랜덤 수치 결정
        attack = UnityEngine.Random.Range(data.minAttack, data.maxAttack + 1);
        defense = UnityEngine.Random.Range(data.minDefense, data.maxDefense + 1);
    }
}