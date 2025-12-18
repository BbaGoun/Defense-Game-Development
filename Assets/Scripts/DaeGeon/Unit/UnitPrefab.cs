using UnityEngine;

public class UnitPrefab : MonoBehaviour
{
    [Header("Status")]
    public int shardCount = 0;   // 현재 조각 수
    public int level = 1;        // 강화 레벨

    // 전투용 스탯
    public float attack;
    public float attackSpeed;
    public float range;

    // 외부에서 데이터 받아 초기화
    public void Setup(UnitData data, int initialShards = 0, int initialLevel = 1)
    {
        shardCount = initialShards;
        level = initialLevel;

        attack = data.baseAttack;
        attackSpeed = data.baseAttackSpeed;
        range = data.baseRange;

        // 강화 시 레벨 반영
        for (int i = 1; i < level; i++)
            ApplyUpgrade(data.upgradeMultiplier);
    }

    // 강화 함수 (조각 충분 시)
    public bool Upgrade(UnitData data)
    {
        if (shardCount >= data.shardsRequiredPerUpgrade)
        {
            shardCount -= data.shardsRequiredPerUpgrade;
            level++;
            ApplyUpgrade(data.upgradeMultiplier);
            return true;
        }
        return false;
    }

    // 조각 추가
    public void AddShards(int amount)
    {
        shardCount += amount;
    }

    // Prefab 활성화/비활성화
    public void Activate(bool active)
    {
        gameObject.SetActive(active);
    }

    // 내부 함수: 레벨업 시 스탯 적용
    private void ApplyUpgrade(float multiplier)
    {
        attack *= multiplier;
        attackSpeed *= multiplier;
        range *= multiplier;
    }
}
