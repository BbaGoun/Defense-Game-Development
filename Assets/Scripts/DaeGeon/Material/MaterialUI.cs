using UnityEngine;

public class MaterialUI : MonoBehaviour
{
    public static MaterialUI Instance;

    public GameObject materialSlotPrefab; // 재료 전용 프리팹
    public Transform contentParent;      // 슬롯들이 생성될 부모 (ScrollView의 Content)

    private void Awake()
    {
        Instance = this;
    }

    // 화면이 켜질 때나 재료가 바뀔 때 호출
    public void RefreshMaterialList()
    {
        // 기존 슬롯 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // MaterialManager의 리스트를 순회하며 생성
        foreach (var item in MaterialManager.Instance.materialItems)
        {
            GameObject obj = Instantiate(materialSlotPrefab, contentParent);
            // MaterialItemButton 스크립트의 Setup 호출
            obj.GetComponent<MaterialItemButton>().Setup(item);
        }
    }
}