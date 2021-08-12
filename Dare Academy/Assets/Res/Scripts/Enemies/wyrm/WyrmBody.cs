using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

public class WyrmBody : WyrmSection
{
    public bool isLegs = false;

    protected override void Start()
    {
        base.Start();

        // we remove from current node
        RemoveFromCurrentNode();
        m_currentNode = null;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        Health = 5;
        Flags.SetFlags(flags.alwaysWinConflict, true);
    }

    protected void Update()
    {
        animator.SetBool("isLegs", isLegs);
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();
        BodyAnalyseLogic();
        doReanalyse = true;
    }

    public override void ReAnalyseStep()
    {
        base.ReAnalyseStep();
        BodyAnalyseLogic();
        doReanalyse = false;
    }

    private void BodyAnalyseLogic()
    {
        if (currentNode == null)
        {
            // is underground
            if (SectionInfront.currentNode != null && SectionInfront.ResurfacedThisStep == false)
            {
                Resurface();
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
        else
        {
            Vector2Int dir;
            GridNode node;

            // check if infront has burrowed
            if (SectionInfront.currentNode == null)
            {
                if (currentNode == SectionInfront.BurrowFrom)
                {
                    Burrow();
                }
                else
                {
                    node = SectionInfront.BurrowFrom;
                    if (node == null)
                        return;

                    dir = node.position.grid - currentNode.position.grid;
                    SetMovementDirection(dir);
                }
                return;
            }

            // follow in front
            if (SectionInfront.MovedThisStep)
            {
                node = SectionInfront.currentNode;
                if (node == null)
                    return;

                dir = node.position.grid - currentNode.position.grid;
                SetMovementDirection(dir);
            }
        }
    }

    public override void DrawStep()
    {
        // base.DrawStep();

        if (MovedThisStep && m_currentNode != null)
        {
            // #matthew what if the wyrm moves twice in different directions?
            m_animationController.animator.SetBool("IsMoving", true);
            LeanTween.move(gameObject, m_currentNode.position.world, m_stepController.stepTime)
                    .setOnComplete(() => { m_animationController.animator.SetBool("IsMoving", false); });
        }
    }

    public override void EndStep()
    {
        base.EndStep();

        ResurfacedThisStep = false;
    }

    protected override void Resurface()
    {
        m_currentNode = SectionInfront.currentNode;
        base.Resurface();
    }
}