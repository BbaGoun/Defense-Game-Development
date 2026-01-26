using UnityEngine;

namespace Sangmin
{
    public class UnitAura : MonoBehaviour
    {
        public float alpha = 0.5f;
        public SpriteRenderer spriteRenderer;
        public Animator animator;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            animator.Rebind();
        }

        private void OnDisable()
        {
            animator.Rebind();
        }

        public void Initialize(Unit unit)
        {
            SetAuraColor(unit);
        }

        public void SetAuraColor(Unit unit)
        {
            Color colorWithAlpha = UnitGradeColor.GetAuraColor(unit.unitData.grade);
            colorWithAlpha.a = alpha;

            spriteRenderer.color = colorWithAlpha;
        }
    }
}