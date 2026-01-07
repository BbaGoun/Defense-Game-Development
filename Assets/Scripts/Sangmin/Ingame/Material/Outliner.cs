using UnityEngine;

namespace Sangmin
{
    public class Outliner : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField, Range(0f, 0.01f)]
        private float thickness = 0.005f;
        [ColorUsage(true, true), SerializeField]
        private Color outlineColor = Color.red;
        [SerializeField] private float yOffsetMultiplier = 10;
        [SerializeField] private bool isOutline;
        [SerializeField] private bool awakeDone = false;

        void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            _spriteRenderer.material.SetFloat("_Thickness", thickness);
            _spriteRenderer.material.SetColor("_OutlineColor", outlineColor);
            _spriteRenderer.material.SetFloat("_YOffsetMultiplier", yOffsetMultiplier);
            _spriteRenderer.material.SetFloat("_IsOutline", isOutline ? 1f : 0f);

            awakeDone = true;
        }

        public void OnOutline()
        {
            isOutline = true;
            _spriteRenderer.material.SetFloat("_IsOutline", isOutline ? 1f : 0f);
        }

        public void OffOutline()
        {
            isOutline = false;
            _spriteRenderer.material.SetFloat("_IsOutline", isOutline ? 1f : 0f);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!awakeDone)
                return;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.material.SetFloat("_Thickness", thickness);
                _spriteRenderer.material.SetColor("_OutlineColor", outlineColor);
                _spriteRenderer.material.SetFloat("_YOffsetMultiplier", yOffsetMultiplier);
                _spriteRenderer.material.SetFloat("_IsOutline", isOutline ? 1f : 0f);
            }
        }
#endif
    }
}
