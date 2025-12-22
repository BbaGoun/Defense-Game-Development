using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Attach Points")]
    public Transform headAttachPoint;
    public Transform bodyAttachPoint;

    // 실제 씬에 붙어있는 외형 프리팹
    private GameObject currentHeadItem;
    private GameObject currentBodyItem;

    // 미리보기용 오브젝트
    private GameObject previewHeadItem;
    private GameObject previewBodyItem;
    // 미리보기로 인해 숨겨진 실제 장착 오브젝트 상태 추적
    private bool hiddenHeadEquipped = false;
    private bool hiddenBodyEquipped = false;
    // 미리보기용 데이터 추적
    private ItemData previewHeadData;
    private ItemData previewBodyData;

    //  슬롯에 장착된 아이템 데이터 (진짜 장착 상태)
    public ItemData headSlot;
    public ItemData bodySlot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        // Ensure UI reflects player-dependent state after Player is ready
        var inv = FindAnyObjectByType<InventoryUI>();
        if (inv != null) inv.Refresh();
    }

    // ================= 장착 =================
    public void Equip(ItemData data)
    {
        // 장착 시 해당 부위 미리보기는 제거
        ClearPreviewForPoint(data.attachPoint);
        switch (data.attachPoint)
        {
            case AttachPoint.Head:
                Unequip(AttachPoint.Head);
                headSlot = data;
                currentHeadItem = AttachPrefab(data.equip, headAttachPoint);
                break;

            case AttachPoint.Body:
                Unequip(AttachPoint.Body);
                bodySlot = data;
                currentBodyItem = AttachPrefab(data.equip, bodyAttachPoint);
                break;
        }

        Debug.Log($"{data.itemName} 장착됨");
    }

    // ================= 해제 =================
    public void Unequip(AttachPoint point)
    {
        // 해제 시 해당 부위 미리보기도 제거
        ClearPreviewForPoint(point);
        switch (point)
        {
            case AttachPoint.Head:
                if (currentHeadItem != null)
                    Destroy(currentHeadItem);

                currentHeadItem = null;
                headSlot = null;
                break;

            case AttachPoint.Body:
                if (currentBodyItem != null)
                    Destroy(currentBodyItem);

                currentBodyItem = null;
                bodySlot = null;
                break;
        }

        Debug.Log($"{point} 슬롯 해제");
    }

    // ================= 내부 프리팹 부착 =================
    private GameObject AttachPrefab(GameObject prefab, Transform parent)
    {
        if (prefab == null || parent == null) return null;

        GameObject obj = Instantiate(prefab, parent);

        // 기본 Transform
        obj.transform.localPosition = prefab.transform.localPosition;
        obj.transform.localRotation = prefab.transform.localRotation;
        obj.transform.localScale = prefab.transform.localScale;

        // 보정값 (있으면)
        ItemAttach attach = prefab.GetComponent<ItemAttach>();
        if (attach != null)
        {
            obj.transform.localPosition += attach.attachSettings.localPosition;
            obj.transform.localRotation *= Quaternion.Euler(attach.attachSettings.localRotation);
            obj.transform.localScale =
                Vector3.Scale(obj.transform.localScale, attach.attachSettings.localScale);
        }

        return obj;
    }

    // ================= 미리보기 =================
    public void Preview(ItemData data)
    {
        if (data == null || data.equip == null) return;

        switch (data.attachPoint)
        {
            case AttachPoint.Head:
                // 기존 미리보기 제거
                if (previewHeadItem != null) Destroy(previewHeadItem);
                // 이미 장착된 아이템이 있으면 숨기고 상태 저장
                if (currentHeadItem != null && currentHeadItem.activeSelf)
                {
                    currentHeadItem.SetActive(false);
                    hiddenHeadEquipped = true;
                }
                previewHeadItem = AttachPrefab(data.equip, headAttachPoint);
                previewHeadData = data;
                MakePreviewVisual(previewHeadItem);
                break;

            case AttachPoint.Body:
                if (previewBodyItem != null) Destroy(previewBodyItem);
                if (currentBodyItem != null && currentBodyItem.activeSelf)
                {
                    currentBodyItem.SetActive(false);
                    hiddenBodyEquipped = true;
                }
                previewBodyItem = AttachPrefab(data.equip, bodyAttachPoint);
                previewBodyData = data;
                MakePreviewVisual(previewBodyItem);
                break;
        }
    }

    public void ClearPreview()
    {
        if (previewHeadItem != null) { Destroy(previewHeadItem); previewHeadItem = null; }
        if (previewBodyItem != null) { Destroy(previewBodyItem); previewBodyItem = null; }
        // 복원
        if (hiddenHeadEquipped && currentHeadItem != null)
        {
            currentHeadItem.SetActive(true);
            hiddenHeadEquipped = false;
        }
        if (hiddenBodyEquipped && currentBodyItem != null)
        {
            currentBodyItem.SetActive(true);
            hiddenBodyEquipped = false;
        }
        previewHeadData = null;
        previewBodyData = null;
    }

    void ClearPreviewForPoint(AttachPoint point)
    {
        switch (point)
        {
            case AttachPoint.Head:
                if (previewHeadItem != null) { Destroy(previewHeadItem); previewHeadItem = null; }
                if (hiddenHeadEquipped && currentHeadItem != null)
                {
                    currentHeadItem.SetActive(true);
                    hiddenHeadEquipped = false;
                }
                previewHeadData = null;
                break;
            case AttachPoint.Body:
                if (previewBodyItem != null) { Destroy(previewBodyItem); previewBodyItem = null; }
                if (hiddenBodyEquipped && currentBodyItem != null)
                {
                    currentBodyItem.SetActive(true);
                    hiddenBodyEquipped = false;
                }
                previewBodyData = null;
                break;
        }
    }

    // 토글형 미리보기: 같은 아이템이면 해당 부위 미리보기 해제, 아니면 미리보기
    public void TogglePreview(ItemData data)
    {
        if (data == null) return;

        switch (data.attachPoint)
        {
            case AttachPoint.Head:
                if (previewHeadData == data)
                    ClearPreviewForPoint(AttachPoint.Head);
                else
                    Preview(data);
                break;

            case AttachPoint.Body:
                if (previewBodyData == data)
                    ClearPreviewForPoint(AttachPoint.Body);
                else
                    Preview(data);
                break;
        }
    }

    // 미리보기용 시각 처리 (반투명 등)
    private void MakePreviewVisual(GameObject obj)
    {
        if (obj == null) return;

        var renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            // 인스턴스화된 머테리얼 사용
            var mat = r.material;
            if (mat != null && mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                c.a = 0.6f;
                mat.color = c;
            }
        }
    }

    // ================= UI용 상태 체크 =================
    public bool IsEquipped(ItemData data)
    {
        if (data == null) return false;

        switch (data.attachPoint)
        {
            case AttachPoint.Head:
                if (headSlot == null) return false;
                if (headSlot == data) return true;
                return !string.IsNullOrEmpty(headSlot.itemID) && headSlot.itemID == data.itemID;

            case AttachPoint.Body:
                if (bodySlot == null) return false;
                if (bodySlot == data) return true;
                return !string.IsNullOrEmpty(bodySlot.itemID) && bodySlot.itemID == data.itemID;
        }
        return false;
    }
}
