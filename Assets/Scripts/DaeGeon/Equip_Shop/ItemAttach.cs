using UnityEngine;

[System.Serializable]
public class AttachSettings
{
    public Vector3 localPosition = Vector3.zero;
    public Vector3 localRotation = Vector3.zero;
    public Vector3 localScale = Vector3.one;
}

public class ItemAttach : MonoBehaviour
{
    public Transform attachPoint;       // Prefab 안에서 장착 기준 위치
    public AttachSettings attachSettings = new AttachSettings();
}
