using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    // items는 외형(Appearance) 아이템용 (SO 그대로 사용)
    public List<ItemData> items = new List<ItemData>(); 
    
    // [변경] equipItems는 장비(Equipment) 아이템용 (인스턴스 사용)
    public List<ItemInstance> equipItems = new List<ItemInstance>(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 통합 아이템 추가 함수. 
    /// 장비 아이템은 개별 인스턴스(랜덤 수치)로 생성하여 추가하고,
    /// 외형 아이템은 원본 데이터를 추가합니다.
    /// </summary>
    public ItemInstance AddItem(ItemData data)
    {
        if (data == null) return null;

        ItemInstance createdInstance = null;

        // 1. 장비 아이템인 경우 (랜덤 수치 결정 및 인스턴스 생성)
        if (data.equipmentType != EquipmentType.None)
        {
            createdInstance = new ItemInstance(data);
            equipItems.Add(createdInstance);

            // 장비창 UI 새로고침
            if (EquipmentUI.Instance != null) EquipmentUI.Instance.RefreshList();
        }
        
        // 2. 외형 아이템인 경우 (기존 로직 유지)
        if (data.attachPoint != AttachPoint.None)
        {
            if (!items.Contains(data)) 
            {
                // (상점 정렬 로직 생략 - 기존 코드 그대로 유지하세요)
                items.Add(data);
            }
        }

        PlayerEvents.OnClearPreviewRequest?.Invoke();
        
        // 방금 만든 장비 인스턴스를 반환 (가챠 매니저에서 사용함)
        return createdInstance;
    }

    // SaveManager에서 사용: 아이템 ID 목록으로 인벤토리 복구
    public void SetItemsByIDs(List<string> ids)
    {
        items.Clear();
        equipItems.Clear(); 

        if (ids == null || ids.Count == 0) return;
        if (ItemDatabase.Instance == null) return;

        foreach (var id in ids)
        {
            var it = ItemDatabase.Instance.GetByID(id);
            if (it != null)
            {
                AddItem(it);
            }
        }
    }

    // 아이템 존재 여부 확인
    public bool HasItem(ItemData data)
    {
        // 외형 리스트 확인
        if (items.Contains(data)) return true;

        // 장비 리스트 확인 (인스턴스들이 가진 원본 data와 비교)
        return equipItems.Exists(instance => instance.data == data);
    }
}