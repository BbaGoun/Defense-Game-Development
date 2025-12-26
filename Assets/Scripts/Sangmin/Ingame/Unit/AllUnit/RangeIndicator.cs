using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// 유닛의 사거리를 원형으로 표시하는 컴포넌트 (SpriteRenderer 사용)
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class RangeIndicator : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private bool isActive;

        [Header("Range Visual Settings")]
        [SerializeField] private float currentRange;
        [SerializeField] private float multiplier;
        [SerializeField] private Color rangeColor = Color.yellow; // 원의 색상 (RGB만 사용, alpha는 alpha 필드 사용)
        [SerializeField] private float alpha = 0.3f; // 원의 투명도 (0~1)

        public void InitializeSpriteRenderer(float range)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            Color colorWithAlpha = rangeColor;
            colorWithAlpha.a = alpha;
            spriteRenderer.color = colorWithAlpha;

            currentRange = range;
            spriteRenderer.size = new Vector2(currentRange*multiplier, currentRange*multiplier);

            spriteRenderer.sortingOrder = 3;
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// 사거리 표시를 활성화
        /// </summary>
        public void ShowRange()
        {
            isActive = true;
            spriteRenderer.enabled = true;
        }

        /// <summary>
        /// 사거리 표시를 비활성화
        /// </summary>
        public void HideRange()
        {
            isActive = false;
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// 사거리가 변경되면 크기를 다시 조정
        /// </summary>
        public void UpdateRange(float newRange)
        {
            currentRange = newRange;
            spriteRenderer.size = new Vector2(currentRange*multiplier, currentRange*multiplier);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                Color colorWithAlpha = rangeColor;
                colorWithAlpha.a = alpha;
                spriteRenderer.color = colorWithAlpha;

                spriteRenderer.size = new Vector2(currentRange*multiplier, currentRange*multiplier);
            }
        }
#endif
    }
}

