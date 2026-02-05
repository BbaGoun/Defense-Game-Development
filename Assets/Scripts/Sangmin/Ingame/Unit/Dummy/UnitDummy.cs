using UnityEngine;

public class UnitDummy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public void Init(SpriteRenderer _sr, Animator _animator)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.sprite = _sr.sprite;
        spriteRenderer.flipX = _sr.flipX;
        spriteRenderer.material = _sr.material;
        spriteRenderer.sortingOrder = _sr.sortingOrder - 1;
        animator.runtimeAnimatorController = _animator.runtimeAnimatorController;
    }

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("TAttack");
        }
    }
}
