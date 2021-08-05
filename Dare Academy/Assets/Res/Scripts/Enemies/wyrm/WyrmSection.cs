using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

public abstract class WyrmSection : GridEntity
{
    private WyrmSection m_sectionInfront;
    private WyrmSection m_sectionBehind;

    [SerializeField, HideInInspector] protected SpriteRenderer spriteRenderer;

    public WyrmSection SectionInfront
    { get; set; }

    public WyrmSection SectionBehind
    { get; set; }

    public bool ResurfacedThisStep
    { get; protected set; }

    public GridNode BurrowFrom
    { get; protected set; }

    protected override void OnValidate()
    {
        base.OnValidate();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Awake()
    {
        Debug.Assert(Flags.IsFlagsSet(flags.allowedOffGrid));
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();
    }

    protected virtual void Burrow()
    {
        RemoveFromCurrentNode();
        BurrowFrom = m_currentNode;
        m_currentNode = null;
    }

    protected virtual void Resurface()
    {
        AddToCurrentNode();
        spriteRenderer.enabled = true;
        BurrowFrom = null;
        transform.position = m_currentNode.position.world;
        ResurfacedThisStep = true;
    }
}