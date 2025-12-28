using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentGachaManager : MonoBehaviour
{
    public static EquipmentGachaManager Instance;

    [Header("Result UI (장비용)")]
    public GameObject resultPanel;
    public Transform resultRoot;
    public GameObject resultItemPrefab; // 장비용 결과 프리팹

    private void Awake()
    {
        Instance = this;
        if(resultPanel != null) resultPanel.SetActive(false);
    }

    public void Draw(int drawCount)
    {
        ClearResultUI();
        resultPanel.SetActive(true);

        for (int i = 0; i < drawCount; i++)
        {
            // 1. 확률에 따라 원본 데이터(설계도) 뽑기
            ItemData selectedData = GetRandomEquipmentByProbability();

            if (selectedData != null)
            {
                // 2. 인벤토리에 추가 (내부에서 ItemInstance 생성 & 랜덤 수치 결정됨)
                InventoryManager.Instance.AddItem(selectedData);

                // 3. UI 표시를 위해 방금 생성된 인스턴스 가져오기
                ItemInstance newInstance = InventoryManager.Instance.equipItems.Last();
                CreateResultUI(newInstance);

                Debug.Log($"[가챠] {selectedData.itemName} 획득! (공격력: {newInstance.attack})");
            }
        }
        
        // 장비창 UI가 열려있다면 새로고침
        if (EquipmentUI.Instance != null) EquipmentUI.Instance.RefreshList();
    }

    void CreateResultUI(ItemInstance instance)
    {
        GameObject go = Instantiate(resultItemPrefab, resultRoot);
        // 결과창 전용 스크립트가 있다면 Setup 호출 (없으면 버튼 스크립트 재활용 가능)
        var ui = go.GetComponent<EquipmentItemButton>(); 
        if (ui != null) ui.Setup(instance);
    }

    ItemData GetRandomEquipmentByProbability()
    {
        // 🔑 ItemDatabase의 equipmentItems 카테고리 참조
        var pool = ItemDatabase.Instance.equipmentItems;
        if (pool.Count == 0) return null;

        float roll = Random.value; // 0.0 ~ 1.0

        // 확률 분포 (예시: Common 60%, Uncommon 25%, Rare 10%, Epic 4%, Legendary 1%)
        ItemGrade targetGrade;

        if (roll < 0.60f)      targetGrade = ItemGrade.Common;
        else if (roll < 0.85f) targetGrade = ItemGrade.Uncommon;
        else if (roll < 0.95f) targetGrade = ItemGrade.Rare;
        else if (roll < 0.99f) targetGrade = ItemGrade.Epic;
        else                   targetGrade = ItemGrade.Legendary;

        // 해당 등급의 아이템들만 필터링
        var filteredPool = pool.Where(it => it.grade == targetGrade).ToList();

        // 만약 해당 등급에 아이템이 없다면 안전장치로 전체에서 랜덤
        if (filteredPool.Count == 0) return pool[Random.Range(0, pool.Count)];

        return filteredPool[Random.Range(0, filteredPool.Count)];
    }

    void ClearResultUI()
    {
        foreach (Transform child in resultRoot) Destroy(child.gameObject);
    }

    public void CloseResultPanel() => resultPanel.SetActive(false);
}