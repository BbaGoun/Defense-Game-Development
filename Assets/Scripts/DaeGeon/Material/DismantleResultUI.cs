using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DaeGeon
{
    public class DismantleResultUI : MonoBehaviour
{
    public static DismantleResultUI Instance;

    [Header("UI References")]
    public GameObject panel;           // 전체 팝업 오브젝트
    public Transform contentParent;    // 재료 슬롯이 생성될 부모 (Content 오브젝트)
    public GameObject materialPrefab;  // MaterialUI에서 쓰는 것과 동일한 프리팹

    private void Awake()
    {
        Instance = this;
        // 시작 시 패널 끄기
        if (panel != null) panel.SetActive(false);
    }

    /// <summary>
    /// 분해 결과를 화면에 표시합니다.
    /// </summary>
    public void ShowResult(ItemData material, int amount)
    {
        if (material == null || contentParent == null || materialPrefab == null) return;

        // 1. 이전 결과물 아이콘들 청소 (새로운 분해 결과를 보여주기 위함)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 프리팹 생성
        GameObject obj = Instantiate(materialPrefab, contentParent);

        // 3. 프리팹 내부 스크립트에 데이터 전달
        // 인벤토리에서 사용하는 MaterialItemButton(또는 비슷한 이름) 스크립트를 가져옵니다.
        MaterialItemButton slotScript = obj.GetComponent<MaterialItemButton>();
        if (slotScript != null)
        {
            // 수량(stackCount)이 반영된 임시 인스턴스를 만들어 Setup 호출
            slotScript.Setup(new ItemInstance(material, amount));
        }

        // 4. 팝업 표시
        panel.SetActive(true);
    }

    public void OnCloseClick()
    {
        panel.SetActive(false);
    }
}
}