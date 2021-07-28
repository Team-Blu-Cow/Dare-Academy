using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

public class WyrmBody : WyrmSection
{
    protected override void OnValidate()
    {
        base.OnValidate();
        Health = 5;
        Flags.SetFlags(flags.alwaysWinConflict, true);
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();

        if(Health < 5)
        {
            Health = 5;
            SectionInfront.Health -= 1;
        }

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

            node = SectionInfront.currentNode;
            if (node == null)
                return;

            dir = node.position.grid - currentNode.position.grid;
            SetMovementDirection(dir);
        }
    }

    protected override void Resurface()
    {
        m_currentNode = SectionInfront.currentNode;
        base.Resurface();
    }
}