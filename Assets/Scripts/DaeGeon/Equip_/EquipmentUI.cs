using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace DaeGeon
{
    public class EquipmentUI : MonoBehaviour
{
    public static EquipmentUI Instance;

    public enum SortType
    {
        Newest,    // 최신 획득 순
        Attack,    // 공격력 높은 순
        Defense,   // 방어력 높은 순
        Grade,     // 등급 순
        Name       // 이름 순
    }

    public Transform content;
    public GameObject equipmentPrefab;

    [Header("장착 슬롯 이미지")]
    public Image helmetSlot; 
    public Image chestSlot;
    public Image legsSlot;
    public Image weaponSlot;
    public Image bootsSlot;
    public Image glovesSlot;

    private SortType currentSort = SortType.Newest;
    
    private EquipmentType? currentFilter = null;

    private void Awake() => Instance = this;

    private void Start() 
    {
        RefreshList();
        UpdateAllSlots(); // 시작할 때 모든 슬롯 UI 상태 갱신
    }

    public void RefreshList()
    {
        // 1. 기존 리스트 UI 제거
        foreach (Transform child in content) Destroy(child.gameObject);

        if (InventoryManager.Instance == null) return;

        // 2. 인벤토리에서 아이템 리스트 가져오기 및 필터링
        var items = InventoryManager.Instance.equipItems
            .Where(item => currentFilter == null || item.data.equipmentType == currentFilter)
            .ToList();

        // 3. 정렬 적용
        items = GetSortedItems(items);

        // 4. 정렬된 리스트로 UI 생성
        foreach (var item in items)
        {
            var obj = Instantiate(equipmentPrefab, content);
            var btnScript = obj.GetComponent<EquipmentItemButton>();
            if (btnScript != null) btnScript.Setup(item);
        }
    }

    private List<ItemInstance> GetSortedItems(List<ItemInstance> items)
    {
        return currentSort switch
        {
            SortType.Attack  => items.OrderByDescending(i => i.attack).ToList(),
            SortType.Defense => items.OrderByDescending(i => i.defense).ToList(),
            SortType.Grade   => items.OrderByDescending(i => (int)i.data.grade).ToList(),
            SortType.Name    => items.OrderBy(i => i.data.itemName).ToList(),
            SortType.Newest  => items.OrderByDescending(i => i.acquiredTicks).ToList(), 
            _ => items
        };
    }

    // [UI 호출용 함수] 외부(Dropdown 등)에서 정렬 방식을 바꿀 때 사용
    public void SetSortType(int sortIndex)
    {
        currentSort = (SortType)sortIndex;
        RefreshList();
    }

    // [추가] 모든 슬롯 UI를 한꺼번에 갱신
    public void UpdateAllSlots()
    {
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            if (type != EquipmentType.None) UpdateSlotUI(type);
        }
    }

    public void UpdateSlotUI(EquipmentType type)
    {
        ItemInstance instance = EquipmentManager.Instance.GetEquippedInstance(type);
        Image slot = GetSlotImage(type);
        if (slot == null) return;

        // [추가] 슬롯 오브젝트에 Button 컴포넌트가 있다면 해제 기능을 연결합니다.
        Button slotBtn = slot.GetComponent<Button>();

        if (instance != null) 
        { 
            slot.sprite = instance.data.icon; 
            slot.color = Color.white;
            slot.enabled = true; // 이미지 활성화

            // 슬롯 클릭 시 해제 실행
            if (slotBtn != null)
            {
                slotBtn.onClick.RemoveAllListeners();
                slotBtn.onClick.AddListener(() => {
                    EquipmentManager.Instance.Unequip(type);
                    RefreshList(); // 인벤토리 버튼 텍스트 갱신을 위해 리스트 새로고침
                });
            }
        }
        else 
        { 
            slot.sprite = null; 
            slot.color = new Color(1, 1, 1, 0); 
            slot.enabled = false; // 이미지 비활성화

            if (slotBtn != null) slotBtn.onClick.RemoveAllListeners();
        }
    }

    public void SetFilter(string typeName)
    {
        if (string.Equals(typeName, "All", System.StringComparison.OrdinalIgnoreCase))
        {
            currentFilter = null;
        }
        else
        {
            if (System.Enum.TryParse(typeName, true, out EquipmentType parsedType))
            {
                currentFilter = parsedType;
            }
            else
            {
                // 3. 잘못된 값이 들어왔을 때만 경고 출력
                Debug.LogWarning($"{typeName}은(는) 올바른 EquipmentType 이름이 아닙니다! 인스펙터 입력을 확인하세요.");
                return;
            }
        }

        RefreshList();
    }

    private Image GetSlotImage(EquipmentType type) => type switch {
        EquipmentType.HELMET => helmetSlot, 
        EquipmentType.CHEST => chestSlot,
        EquipmentType.LEGS => legsSlot, 
        EquipmentType.WEAPON => weaponSlot,
        EquipmentType.BOOTS => bootsSlot, 
        EquipmentType.GLOVES => glovesSlot, 
        _ => null
    };
}
}