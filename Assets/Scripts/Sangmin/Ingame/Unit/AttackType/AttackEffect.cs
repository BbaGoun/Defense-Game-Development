using Sangmin;
using UnityEngine;

[RequireComponent(typeof(PoolAble))]
public class AttackEffect : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        animator.Rebind();
    }

    void OnDisable()
    {
        animator.Rebind();
    }
}
