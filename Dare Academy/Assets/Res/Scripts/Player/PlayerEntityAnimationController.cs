using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using JUtil;
using blu;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    private PlayerEntity m_playerEntity;

    private Volume m_ppVolume;
    private Vignette m_ppVignette;
    private ChromaticAberration m_ppChromaticAberration;
    private LensDistortion m_ppLensDistortion;

    [SerializeField, HideInInspector] private ParticleSystem m_luvGunParticles;

    [Header("Vignette Settings")]
    [SerializeField] private Color[] m_abilityColours;

    [SerializeField] private float m_vignetteIntensityVal = 0.4f;
    private float m_currentVignetteIntensityVal = 0;
    private Color m_vignetteColour;

    [SerializeField] private float m_chromaticIntensityVal = 1;
    private float m_currentChromaticIntensityVal = 0;

    [SerializeField] private float m_lensIntensityVal = 0.2f;
    private float m_currentLensIntensityVal = 0;

    [SerializeField] private float m_lerpTime = 0.1f;

    [Header("Dash Settings")]
    [SerializeField] private float m_distanceBetweenImages;

    [SerializeField] private float m_imageActiveTime;
    private Vector3 m_lastImagePos;

    private bool m_abilityMode;
    private int m_abilityState;

    [SerializeField, HideInInspector] private Transform[] m_muzzlePositions;

    [SerializeField, HideInInspector] private GameObject m_LuvMuzzleFlashPrefab;

    protected override void OnValidate()
    {
        base.OnValidate();

        m_playerEntity = GetComponent<PlayerEntity>();

        m_LuvMuzzleFlashPrefab = Resources.Load<GameObject>("prefabs/GFX/LuvGunMuzzleFlash");

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

        if (m_playerEntity == null)
            m_playerEntity = GetComponent<PlayerEntity>();

        if (m_LuvMuzzleFlashPrefab == null)
            m_LuvMuzzleFlashPrefab = Resources.Load<GameObject>("prefabs/GFX/LuvGunMuzzleFlash");

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

        switch (m_abilityState)
        {
            case 1: // shoot
                if (m_playerEntity.m_abilityDirection == Vector2Int.zero)
                    StopLuvParticles();
                else if (m_playerEntity.Energy < m_playerEntity.m_shootEnergyCost)
                {
                    StopLuvParticles();
                    // #jay #TODO put sputter effect here
                }
                else
                {
                    if (m_abilityMode)
                        StartLuvParticles();
                }
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

        if (m_playerEntity.Energy < m_playerEntity.m_shootEnergyCost || m_playerEntity.m_abilityDirection == Vector2Int.zero)
        {
            StopLuvParticles();
            // #jay #TODO put sputter effect here
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
        {
            m_luvGunParticles.Play();

            blu.App.GetModule<blu.AudioModule>().GetAudioEvent("event:/SFX/Player/sfx_ability_select").SetParameter("selecting", 1);
        }
    }

    public void StopLuvParticles()
    {
        if (m_luvGunParticles == null)
            return;

        if (m_luvGunParticles.isPlaying)
        {
            m_luvGunParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            blu.App.GetModule<blu.AudioModule>().GetAudioEvent("event:/SFX/Player/sfx_ability_select").SetParameter("selecting", 0);
        }
    }

    public void CreateDashAfterImages()
    {
        StartCoroutine(DrawDash());
    }

    public void CreateAfterImage()
    {
        GameObject afterImage = PlayerAfterEffectPool.Instance.GetFromPool();
        afterImage.transform.position = transform.position + (Vector3.up * 0.01f);
        afterImage.GetComponent<PlayerEntityAfterEffect>().m_activeTime = m_imageActiveTime;
        m_lastImagePos = transform.position;
    }

    private IEnumerator DrawDash()
    {
        App.GetModule<AudioModule>().PlayAudioEvent("event:/SFX/Player/sfx_dash");
        CreateAfterImage();

        float dashTimeLeft = App.GetModule<LevelModule>().StepController.stepTime;

        while (dashTimeLeft > 0)
        {
            dashTimeLeft -= Time.deltaTime;

            if (Vector3.Distance(transform.position, m_lastImagePos) > m_distanceBetweenImages)
                CreateAfterImage();

            yield return null;
        }
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

    public void MuzzleFlash(Vector2Int direction)
    {
        int index = direction.RotationToIndex(90);

        //Instantiate(m_LuvMuzzleFlashPrefab, m_muzzlePositions[index].position, Quaternion.identity);
        Instantiate(m_LuvMuzzleFlashPrefab, m_muzzlePositions[index]);
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