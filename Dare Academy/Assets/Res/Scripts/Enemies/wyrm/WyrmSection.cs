using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using flags = GridEntityFlags.Flags;
using interalFlags = GridEntityInternalFlags.Flags;

public abstract class WyrmSection : GridEntity
{
    private WyrmSection m_sectionInfront;
    private WyrmSection m_sectionBehind;

    public WyrmHead Head
    { get; set; }

    [SerializeField, HideInInspector] protected SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector] protected Animator animator;

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
        animator = GetComponent<Animator>();
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

    public override void OnHit(int damage, float offsetTime = 0)
    {
        Head.Health -= damage;
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

    private void CallAnimateCharge(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(AnimateCharge(startPos, endPos));
    }

    protected IEnumerator AnimateCharge(Vector3 startPos, Vector3 endPos)
    {
        float animationTime = m_stepController.stepTime * 2.5f;
        float currentTime = 0;

        bool nextHasStarted = false;

        while (true)
        {
            spriteRenderer.enabled = true;
            currentTime += Time.deltaTime;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, currentTime / animationTime);
            transform.position = currentPos;
            yield return null;

            if (!nextHasStarted)
            {
                Vector3 diff = currentPos - startPos;

                if (diff.magnitude > 1)
                {
                    if (SectionBehind)
                    {
                        SectionBehind.CallAnimateCharge(startPos, endPos);
                    }
                    nextHasStarted = true;
                }
            }

            if (currentTime > animationTime)
                break;
        }

        spriteRenderer.enabled = false;
    }

    protected void SetAnimationFlags(Vector3 start_pos, Vector3 end_pos)
    {
        Vector3 diff = end_pos - start_pos;

        if (diff.magnitude < 0.7)
        {
            return;
            // ZeroAnimationFlags();
        }

        Quaternion rot = Quaternion.identity;

        if (diff.x > 0.7)
        {
            animator.SetBool("movingUp", false);
            animator.SetBool("movingDown", false);
            animator.SetBool("movingLeft", false);
            animator.SetBool("movingRight", true);
            rot = Quaternion.Euler(0, 180, 0);
        }
        if (diff.x < -0.7)
        {
            animator.SetBool("movingUp", false);
            animator.SetBool("movingDown", false);
            animator.SetBool("movingLeft", true);
            animator.SetBool("movingRight", false);
        }
        if (diff.y > 0.7)
        {
            animator.SetBool("movingUp", true);
            animator.SetBool("movingDown", false);
            animator.SetBool("movingLeft", false);
            animator.SetBool("movingRight", false);
        }
        if (diff.y < -0.7)
        {
            animator.SetBool("movingUp", false);
            animator.SetBool("movingDown", true);
            animator.SetBool("movingLeft", false);
            animator.SetBool("movingRight", false);
        }

        transform.rotation = rot;
    }
}