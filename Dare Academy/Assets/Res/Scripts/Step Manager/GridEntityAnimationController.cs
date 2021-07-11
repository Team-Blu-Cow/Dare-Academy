using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;
using JUtil;

[RequireComponent(typeof(Animator))]
public class GridEntityAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator m_animator;
    [SerializeField] protected GameObject m_sprite;
    [SerializeField] protected GameObject m_spriteHead;
    [SerializeField] protected SpriteRenderer m_spriteRendererHead;
    [SerializeField] protected bool m_hasHead = false;
    [SerializeField] protected List<Sprite> m_headSprites;

    float m_xScale = 1;

    public float animatorSpeed;

    protected virtual void OnValidate()
    {
        m_animator = GetComponent<Animator>();
        Transform[] ts = GetComponentsInChildren<Transform>();
        bool spriteExists = false;
        foreach (var t in ts)
        {
            if (t.name == "sprite")
            {
                spriteExists = true;
                m_sprite = t.gameObject;
                break;
            }
        }

        if(!spriteExists)
        {
            m_sprite = new GameObject("sprite");
            m_sprite.AddComponent<Transform>();
            m_sprite.AddComponent<SpriteRenderer>();

            m_sprite.transform.parent = this.transform;
            m_sprite.transform.position = Vector3.zero;
        }

        if(m_hasHead)
        {
            Transform[] hts = m_sprite.GetComponentsInChildren<Transform>();
            bool spriteHeadExists = false;
            foreach (var t in hts)
            {
                if (t.name == "head")
                {
                    spriteHeadExists = true;
                    m_spriteHead = t.gameObject;
                    break;
                }
            }

            if (!spriteHeadExists)
            {
                m_spriteHead = new GameObject("head");
                m_spriteHead.AddComponent<Transform>();
                m_spriteHead.AddComponent<SpriteRenderer>();

                m_spriteHead.transform.parent = this.transform;
                m_spriteHead.transform.position = Vector3.zero;

                m_spriteRendererHead = m_spriteHead.GetComponent<SpriteRenderer>();
            }
        }
    }

    protected void Start()
    {
        animatorSpeed =  1f/App.GetModule<LevelModule>().StepController.stepTime;
        m_animator.speed = animatorSpeed;
    }

    public virtual void PlayAnimation(string animationName, float time)
    {
        if (m_animator.runtimeAnimatorController == null)
            return;

        if (!m_animator.HasState(0, Animator.StringToHash(animationName)))
            return;
        m_animator.speed = 1f/time;

        m_animator.Play(animationName, -1, 0f);
    }

    public virtual void SetDirection(Vector2Int dir) => SetDirection(dir.x, dir.y);

    public virtual void SetDirection(float x, float y)
    {
        m_xScale = Mathf.Sign(x);
    }

    public virtual void SetHeadDirection(float x, float y)
    {
        if (m_hasHead == false || m_spriteHead == null || m_spriteRendererHead == null || m_headSprites == null || m_headSprites.Count < 3)
            return;

        Vector2 dir = new Vector2(x,y);

        int angle = dir.RotationToIndex(90);

        m_spriteRendererHead.sprite = m_headSprites[angle];
    }

    protected virtual void Update()
    {
        if (m_hasHead && m_spriteHead != null)
        {
            m_spriteHead.transform.localScale = new Vector3(
                transform.localScale.x * Mathf.Abs(m_spriteHead.transform.localScale.x),
                m_spriteHead.transform.localScale.y,
                m_spriteHead.transform.localScale.z
                );
        }

        transform.localScale = new Vector3(m_xScale * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
