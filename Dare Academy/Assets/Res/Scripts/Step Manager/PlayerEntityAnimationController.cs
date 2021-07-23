using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    public void ResetFaceDirection()
    {
        animator.SetFloat("HeadDirX", animator.GetFloat("WalkDirX"));
        animator.SetFloat("HeadDirY", animator.GetFloat("WalkDirY"));
    }
}
