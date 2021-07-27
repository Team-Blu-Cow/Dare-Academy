using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

public abstract class WyrmSection : GridEntity
{
    [SerializeField] private WyrmSection m_sectionInfront;
    private WyrmSection m_sectionBehind;

    protected SpriteRenderer spriteRenderer;

    public WyrmSection SectionInfront
    {
        get => m_sectionInfront;
        set => m_sectionInfront = value;
    }

    public WyrmSection SectionBehind
    {
        get => m_sectionBehind;
        set => m_sectionBehind = value;
    }

    protected GridNode m_burromFrom;

    public GridNode BurrowFrom => m_burromFrom;

    public bool ResurfacedThisStep = false;

    protected enum BossPhase
    {
        Phase1,
        Phase2,
        Phase3,
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        if (m_sectionInfront != null)
        {
            m_sectionInfront.m_sectionBehind = this;
        }
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();
        ResurfacedThisStep = false;
    }

    protected virtual void Burrow()
    {
        RemoveFromCurrentNode();
        Flags.SetFlags(flags.allowedOffGrid, true);
        m_burromFrom = m_currentNode;
        m_currentNode = null;
    }

    protected virtual void Resurface()
    {
        AddToCurrentNode();
        Flags.SetFlags(flags.allowedOffGrid, false);
        spriteRenderer.enabled = true;
        m_burromFrom = null;
        transform.position = m_currentNode.position.world;
        ResurfacedThisStep = true;
    }
}