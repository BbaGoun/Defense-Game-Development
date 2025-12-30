using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string instanceID;
    public ItemData data;
    
    public int attack;
    public int defense;
    public int upgradeLevel = 0; 
    public bool isEquipped;
    
    // 재료 아이템의 수량을 저장할 변수
    public int stackCount = 1; 

    // 생성자 1: 일반 생성 (장비, 외형 등)
    public ItemInstance(ItemData sourceData)
    {
        data = sourceData;
        instanceID = System.Guid.NewGuid().ToString();
        stackCount = 1;

        // 장비 데이터일 경우 랜덤 스탯 부여
        attack = UnityEngine.Random.Range(data.minAttack, data.maxAttack + 1);
        defense = UnityEngine.Random.Range(data.minDefense, data.maxDefense + 1);
    }

    // 생성자 2: 수량 지정 생성 (재료 아이템 전용)
    public ItemInstance(ItemData sourceData, int amount)
    {
        data = sourceData;
        instanceID = System.Guid.NewGuid().ToString();
        stackCount = amount;
        
        attack = 0;
        defense = 0;
    }

    /// <summary>
    /// 강화를 시도합니다. 재료가 충분하면 소모하고 스탯을 올립니다.
    /// </summary>
    /// <returns>강화 성공 여부</returns>
    public bool TryUpgrade()
    {
        // 1. 강화 가능한 아이템인지 확인 (외형템 등은 upgradeMaterial이 None일 것)
        if (data.upgradeMaterial == null)
        {
            Debug.Log($"{data.itemName}은(는) 강화할 수 없는 아이템입니다.");
            return false;
        }

        // 2. 필요 재료 개수 계산 (기본 개수 + 강화당 추가 개수 로직 예시)
        // 예: 0강 -> 1강 시 기본 개수만큼, 이후 단계당 2개씩 증가
        int requiredAmount = data.baseMaterialCount + (upgradeLevel * 2);

        // 3. MaterialManager를 통해 재료 소모 시도
        if (MaterialManager.Instance.ConsumeMaterial(data.upgradeMaterial, requiredAmount))
        {
            // 4. 재료 소모에 성공했다면 능력치 상승
            PerformUpgrade();
            return true;
        }
        else
        {
            Debug.Log($"재료가 부족합니다! (필요: {data.upgradeMaterial.itemName} {requiredAmount}개)");
            return false;
        }
    }

    // 실제 수치 상승 로직 (TryUpgrade 내부에서 호출)
    private void PerformUpgrade()
    {
        upgradeLevel++;

        // 공격력/방어력이 있는 아이템인 경우 수치 상승
        if (attack > 0) attack += 5; 
        if (defense > 0) defense += 3;

        Debug.Log($"{data.itemName} 강화 성공! 현재 레벨: +{upgradeLevel}");
    }
}