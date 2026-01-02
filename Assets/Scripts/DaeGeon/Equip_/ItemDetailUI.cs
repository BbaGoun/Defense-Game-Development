using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailUI : MonoBehaviour
{
    public static ItemDetailUI Instance;

    [Header("UI Panel")]
    public GameObject panelContent; // 실제 팝업창 (배경 포함)

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI infoText;    
    public TextMeshProUGUI equipBtnText;
    public Button equipButton;
    public Button upgradeButton;
    public Button dismantleButton;

    [Header("Upgrade Material UI (Static)")]
    public Image materialIcon;          
    public TextMeshProUGUI materialText; 

    private ItemInstance selectedItem; // 선택된 아이템 저장용

    private void Awake()
    {
        Instance = this;
    }

    public void Open(ItemInstance item)
    {
        if (item == null) return;
        
        selectedItem = item;
        panelContent.SetActive(true); // 팝업 "짠" 하고 띄우기
        RefreshUI();
    }

    public void Close()
    {
        panelContent.SetActive(false); // 팝업 닫기
    }

    public void RefreshUI()
    {
        if (selectedItem == null) return;

        // 1. 정보 텍스트 (강화수치, 스탯, 설명)
        string upgradeStr = selectedItem.upgradeLevel > 0 ? $" <color=yellow>(+{selectedItem.upgradeLevel})</color>" : "";
        string stats = selectedItem.attack > 0 ? $"공격력: {selectedItem.attack}\n" : "";
        stats += selectedItem.defense > 0 ? $"방어력: {selectedItem.defense}\n" : "";
        
        infoText.text = $"<b>{selectedItem.data.itemName}</b>{upgradeStr}\n" +
                        $"등급: {selectedItem.data.grade}\n" +
                        $"{stats}\n" +
                        $"설명: {selectedItem.data.description}";

        // 2. 버튼 텍스트 (장착/해제 상태 반영)
        equipBtnText.text = selectedItem.isEquipped ? "해제" : "장착";

        // 3. 버튼 리스너 (기존꺼 지우고 새로 연결)
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(OnEquipClick);

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClick);

        dismantleButton.onClick.RemoveAllListeners();
        dismantleButton.onClick.AddListener(OnDismantleClick);

        // 4. 아이콘 이미지
        if (iconImage != null)
            iconImage.sprite = selectedItem.data.icon;

        // 5. 강화 재료 UI 갱신
        UpdateUpgradeUI(selectedItem);
    }

    private void OnEquipClick()
    {
        if (selectedItem == null) return;

        Close(); 

        if (selectedItem.isEquipped)
            EquipmentManager.Instance.Unequip(selectedItem.data.equipmentType);
        else
            EquipmentManager.Instance.Equip(selectedItem);

        if (EquipmentUI.Instance != null) EquipmentUI.Instance.RefreshList();
    }

    public void UpdateUpgradeUI(ItemInstance item)
    {
        // 1. 데이터 가져오기 (재료 종류만 가져옵니다)
        ItemData reqMaterial = item.data.upgradeMaterial;
        
        // [수정] ItemInstance에 만든 공식을 사용하여 현재 필요한 개수를 가져옵니다.
        int reqAmount = item.GetNextUpgradeCost();

        // 2. 강화 재료 정보가 있는지 확인 (강화 불가 아이템은 reqMaterial이 null임)
        if (reqMaterial != null)
        {
            // 3. 보유량 확인 (MaterialManager에서 현재 내 인벤토리 확인)
            int ownedAmount = MaterialManager.Instance.GetMaterialCount(reqMaterial);

            // 4. UI 업데이트
            if (materialIcon != null)
            {
                materialIcon.sprite = reqMaterial.icon;
                materialIcon.color = Color.white; 
            }
            
            if (materialText != null)
            {
                // [표시] 필요개수 / 보유개수 (예: 7 / 15)
                materialText.text = $"{reqAmount} / {ownedAmount}";
                
                // 부족하면 빨간색, 충분하면 흰색
                materialText.color = (ownedAmount < reqAmount) ? Color.red : Color.white;
            }

            // 5. 강화 버튼 활성화 (재료가 충분해야만 클릭 가능)
            upgradeButton.interactable = (ownedAmount >= reqAmount);
        }
        else
        {
            // 강화 재료가 설정되지 않은 아이템 처리
            if (materialIcon != null) materialIcon.color = new Color(1, 1, 1, 0); 
            
            if (materialText != null)
            {
                materialText.text = "강화 불가 아이템";
                materialText.color = Color.gray;
            }
            
            upgradeButton.interactable = false;
        }
    }

    private void OnUpgradeClick()
    {
        // TryUpgrade 내부에서 MaterialManager와 통신하여 재료를 깎음
        if (selectedItem.TryUpgrade())
        {
            if (selectedItem.isEquipped)
                EquipmentManager.Instance.Equip(selectedItem); 

            RefreshUI();
            EquipmentUI.Instance.RefreshList();
            
            Debug.Log($"<color=green>{selectedItem.data.itemName} 강화 성공! 현재 +{selectedItem.upgradeLevel}</color>");
        }
        else
        {
            Debug.Log("<color=red>강화 실패: 재료가 부족하거나 강화할 수 없는 아이템입니다.</color>");
        }
    }

    private void OnDismantleClick()
    {
        // 1. 안전 장치: 선택된 아이템이 없으면 중단
        if (selectedItem == null) 
        {
            Debug.LogError("분해할 아이템이 선택되지 않았습니다.");
            return;
        }

        // 2. 보상 재료 및 개수 설정 (ItemData 에셋에 설정된 값)
        ItemData rewardMaterial = selectedItem.data.upgradeMaterial;
        int rewardAmount = selectedItem.data.dismantleAmount;

        // 3. 재료 지급 (MaterialManager를 통해 재료 가방에 추가)
        if (rewardMaterial != null && rewardAmount > 0)
        {
            MaterialManager.Instance.AddMaterial(rewardMaterial, rewardAmount);
            
            // 보상 알림창 띄우기 (DismantleResultUI)
            if (DismantleResultUI.Instance != null)
            {
                DismantleResultUI.Instance.ShowResult(rewardMaterial, rewardAmount);
            }
        }
        else
        {
            Debug.LogWarning($"{selectedItem.data.itemName}에 설정된 분해 보상(재료/개수)이 없습니다.");
        }

        // 4. 장착 중인 장비라면 먼저 해제 (중요: 스탯 감소 반영)
        if (selectedItem.isEquipped)
        {
            EquipmentManager.Instance.Unequip(selectedItem.data.equipmentType);
        }

        // 5. 인벤토리에서 아이템 제거
        if (InventoryManager.Instance.equipItems.Contains(selectedItem))
        {
            InventoryManager.Instance.equipItems.Remove(selectedItem);
        }

        // 6. UI 정리
        Close(); // 상세창 닫기
        EquipmentUI.Instance.RefreshList(); // 장비 슬롯 리스트 새로고침
        
        if (MaterialUI.Instance != null) 
            MaterialUI.Instance.RefreshMaterialList(); // 재료 리스트 새로고침
    }
}