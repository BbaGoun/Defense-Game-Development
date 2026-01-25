using System.Collections.Generic;
using UnityEngine;

namespace DaeGeon
{
    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager Instance;
        public Dictionary<EquipmentType, ItemInstance> currentEquips = new Dictionary<EquipmentType, ItemInstance>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
            {
                if (type != EquipmentType.None) currentEquips[type] = null;
            }
        }

        public ItemInstance GetEquippedInstance(EquipmentType type)
        {
            return currentEquips.ContainsKey(type) ? currentEquips[type] : null;
        }

        public void Equip(ItemInstance instance)
        {
            if (instance == null || instance.data == null) return;
            EquipmentType type = instance.data.equipmentType;

            // 이미 같은 아이템을 장착 중이면 해제 로직으로 연결 (토글 기능)
            if (currentEquips[type] == instance)
            {
                Unequip(type);
                return;
            }

            // 기존 다른 장비 해제
            if (currentEquips[type] != null) currentEquips[type].isEquipped = false;

            currentEquips[type] = instance;
            instance.isEquipped = true;

            UpdateAfterChange(type, instance.data);
        }

        public void Unequip(EquipmentType type)
        {
            if (currentEquips.ContainsKey(type) && currentEquips[type] != null)
            {
                currentEquips[type].isEquipped = false;
                currentEquips[type] = null;

                UpdateAfterChange(type, null);
            }
        }

        private void UpdateAfterChange(EquipmentType type, ItemData data)
        {
            // 1. 외형 변경
            PlayerEvents.OnEquipRequest?.Invoke(data);
            // 2. 슬롯 UI 갱신
            if (EquipmentUI.Instance != null) EquipmentUI.Instance.UpdateSlotUI(type);
            // 3. 스탯 재계산
            UpdatePlayerBonusStats();
        }

        private void UpdatePlayerBonusStats()
        {
            PlayerStatus totalBonus = new PlayerStatus();
            foreach (var equip in currentEquips.Values)
            {
                if (equip != null)
                {
                    totalBonus.strength += equip.attack;
                    totalBonus.agility += equip.defense;
                }
            }

            if (PlayerStatManager.Instance != null)
                PlayerStatManager.Instance.SetBonusStatus(totalBonus);
        }
    }
}