using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityAfterEffect : MonoBehaviour
{
    private Transform m_playerTransform;

    [SerializeField, HideInInspector] private SpriteRenderer m_headRenderer;
    [SerializeField, HideInInspector] private SpriteRenderer m_bodyRenderer;
    [SerializeField, HideInInspector] private SpriteRenderer m_legsRenderer;

    [SerializeField] private SpriteRenderer m_srcHeadRenderer;
    [SerializeField] private SpriteRenderer m_srcBodyRenderer;
    [SerializeField] private SpriteRenderer m_srcLegsRenderer;

    private Color m_colour;

    [SerializeField] public float m_activeTime = 0.1f;
    private float m_timeActivated;

    private float m_alpha;
    [SerializeField] private float m_startAlpha = 0.8f;
    [SerializeField] private float m_alphaMultiplier = 0.85f;

    private void OnEnable()
    {
        m_playerTransform = PlayerEntity.Instance.transform;

        GetPlayerSpriteRenderers();

        m_alpha = m_startAlpha;

        m_headRenderer.sprite = m_srcHeadRenderer.sprite;
        m_bodyRenderer.sprite = m_srcBodyRenderer.sprite;
        m_legsRenderer.sprite = m_srcLegsRenderer.sprite;

        transform.position = m_playerTransform.position;

        m_timeActivated = Time.time;
    }

    private void GetPlayerSpriteRenderers()
    {
        SpriteRenderer[] srs = m_playerTransform.GetComponentsInChildren<SpriteRenderer>();
        bool headExists = false;
        bool bodyExists = false;
        bool legsExists = false;

        foreach(var sr in srs)
        {
            switch (sr.name)
            {
                case "head":
                    m_srcHeadRenderer = sr;
                    headExists = true;
                    break;
                case "body":
                    m_srcBodyRenderer = sr;
                    bodyExists = true;
                    break;
                case "sprite":
                    m_srcLegsRenderer = sr;
                    legsExists = true;
                    break;
            }
        }
        if (!(headExists && bodyExists && legsExists))
            Debug.LogError("[PlayerEntityAfterEffect.cs] <" + transform.name + "> : Player SpriteRenderers could not be found");
    }

    private void Update()
    {
        m_alpha *= m_alphaMultiplier;

        m_colour = new Color(1f, 1f, 1f, m_alpha);

        m_headRenderer.color = m_colour;
        m_bodyRenderer.color = m_colour;
        m_legsRenderer.color = m_colour;

        if(Time.time >= m_timeActivated+m_activeTime)
        {
            PlayerAfterEffectPool.Instance.AddToPool(gameObject);
        }
    }
}
