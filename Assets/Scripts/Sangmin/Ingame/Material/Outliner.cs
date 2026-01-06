using UnityEngine;

public class Outliner : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _material;
    [SerializeField][Range(0f, 0.01f)] private float thickness;
    [SerializeField] private Color outlineColor;
    [SerializeField] private float yOffsetMultiplier;
    [SerializeField] bool awakeDone = false;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = new Material(_spriteRenderer.material);
        _spriteRenderer.material = _material;

        _material.SetFloat("_Thickness", thickness);
        _material.SetColor("_OutlineColor", outlineColor);
        _material.SetFloat("_YOffsetMultiplier", yOffsetMultiplier);

        awakeDone = true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!awakeDone)
            return;

        if (_spriteRenderer != null)
        {
            _material.SetFloat("_Thickness", thickness);
            _material.SetColor("_OutlineColor", outlineColor);
            _material.SetFloat("_YOffsetMultiplier", yOffsetMultiplier);
        }
    }
#endif
}
