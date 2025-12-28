using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    // [변경] ItemData 대신 ItemInstance를 담는 딕셔너리로 변경
    public Dictionary<EquipmentType, ItemInstance> currentEquips = new Dictionary<EquipmentType, ItemInstance>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 모든 장비 슬롯을 null로 초기화
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            if (type != EquipmentType.None) currentEquips[type] = null;
        }
    }

    // [변경] 인스턴스를 매개변수로 받습니다.
    public void Equip(ItemInstance instance)
    {
        // 원본 데이터(instance.data)가 있는지 확인
        if (instance == null || instance.data.equipmentType == EquipmentType.None) return;

        EquipmentType type = instance.data.equipmentType;

        // 1. 해당 부위에 이미 장착된 아이템이 있다면 해제 처리 (필요 시)
        if (currentEquips[type] != null)
        {
            currentEquips[type].isEquipped = false;
        }

        // 2. 새로운 아이템 장착
        currentEquips[type] = instance;
        instance.isEquipped = true;

        // 3. 실제 캐릭터 외형 변경 요청 (원본 data 전달)
        // Player 스크립트는 원본 프리팹 정보가 필요하므로 instance.data를 보냅니다.
        PlayerEvents.OnEquipRequest?.Invoke(instance.data);

        // 4. UI 슬롯 업데이트
        if (EquipmentUI.Instance != null)
            EquipmentUI.Instance.UpdateSlotUI(type);
            
        Debug.Log($"{instance.data.itemName} (ATK: {instance.attack}) 장착 완료!");
    }

    // [추가/변경] UI에서 장착된 아이템 정보를 가져갈 때 사용
    public ItemInstance GetEquippedInstance(EquipmentType type) 
    {
        return currentEquips.ContainsKey(type) ? currentEquips[type] : null;
    }

    // 기존 함수 이름을 유지하고 싶을 때를 위한 래퍼 (선택 사항)
    public ItemData GetEquippedItem(EquipmentType type) 
    {
        return currentEquips.ContainsKey(type) && currentEquips[type] != null 
               ? currentEquips[type].data : null;
    }
}