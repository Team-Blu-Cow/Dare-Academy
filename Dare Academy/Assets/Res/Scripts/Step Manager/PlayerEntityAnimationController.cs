using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    [SerializeField] private Volume m_ppVolume;
    [SerializeField] private Vignette m_ppVignette;
    [SerializeField] private ChromaticAberration m_ppChromaticAberration;
    [SerializeField] private LensDistortion m_ppLensDistortion;

    [SerializeField] private Color[] m_abilityColours;

    [SerializeField] private float m_vignetteIntensityVal = 0.4f;
    [SerializeField] private float m_currentVignetteIntensityVal = 0;
    private Color m_vignetteColour;

    [SerializeField] private float m_chromaticIntensityVal = 1;
    private float m_currentChromaticIntensityVal = 0;

    [SerializeField] private float m_lensIntensityVal = 0.2f;
    private float m_currentLensIntensityVal = 0;

    [SerializeField] private float m_lerpTime = 0.1f;

    bool m_vignetteExists = false;
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
        m_abilityMode = true;
        m_abilityState = abilityState;

        m_ppVignette.intensity.overrideState = abilityMode;
        m_ppVignette.intensity.value = m_vignetteIntensityVal;


        /*if (m_abilityMode)
            //m_ppVignette.intensity.value = m_vignetteIntensity;
            m_ppVignette.intensity.overrideState = m_abilityMode;
        else
            //m_ppVignette.intensity.value = 0;*/

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
        if (abilityState <= 0)
            return;



        LeanTween.value(gameObject, m_vignetteColour, m_abilityColours[abilityState], m_lerpTime)
            .setOnUpdateColor((value) =>
            {
                m_vignetteColour = value;
            });
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
        //m_ppVignette.intensity.overrideState = false;

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
