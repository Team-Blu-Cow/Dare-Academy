using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    private Volume m_ppVolume;
    private Vignette m_ppVignette;
    private ChromaticAberration m_ppChromaticAberration;
    private LensDistortion m_ppLensDistortion;

    [SerializeField] private ParticleSystem m_luvGunParticles;

    [SerializeField] private Color[] m_abilityColours;

    [SerializeField] private float m_vignetteIntensityVal = 0.4f;
    private float m_currentVignetteIntensityVal = 0;
    private Color m_vignetteColour;

    [SerializeField] private float m_chromaticIntensityVal = 1;
    private float m_currentChromaticIntensityVal = 0;

    [SerializeField] private float m_lensIntensityVal = 0.2f;
    private float m_currentLensIntensityVal = 0;

    [SerializeField] private float m_lerpTime = 0.1f;

    bool m_abilityMode;
    int m_abilityState;

    protected override void OnValidate()
    {
        base.OnValidate();

        //m_ppVolume = FindObjectOfType<Volume>();
        //m_ppVolume.profile.TryGet<Vignette>(out m_ppVignette);
    }

    protected override void Start()
    {
        base.Start();

        m_vignetteColour = Color.black;

        m_ppVolume = GetComponentInChildren<Volume>();
        m_ppVolume.profile.TryGet<Vignette>(out m_ppVignette);
        m_ppVolume.profile.TryGet<ChromaticAberration>(out m_ppChromaticAberration);
        m_ppVolume.profile.TryGet<LensDistortion>(out m_ppLensDistortion);

        StopLuvParticles();
    }

    public void ResetFaceDirection()
    {
        animator.SetFloat("HeadDirX", animator.GetFloat("WalkDirX"));
        animator.SetFloat("HeadDirY", animator.GetFloat("WalkDirY"));
    }

    public void SetAbilityMode(bool abilityMode, int abilityState)
    {
        m_abilityMode = true;
        m_abilityState = abilityState;

        SetAbilityState(abilityState);

        Tween(m_currentVignetteIntensityVal, m_vignetteIntensityVal, m_lerpTime, (value) =>
        {
            m_currentVignetteIntensityVal = value;
        });

        Tween(m_currentChromaticIntensityVal, m_chromaticIntensityVal, m_lerpTime, (value) =>
        {
            m_currentChromaticIntensityVal = value;
        });

        Tween(m_currentLensIntensityVal, m_lensIntensityVal, m_lerpTime, (value) =>
        {
            m_currentLensIntensityVal = value;
        });
    }

    public void SetAbilityState(int abilityState)
    {
        m_abilityState = abilityState;

        if (m_abilityMode)
        {
            switch (m_abilityState)
            {
                case 1: // shoot
                    StartLuvParticles();
                    break;

                case 2: // dash
                    StopLuvParticles();
                    break;

                case 3: // shield
                    StopLuvParticles();
                    break;

                default:
                    StopLuvParticles();
                    break;
            }
        }

        LeanTween.value(gameObject, m_vignetteColour, m_abilityColours[abilityState], m_lerpTime)
            .setOnUpdateColor((value) =>
            {
                m_vignetteColour = value;
            });
    }

    public void StartLuvParticles()
    {
        if (m_luvGunParticles == null)
            return;

        if (!m_luvGunParticles.isPlaying)
            m_luvGunParticles.Play();
    }

    public void StopLuvParticles()
    {
        if (m_luvGunParticles == null)
            return;

        if (m_luvGunParticles.isPlaying)
            m_luvGunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Tween(float tweenVal, float targetVal, float time, System.Action<float> setMethod)
    {
        LeanTween.value(tweenVal, targetVal, time)
            .setOnUpdate(setMethod);
    }

    public void DisableVignette()
    {
        m_abilityMode = false;
        m_ppVignette.intensity.value = 0;

        Tween(m_currentVignetteIntensityVal, 0, m_lerpTime, (value) =>
        {
            m_currentVignetteIntensityVal = value;
        });

        Tween(m_currentChromaticIntensityVal, 0, m_lerpTime, (value) =>
        {
            m_currentChromaticIntensityVal = value;
        });

        Tween(m_currentLensIntensityVal, 0, m_lerpTime, (value) =>
        {
            m_currentLensIntensityVal = value;
        });
    }

    protected override void Update()
    {
        m_ppVignette.color.value = m_vignetteColour;
        m_ppVignette.intensity.value = m_currentVignetteIntensityVal;

        m_ppChromaticAberration.intensity.value = m_currentChromaticIntensityVal;

        m_ppLensDistortion.intensity.value = m_currentLensIntensityVal;

        base.Update();
    }

}
