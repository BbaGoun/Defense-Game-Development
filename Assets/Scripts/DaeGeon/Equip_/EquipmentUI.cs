using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public static EquipmentUI Instance;

    public Transform content;
    public GameObject equipmentPrefab;

    [Header("장착 슬롯 이미지")]
    public Image helmetSlot; 
    public Image chestSlot;
    public Image legsSlot;
    public Image weaponSlot;
    public Image bootsSlot;
    public Image glovesSlot;

    private EquipmentType? currentFilter = null;

    private void Awake() => Instance = this;

    private void Start() 
    {
        RefreshList();
        UpdateAllSlots(); // 시작할 때 모든 슬롯 UI 상태 갱신
    }

    // 인벤토리 리스트 생성
    public void RefreshList()
    {
        foreach (Transform child in content) Destroy(child.gameObject);

        if (InventoryManager.Instance == null) return;

        foreach (var item in InventoryManager.Instance.equipItems) 
        {
            if (currentFilter == null || item.data.equipmentType == currentFilter)
            {
                var obj = Instantiate(equipmentPrefab, content);
                var btnScript = obj.GetComponent<EquipmentItemButton>();
                
                if (btnScript != null) btnScript.Setup(item);
            }
        }
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
        if (typeName == "All") currentFilter = null;
        else currentFilter = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), typeName);
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