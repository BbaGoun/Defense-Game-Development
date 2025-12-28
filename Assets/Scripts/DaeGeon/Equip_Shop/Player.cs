using UnityEngine;

public class Player : MonoBehaviour
{
    // 데이터의 기준점이 되는 단 하나의 인스턴스
    public static Player Instance; 

    [Header("Attach Points")]
    public Transform headAttachPoint;
    public Transform bodyAttachPoint;

    // 각 유닛이 개별적으로 생성한 프리팹 오브젝트
    private GameObject currentHeadItem;
    private GameObject currentBodyItem;
    private GameObject previewHeadItem;
    private GameObject previewBodyItem;

    // 미리보기 상태 추적용 변수
    private bool hiddenHeadEquipped = false;
    private bool hiddenBodyEquipped = false;
    private ItemData previewHeadData;
    private ItemData previewBodyData;

    [Header("Current Data (Only for Instance)")]
    // 실제 장착 데이터 (Instance로 지정된 유닛의 변수만 사용됨)
    public ItemData headSlot;
    public ItemData bodySlot;

    private void Awake()
    {
        // 씬에서 가장 먼저 활성화된 Player가 데이터 기준(Instance)이 됨
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        // 모든 Player 유닛이 방송국 채널을 구독
        PlayerEvents.OnEquipRequest += Equip;
        PlayerEvents.OnUnequipRequest += Unequip;
        PlayerEvents.OnTogglePreviewRequest += TogglePreview;
        PlayerEvents.OnClearPreviewRequest += ClearPreview;

        // 활성화될 때: 기준 데이터(Instance)를 확인하여 내 외형을 동기화
        RefreshAppearance();
    }

    private void OnDisable()
    {
        PlayerEvents.OnEquipRequest -= Equip;
        PlayerEvents.OnUnequipRequest -= Unequip;
        PlayerEvents.OnTogglePreviewRequest -= TogglePreview;
        PlayerEvents.OnClearPreviewRequest -= ClearPreview;
    }

    // ================= 외형 동기화 =================
    public void RefreshAppearance()
    {
        // 기준점(Instance)이 없거나, 내가 아직 준비 중일 땐 리턴
        if (Instance == null) return;

        // Instance가 들고 있는 장착 데이터를 가져와서 내 몸에 입힘
        if (Instance.headSlot != null) EquipLogic(Instance.headSlot);
        if (Instance.bodySlot != null) EquipLogic(Instance.bodySlot);
    }

    // ================= 장착 요청 처리 =================
    public void Equip(ItemData data)
    {
        if (data == null) return;

        // 1. 기준 데이터(Instance)를 업데이트 (모든 유닛의 공통 장부 수정)
        if (Instance != null)
        {
            if (data.attachPoint == AttachPoint.Head) Instance.headSlot = data;
            else if (data.attachPoint == AttachPoint.Body) Instance.bodySlot = data;
        }

        // 2. 실제 내 몸에 프리팹 생성
        EquipLogic(data);
    }

    // 실제 오브젝트를 생성하고 교체하는 내부 로직
    private void EquipLogic(ItemData data)
    {
        ClearPreviewForPoint(data.attachPoint);

        if (data.attachPoint == AttachPoint.Head)
        {
            if (currentHeadItem != null) Destroy(currentHeadItem);
            currentHeadItem = AttachPrefab(data.equip, headAttachPoint);
        }
        else if (data.attachPoint == AttachPoint.Body)
        {
            if (currentBodyItem != null) Destroy(currentBodyItem);
            currentBodyItem = AttachPrefab(data.equip, bodyAttachPoint);
        }
    }

    // ================= 해제 요청 처리 =================
    public void Unequip(AttachPoint point)
    {
        // 1. 기준 데이터 삭제
        if (Instance != null)
        {
            if (point == AttachPoint.Head) Instance.headSlot = null;
            else if (point == AttachPoint.Body) Instance.bodySlot = null;
        }

        // 2. 내 몸에서 오브젝트 제거
        UnequipLogic(point);
    }

