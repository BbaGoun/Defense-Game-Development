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
    private void Start() => RefreshList();

    public void RefreshList()
    {
        foreach (Transform child in content) Destroy(child.gameObject);

        if (InventoryManager.Instance == null) return;

        // [변경] 이제 item은 ItemInstance 타입입니다.
        foreach (var item in InventoryManager.Instance.equipItems) 
        {
            // 원본 데이터(data)의 타입을 확인하여 필터링합니다.
            if (currentFilter == null || item.data.equipmentType == currentFilter)
            {
                var obj = Instantiate(equipmentPrefab, content);
                var btnScript = obj.GetComponent<EquipmentItemButton>();
                
                // [변경] Setup 함수에 ItemInstance 전체를 넘겨줍니다.
                if (btnScript != null) btnScript.Setup(item);
            }
        }
    }

    public void SetFilter(string typeName)
    {
        if (typeName == "All") currentFilter = null;
        else currentFilter = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), typeName);
        RefreshList();
    }

    public void UpdateSlotUI(EquipmentType type)
    {
        // [변경] EquipmentManager에서도 이제 ItemInstance를 가져와야 합니다.
        ItemInstance instance = EquipmentManager.Instance.GetEquippedInstance(type);
        Image slot = GetSlotImage(type);
        if (slot == null) return;

        // [변경] instance 안의 원본 데이터(data)에서 아이콘을 가져옵니다.
        if (instance != null) 
        { 
            slot.sprite = instance.data.icon; 
            slot.color = Color.white; 
        }
        else 
        { 
            slot.sprite = null; 
            slot.color = new Color(1, 1, 1, 0); 
        }
    }

    private Image GetSlotImage(EquipmentType type) => type switch {
        EquipmentType.Helmet => helmetSlot, 
        EquipmentType.Chest => chestSlot,
        EquipmentType.Legs => legsSlot, 
        EquipmentType.Weapon => weaponSlot,
        EquipmentType.Boots => bootsSlot, 
        EquipmentType.Gloves => glovesSlot, 
        _ => null
    };
}