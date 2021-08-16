using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

public class WyrmBody : WyrmSection
{
    public bool isLegs = false;
    private Vector3 m_animationMidNode = Vector3.zero;
    public bool isGoingToSurface;

    public List<GridNode> m_nodesVisited;


    protected override void Start()
    {
        base.Start();
        isGoingToSurface = false;

        // we remove from current node
        RemoveFromCurrentNode();
        m_currentNode = null;

        m_nodesVisited = new List<GridNode>();
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
        m_nodesVisited.Clear();
    }

    public override void ReAnalyseStep()
    {
        base.ReAnalyseStep();
        BodyAnalyseLogic();
        doReanalyse = false;
        if (m_currentNode != null)
        {
            m_animationMidNode = m_currentNode.position.world;
        }
    }

    private void BodyAnalyseLogic()
    {
        if (currentNode == null)
        {
            // is underground
            if (SectionInfront.currentNode != null && SectionInfront.ResurfacedThisStep == false)
            {
                isGoingToSurface = true;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
            return;
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

    public override void MoveStep()
    {
        m_nodesVisited.Add(m_currentNode);
        base.MoveStep();

    }

    public override void DrawStep()
    {
        // base.DrawStep();

        if (MovedThisStep && m_currentNode != null)
        {
            m_animationController.animator.SetBool("IsMoving", true);
            SetAnimationFlags(transform.position, m_animationMidNode);
            LeanTween.move(gameObject, m_animationMidNode, m_stepController.stepTime / 2).setOnComplete(() =>
            {
                if (m_currentNode is null)
                {
                    return;
                }

                SetAnimationFlags(transform.position, m_currentNode.position.world);
                LeanTween.move(gameObject, m_currentNode.position.world, m_stepController.stepTime / 2).setOnComplete(() =>
                {
                    m_animationController.animator.SetBool("IsMoving", false);
                }
                );
            }
            );

            m_animationMidNode = Vector3.zero;
        }
    }

    private void ZeroAnimationFlags()
    {
        animator.SetBool("movingUp", false);
        animator.SetBool("movingDown", false);
        animator.SetBool("movingLeft", false);
        animator.SetBool("movingRight", false);
    }

    public override void EndStep()
    {
        base.EndStep();
    }

    protected override void Resurface()
    {
        m_currentNode = SectionInfront.ResurfaceFrom;
        base.Resurface();
    }

    public override void PreMoveStep()
    {
        base.PreMoveStep();
        if(isGoingToSurface)
        {
            isGoingToSurface = false;
            Resurface();
        }
    }
}