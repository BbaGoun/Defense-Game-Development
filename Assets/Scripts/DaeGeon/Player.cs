using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Attach Points")]
    public Transform headAttachPoint;
    public Transform bodyAttachPoint;

    // 현재 장착 중인 아이템 저장
    private GameObject currentHeadItem;
    private GameObject currentBodyItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 아이템을 해당 AttachPoint에 장착 (Sprite 기준)
    /// </summary>
    public void AttachItem(Sprite icon, AttachPoint attachPoint)
    {
        Transform attachTransform = null;
        GameObject oldItem = null;

        switch (attachPoint)
        {
            case AttachPoint.Head:
                attachTransform = headAttachPoint;
                oldItem = currentHeadItem;
                break;
            case AttachPoint.Body:
                attachTransform = bodyAttachPoint;
                oldItem = currentBodyItem;
                break;
        }

        if (attachTransform == null || icon == null)
        {
            Debug.LogWarning("AttachPoint 혹은 Icon이 null입니다.");
            return;
        }

        // 기존 아이템 제거
        if (oldItem != null) Destroy(oldItem);

        // 새로운 아이템 GameObject 생성
        GameObject newItem = new GameObject(icon.name);
        newItem.transform.SetParent(attachTransform);
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        newItem.transform.localScale = Vector3.one;

        // SpriteRenderer 추가
        var sr = newItem.AddComponent<SpriteRenderer>();
        sr.sprite = icon;

        // 현재 장착 아이템 업데이트
        switch (attachPoint)
        {
            case AttachPoint.Head:
                currentHeadItem = newItem;
                break;
            case AttachPoint.Body:
                currentBodyItem = newItem;
                break;
        }

        Debug.Log($"{icon.name}이 {attachPoint}에 장착됨.");
    }
}
