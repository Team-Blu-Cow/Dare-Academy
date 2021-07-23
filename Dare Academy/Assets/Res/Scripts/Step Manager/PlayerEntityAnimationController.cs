using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    // INITIALISATION METHODS *********************************************************************
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    public void ResetFaceDirection()
    {
        animator.SetFloat("HeadDirX", animator.GetFloat("WalkDirX"));
        animator.SetFloat("HeadDirY", animator.GetFloat("WalkDirY"));
    }
}
