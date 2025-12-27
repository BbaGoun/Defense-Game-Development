using UnityEngine;
using UnityEngine.UI;

public class TraitNode : MonoBehaviour
{
    [Header("Data Settings")]
    [SerializeField] private TraitData traitData;

    [Header("UI References")]
    [SerializeField] private Image buttonImage;    // 버튼의 본체 이미지 (색상 변경용)
    [SerializeField] private GameObject lockOverlay; // 잠금 표시용 오브젝트 (자물쇠 등)

    private bool isUnlocked = false;

    private void OnEnable()
    {
        // 창을 열 때마다 현재 상태에 맞춰 시각적 업데이트
        UpdateVisual();
    }

    public void OnClickNode()
    {
        // 노드 터치 시 설명창 열기
        if (TraitDescriptionUI.Instance != null)
        {
            TraitDescriptionUI.Instance.OpenDescription(traitData, this, isUnlocked);
        }
    }

    /// <summary>
    /// 설명창의 '해금' 버튼을 눌렀을 때 호출됨
    /// </summary>
    public void ConfirmUnlock()
    {
        if (isUnlocked) return;

        // 매니저를 통해 해금 시도
        if (TraitManager.Instance != null && TraitManager.Instance.TryUnlockTrait(traitData))
        {
            isUnlocked = true;
            UpdateVisual();
            
            // 설명창 UI 상태 갱신
            TraitDescriptionUI.Instance.OpenDescription(traitData, this, isUnlocked);
        }
    }

    /// <summary>
    /// 해금 여부에 따라 색상 및 잠금 레이어만 갱신
    /// </summary>
    private void UpdateVisual()
    {
        if (buttonImage != null)
        {
            // 해금되면 하얀색(직접 넣으신 이미지 색상 그대로), 해금 전에는 어둡게
            buttonImage.color = isUnlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f);
        }
        
        if (lockOverlay != null) 
        {
            // 해금되면 잠금 레이어를 끄고, 해금 전에는 켬
            lockOverlay.SetActive(!isUnlocked);
        }
    }
}