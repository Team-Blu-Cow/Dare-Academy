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
    [SerializeField, HideInInspector] protected WyrmUIHealth m_uiHealth;
    [SerializeField, HideInInspector] protected Animator animator;

    [SerializeField] private GameObject m_burrowVFXPrefab; 

    public WyrmSection SectionInfront
    { get; set; }

    public WyrmSection SectionBehind
    { get; set; }

    public bool ResurfacedThisStep
    { get; protected set; }

    public GridNode BurrowFrom
    { get; protected set; }

    public GridNode ResurfaceFrom
    { get; protected set; }

    protected override void OnValidate()
    {
        base.OnValidate();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_uiHealth = FindObjectOfType<WyrmUIHealth>();
    }

    protected void Awake()
    {
        Debug.Assert(Flags.IsFlagsSet(flags.allowedOffGrid));
    }

    protected override void Start()
    {
        base.Start();
        m_uiHealth = FindObjectOfType<WyrmUIHealth>();
    }

    public override void OnHit(int damage, float offsetTime = 0)
    {
        Head.Health -= damage;
        m_uiHealth.Hit(Head.Health, Head);
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();
    }

    public override void PostMoveStep()
    {
        base.PostMoveStep();
        ResurfacedThisStep = false;
    }

    protected virtual void Burrow()
    {
        RemoveFromCurrentNode();
        BurrowFrom = m_currentNode;
        m_currentNode = null;
        spriteRenderer.enabled = false;
        Instantiate(m_burrowVFXPrefab, BurrowFrom.position.world - new Vector3(0, 0.4f, 0), Quaternion.identity);
    }

    protected virtual void Resurface()
    {
        AddToCurrentNode();
        ResurfaceFrom = currentNode;
        transform.position = m_currentNode.position.world;
        spriteRenderer.enabled = true;
        BurrowFrom = null;
        ResurfacedThisStep = true;
        Instantiate(m_burrowVFXPrefab, transform.position-new Vector3(0,0.4f, 0), Quaternion.identity);
    }

    private void CallAnimateCharge(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(AnimateCharge(startPos, endPos, 1));
    }

    protected IEnumerator AnimateCharge(Vector3 startPos, Vector3 endPos, float length)
    {
        

        float animationTime = m_stepController.stepTime*2f;
        float currentTime = 0;

        //if (wait)
        //{
         //   yield return new WaitForSeconds(((animationTime / 12f) / 12f)/12f);
        //}

        bool nextHasStarted = false;

        Vector3 dir = (endPos - startPos).normalized;

        endPos = endPos + (dir * length);

        while (true)
        {
            spriteRenderer.enabled = true;
            currentTime += Time.deltaTime;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, currentTime / animationTime);
            transform.position = currentPos;
            yield return null;

            Vector3 diff = currentPos - startPos;

            WyrmSection currentSection = SectionBehind;
            Vector3 sectionPos = currentPos;

            diff = diff.normalized;

            while(currentSection != null)
            {
                currentSection.spriteRenderer.enabled = true;
                sectionPos = sectionPos - diff;
                currentSection.transform.position = sectionPos;
                currentSection = currentSection.SectionBehind;
            }
            

            if (currentTime > animationTime)
                break;
        }

        spriteRenderer.enabled = false;
        {
            WyrmSection currentSection = SectionBehind;
            while (currentSection != null)
            {
                currentSection.spriteRenderer.enabled = true;
                currentSection = currentSection.SectionBehind;
            }
        }
    }

    protected void SetAnimationFlags(Vector3 start_pos, Vector3 end_pos)
    {
        Vector3 diff = end_pos - start_pos;

        if (diff.magnitude < 0.7)
        {
            return;
            // ZeroAnimationFlags();
        }

        //Quaternion rot = Quaternion.identity;

        if (diff.x > 0.7)
        {
            animator.SetBool("movingUp", false);
            animator.SetBool("movingDown", false);
            animator.SetBool("movingLeft", false);
            animator.SetBool("movingRight", true);
            //rot = Quaternion.Euler(0, 180, 0);
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

        //transform.rotation = rot;
    }
}