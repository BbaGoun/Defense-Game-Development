using System.Collections.Generic;
using System.Linq; // 등급별 필터링을 위해 추가
using UnityEngine;

namespace DaeGeon
{
    public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("--- 통합 데이터베이스 (외형 포함) ---")]
    [Tooltip("모든 ItemData를 할당하세요. 기존 외형 로직 및 Save/Load에서 사용합니다.")]
    public List<ItemData> allItems = new List<ItemData>();

    [Header("--- 장비 전용 카테고리 ---")]
    [Tooltip("가챠에서 사용될 장비 아이템들만 따로 관리합니다.")]
    public List<ItemData> equipmentItems = new List<ItemData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 기존 외형 및 통합 검색 로직 (수정 없음)
    public ItemData GetByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        foreach (var it in allItems)
        {
            if (it != null && it.itemID == id) return it;
        }
        return null;
    }

    // --- 장비 전용 추가 기능 ---

    /// <summary>
    /// 장비 가챠용: 등급에 따른 장비 리스트를 반환합니다.
    /// ItemData에 Grade(Normal, Rare 등)가 설정되어 있어야 합니다.
    /// </summary>
    public List<ItemData> GetEquipmentsByGrade(ItemGrade grade)
    {
        // equipmentItems 리스트에서 해당 등급만 골라냅니다.
        return equipmentItems.Where(it => it.grade == grade).ToList();
    }
}
}