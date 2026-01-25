using UnityEngine;
using System; // DateTime을 사용하기 위해 추가

namespace DaeGeon
{
    [System.Serializable]
    public class ItemInstance
{
    public string instanceID;
    public ItemData data;
    
    public int attack;
    public int defense;
    public int upgradeLevel = 0; 
    public bool isEquipped;
    
    public int stackCount = 1; 

    // [추가] 정렬을 위한 획득 시간 기록
    public long acquiredTicks; 

    // 생성자 1: 일반 생성 (장비 등)
    public ItemInstance(ItemData sourceData)
    {
        data = sourceData;
        instanceID = System.Guid.NewGuid().ToString();
        stackCount = 1;
        
        // 생성되는 순간의 시간을 기록 (정렬용)
        acquiredTicks = DateTime.Now.Ticks;

        attack = UnityEngine.Random.Range(data.minAttack, data.maxAttack + 1);
        defense = UnityEngine.Random.Range(data.minDefense, data.maxDefense + 1);
    }

    // 생성자 2: 수량 지정 생성 (재료 전용)
    public ItemInstance(ItemData sourceData, int amount)
    {
        data = sourceData;
        instanceID = System.Guid.NewGuid().ToString();
        stackCount = amount;
        
        acquiredTicks = DateTime.Now.Ticks; // 재료도 일단 기록
        
        attack = 0;
        defense = 0;
    }

    public int GetNextUpgradeCost()
    {
        if (data == null) return 0;
        return data.baseMaterialCount + (upgradeLevel * 2);
    }

    public bool TryUpgrade()
    {
        if (data.upgradeMaterial == null)
        {
            Debug.Log($"{data.itemName}은(는) 강화할 수 없는 아이템입니다.");
            return false;
        }

        int requiredAmount = GetNextUpgradeCost();

        if (MaterialManager.Instance.ConsumeMaterial(data.upgradeMaterial, requiredAmount))
        {
            PerformUpgrade();
            return true;
        }
        else
        {
            Debug.Log($"재료가 부족합니다! (필요: {data.upgradeMaterial.itemName} {requiredAmount}개)");
            return false;
        }
    }

    private void PerformUpgrade()
    {
        upgradeLevel++;

        acquiredTicks = System.DateTime.Now.Ticks;

        if (attack > 0) attack += 5; 
        if (defense > 0) defense += 3;

        Debug.Log($"{data.itemName} 강화 성공! 현재 레벨: +{upgradeLevel}");
    }
}
}