using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator: MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleAnimation(bool walk, bool run, bool shot, bool reload)
    {
        if (walk)
        {
            animator.SetBool("isWalk", walk);

            if (run)
            {
                animator.SetBool("isRun", run);
            }
            else
            {
                animator.SetBool("isRun", run);
            }

            if (shot)
            {
                PlayTargetAnimation("Shot", shot);
            }

            if (reload)
            {
                PlayTargetAnimation("Reload", reload);
            }
        }
        else
        {
            animator.SetBool("isWalk", walk);
        }
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteract)
    {
        animator.applyRootMotion = isInteract;
        animator.SetBool("isInteract", isInteract);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}