    private void UnequipLogic(AttachPoint point)
    {
        ClearPreviewForPoint(point);

        if (point == AttachPoint.Head && currentHeadItem != null)
        {
            Destroy(currentHeadItem);
            currentHeadItem = null;
        }
        else if (point == AttachPoint.Body && currentBodyItem != null)
        {
            Destroy(currentBodyItem);
            currentBodyItem = null;
        }
    }

    // ================= 프리팹 생성 및 부착 =================
    private GameObject AttachPrefab(GameObject prefab, Transform parent)
    {
        if (prefab == null || parent == null) return null;

        GameObject obj = Instantiate(prefab, parent);
        obj.transform.localPosition = prefab.transform.localPosition;
        obj.transform.localRotation = prefab.transform.localRotation;
        obj.transform.localScale = prefab.transform.localScale;

        ItemAttach attach = prefab.GetComponent<ItemAttach>();
        if (attach != null)
        {
            obj.transform.localPosition += attach.attachSettings.localPosition;
            obj.transform.localRotation *= Quaternion.Euler(attach.attachSettings.localRotation);
            obj.transform.localScale = Vector3.Scale(obj.transform.localScale, attach.attachSettings.localScale);
        }

        return obj;
    }

    // ================= 미리보기 (Toggle/Clear) =================
    public void TogglePreview(ItemData data)
    {
        if (data == null) return;
        
        ItemData currentCompare = (data.attachPoint == AttachPoint.Head) ? previewHeadData : previewBodyData;

        if (currentCompare == data) ClearPreviewForPoint(data.attachPoint);
        else Preview(data);
    }

    public void Preview(ItemData data)
    {
        if (data == null || data.equip == null) return;

        if (data.attachPoint == AttachPoint.Head)
        {
            if (previewHeadItem != null) Destroy(previewHeadItem);
            if (currentHeadItem != null && currentHeadItem.activeSelf)
            {
                currentHeadItem.SetActive(false);
                hiddenHeadEquipped = true;
            }
            previewHeadItem = AttachPrefab(data.equip, headAttachPoint);
            previewHeadData = data;
            MakePreviewVisual(previewHeadItem);
        }
        else
        {
            if (previewBodyItem != null) Destroy(previewBodyItem);
            if (currentBodyItem != null && currentBodyItem.activeSelf)
            {
                currentBodyItem.SetActive(false);
                hiddenBodyEquipped = true;
            }
            previewBodyItem = AttachPrefab(data.equip, bodyAttachPoint);
            previewBodyData = data;
            MakePreviewVisual(previewBodyItem);
        }
    }

    public void ClearPreview()
    {
        ClearPreviewForPoint(AttachPoint.Head);
        ClearPreviewForPoint(AttachPoint.Body);
    }

    private void ClearPreviewForPoint(AttachPoint point)
    {
        if (point == AttachPoint.Head)
        {
            if (previewHeadItem != null) { Destroy(previewHeadItem); previewHeadItem = null; }
            if (hiddenHeadEquipped && currentHeadItem != null) { currentHeadItem.SetActive(true); hiddenHeadEquipped = false; }
            previewHeadData = null;
        }
        else
        {
            if (previewBodyItem != null) { Destroy(previewBodyItem); previewBodyItem = null; }
            if (hiddenBodyEquipped && currentBodyItem != null) { currentBodyItem.SetActive(true); hiddenBodyEquipped = false; }
            previewBodyData = null;
        }
    }

    private void MakePreviewVisual(GameObject obj)
    {
        if (obj == null) return;
        // 2D SpriteRenderer 대응
        var srs = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs) { Color c = sr.color; c.a = 0.6f; sr.color = c; }
        
        // 3D Renderer 대응
        var rs = obj.GetComponentsInChildren<Renderer>();
        foreach (var r in rs) { if (r is SpriteRenderer) continue; var m = r.material; if (m != null && m.HasProperty("_Color")) { Color c = m.color; c.a = 0.6f; m.color = c; } }
    }

    // ================= UI 상태 체크 (항상 Instance 기준) =================
    public bool IsEquipped(ItemData data)
    {
        if (Instance == null || data == null) return false;
        ItemData slot = (data.attachPoint == AttachPoint.Head) ? Instance.headSlot : Instance.bodySlot;
        if (slot == null) return false;
        return slot.itemID == data.itemID;
    }
}