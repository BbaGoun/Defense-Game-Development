using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance;

    // 재료 아이템들만 담는 리스트
    public List<ItemInstance> materialItems = new List<ItemInstance>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬이 바뀌어도 유지하고 싶다면 아래 주석 해제
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 재료 추가 함수
    public void AddMaterial(ItemData materialData, int amount)
    {
        if (materialData == null) return;

        // 1. 이미 같은 재료가 있는지 확인
        ItemInstance existing = materialItems.Find(x => x.data == materialData);

        if (existing != null)
        {
            // 2. 있으면 수량만 추가
            existing.stackCount += amount;
        }
        else
        {
            // 3. 없으면 새 인스턴스 생성 (수량 지정 생성자 사용)
            materialItems.Add(new ItemInstance(materialData, amount));
        }

        // 4. 재료 UI 갱신 (MaterialUI가 씬에 있을 때만)
        UpdateUI();
    }

    // 재료 소모 함수 (ItemInstance.TryUpgrade에서 호출됨)
    public bool ConsumeMaterial(ItemData materialData, int amount)
    {
        if (materialData == null) return false;

        ItemInstance target = materialItems.Find(x => x.data == materialData);
        
        // 재료가 있고, 수량이 충분한지 확인
        if (target != null && target.stackCount >= amount)
        {
            target.stackCount -= amount;
            
            // 수량이 0 이하면 리스트에서 제거
            if (target.stackCount <= 0)
            {
                materialItems.Remove(target);
            }
            
            UpdateUI();
            return true;
        }
        
        Debug.Log($"{materialData.itemName} 재료가 부족합니다.");
        return false; // 재료 부족 또는 해당 재료 없음
    }

    public int GetMaterialCount(ItemData targetData)
    {
        if (targetData == null) return 0;

        // 리스트에서 해당 데이터를 가진 아이템을 찾음
        ItemInstance found = materialItems.Find(x => x.data == targetData);

        // 찾았다면 그 아이템의 stackCount를, 못 찾았다면 0을 반환
        return found != null ? found.stackCount : 0;
    }

    // UI 갱신을 안전하게 호출하기 위한 헬퍼 함수
    private void UpdateUI()
    {
        if (MaterialUI.Instance != null)
        {
            MaterialUI.Instance.RefreshMaterialList();
        }
    }
}