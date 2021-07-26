using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    [SerializeField] private Volume m_ppVolume;
    [SerializeField] private Vignette m_ppVignette;

    [SerializeField] private Color[] m_abilityColours;

    [SerializeField] private float m_vignetteIntensity = 0.25f;

    bool m_vignetteExists = false;
    [SerializeField] bool m_abilityMode;

    protected override void OnValidate()
    {
        base.OnValidate();

        m_ppVolume = FindObjectOfType<Volume>();
        m_ppVolume.profile.TryGet<Vignette>(out m_ppVignette);
    }

    protected override void Start()
    {
        base.Start();

        m_vignetteExists = false;

        if (m_ppVignette != null)
            m_vignetteExists = true;
    }

    public void ResetFaceDirection()
    {
        animator.SetFloat("HeadDirX", animator.GetFloat("WalkDirX"));
        animator.SetFloat("HeadDirY", animator.GetFloat("WalkDirY"));
    }

    public void SetAbilityMode(bool abilityMode, int abilityState)
    {
        m_abilityMode = abilityMode;

        if (m_abilityMode)
            m_ppVignette.intensity.value = m_vignetteIntensity;
        else
            m_ppVignette.intensity.value = 0;

        SetAbilityState(abilityState);
    }

    public void SetAbilityState(int abilityState)
    {
        m_ppVignette.color.value = m_abilityColours[abilityState];
    }

}
