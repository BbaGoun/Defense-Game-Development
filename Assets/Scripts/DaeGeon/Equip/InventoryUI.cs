using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public Transform content;
    public GameObject itemButtonPrefab;

    // 장착 슬롯 이미지
    public Image headSlotImage;
    public Image bodySlotImage;
    
    // 슬롯 미리보기 리셋 버튼 (에디터에 연결)
    public Button previewResetButton;

    private List<InventoryItemButton> buttons = new List<InventoryItemButton>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Refresh();
        UpdateSlotIcon(AttachPoint.Head);
        UpdateSlotIcon(AttachPoint.Body);

        if (previewResetButton != null)
        {
            previewResetButton.onClick.RemoveAllListeners();
            previewResetButton.onClick.AddListener(() => {
                // 직접 호출 대신 방송 송출 (모든 플레이어의 미리보기 초기화)
                PlayerEvents.OnClearPreviewRequest?.Invoke();
                
                // 슬롯 아이콘은 현재 Instance(데이터 기준) 상태에 맞춰 갱신
                UpdateSlotIcon(AttachPoint.Head);
                UpdateSlotIcon(AttachPoint.Body);
            });
        }
    }

    // ================= 전체 새로고침 =================
    public void Refresh()
    {
        if (content == null || itemButtonPrefab == null) return;

        // 기존 버튼 제거
        for (int i = content.childCount - 1; i >= 0; --i)
            Destroy(content.GetChild(i).gameObject);

        buttons.Clear();

        if (InventoryManager.Instance == null) return;

        foreach (var item in InventoryManager.Instance.items)
            CreateInventoryItem(item);

        // 슬롯 아이콘도 갱신
        UpdateSlotIcon(AttachPoint.Head);
        UpdateSlotIcon(AttachPoint.Body);
    }

    void CreateInventoryItem(ItemData data)
    {
        GameObject obj = Instantiate(itemButtonPrefab, content);
        InventoryItemButton btn = obj.GetComponent<InventoryItemButton>();

        if (btn != null)
        {
            btn.Setup(data);
            buttons.Add(btn);
        }
    }

    // ================= 특정 부위 UI만 갱신 =================
    public void RefreshByAttachPoint(AttachPoint point)
    {
        foreach (var btn in buttons)
        {
            if (btn != null && btn.AttachPoint == point)
                btn.RefreshButton();
        }

        UpdateSlotIcon(point);
    }

    // ================= 슬롯 아이콘 갱신 =================
    public void UpdateSlotIcon(AttachPoint point)
    {
        // UI 데이터 확인용으로만 Instance 참조
        if (Player.Instance == null) return;

        switch (point)
        {
            case AttachPoint.Head:
                UpdateSingleSlot(headSlotImage, Player.Instance.headSlot);
                break;

            case AttachPoint.Body:
                UpdateSingleSlot(bodySlotImage, Player.Instance.bodySlot);
                break;
        }
    }

    // ================= 슬롯 하나 처리 (중복 코드 정리) =================
    void UpdateSingleSlot(Image slotImage, ItemData slotData)
    {
        if (slotImage == null) return;

        if (slotData != null && slotData.icon != null)
        {
            // 장착됨 → 아이콘 표시
            slotImage.sprite = slotData.icon;
            slotImage.color = Color.white; // alpha = 1
        }
        else
        {
            // 비어있음 → 완전 투명
            slotImage.sprite = null;
            slotImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}