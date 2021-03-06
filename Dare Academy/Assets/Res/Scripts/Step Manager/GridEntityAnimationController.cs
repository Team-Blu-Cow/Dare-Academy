using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using blu;
using JUtil;

[RequireComponent(typeof(Animator))]
public class GridEntityAnimationController : MonoBehaviour
{
    [SerializeField, HideInInspector] protected Animator m_animator;
    [SerializeField, HideInInspector] protected GameObject m_sprite;
    [SerializeField, HideInInspector] protected GameObject m_spriteHead;
    [SerializeField] protected bool m_hasHead = false;

    public Animator animator
    { get { return m_animator; } }

    float m_xScale = 1;
    float animatorSpeed;

    [SerializeField] public bool m_overwriteAnimSpeed = true;
    [HideInInspector] public bool m_isDead = false;

    [SerializeField] protected GameObject m_deathPoofPrefab;

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

        if (!spriteExists)
        {
            m_sprite = new GameObject("sprite");
            m_sprite.AddComponent<Transform>();
            m_sprite.AddComponent<SpriteRenderer>();

            m_sprite.transform.parent = this.transform;
            m_sprite.transform.localPosition = Vector3.zero;
        }

        if (m_hasHead)
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
                m_spriteHead.transform.localPosition = Vector3.zero;
            }
        }

        if(m_sprite)
            if(m_sprite.GetComponent<SpriteRenderer>().sharedMaterial)
                m_sprite.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
        if (m_hasHead)
            m_spriteHead.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);

        m_deathPoofPrefab = Resources.Load<GameObject>("prefabs/GFX/DeathPoof");
    }

    protected virtual void Start()
    {
        if (m_overwriteAnimSpeed)
        {
            animatorSpeed = 1f / App.GetModule<LevelModule>().StepController.stepTime;
            m_animator.speed = animatorSpeed;
        }

        m_sprite.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
        if (m_hasHead)
            m_spriteHead.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
    }

    public void SetAnimationSpeed(float speed)
    {
        animatorSpeed = speed;
        m_animator.speed = speed;
    }

    public virtual void PlayAnimation(string animationName, float time, int layer = -1)
    {
        if (m_animator.runtimeAnimatorController == null)
            return;

        if (m_overwriteAnimSpeed)
            m_animator.speed = 1f / time;

        m_animator.Play(animationName, layer, 0f);
    }

    public virtual void SetDirection(Vector2Int dir) => SetDirection(dir.x, dir.y);

    public virtual void SetDirection(float x, float y)
    {
        m_xScale = Mathf.Sign(x);
    }

    public virtual void SetHeadDirection(float x, float y)
    {
        if (m_hasHead == false || m_animator == null || m_animator.runtimeAnimatorController == null)
            return;

        m_animator.SetFloat("HeadDirX", x);
        m_animator.SetFloat("HeadDirY", y);
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

    public virtual void DamageFlash()
    {
        LeanTween.value(gameObject, 0, 1, 0.5f)
            .setEasePunch()
            .setOnUpdate((float value) => 
            {
                m_sprite.GetComponent<SpriteRenderer>().material.SetFloat("_Strength", value);
                if(m_hasHead)
                    m_spriteHead.GetComponent<SpriteRenderer>().material.SetFloat("_Strength", value);
            });
    }

    public void SpawnDeathPoof(Vector3 position)
    {
        Instantiate(m_deathPoofPrefab, position - new Vector3(0,0.5f,0), Quaternion.identity);
    }
}
