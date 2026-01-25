using UnityEngine;

namespace DaeGeon
{
    [CreateAssetMenu(fileName = "New Material", menuName = "Game/Item/Material")]
    public class MaterialData : ItemData
{
    [Header("--- 재료 설정 ---")]
    public int maxStack = 999; // 최대 몇 개까지 겹칠 수 있는지
    public bool isConsumable;  // 사용 가능한 아이템인지 (예: 경험치 포션 등)
}
}