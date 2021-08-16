using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using JUtil;
using blu;
using Cinemachine;

public class PlayerEntityAnimationController : GridEntityAnimationController
{
    private PlayerEntity m_playerEntity;

    private Volume m_ppVolume;
    private Vignette m_ppVignette;
    private ChromaticAberration m_ppChromaticAberration;
    private LensDistortion m_ppLensDistortion;

    [SerializeField, HideInInspector] private ParticleSystem m_luvGunParticles;
    [SerializeField, HideInInspector] private ParticleSystem[] m_luvGunSubParticles;

    [SerializeField, HideInInspector] private ParticleSystem m_dashChargeParticles;
    [SerializeField, HideInInspector] private ParticleSystem[] m_dashChargeSubParticles;
    [SerializeField, HideInInspector] private GameObject m_dashBurstPrefab;

    [SerializeField, HideInInspector] protected GameObject m_spriteBody;

    [Header("Vignette Settings")]
    [SerializeField] public Color[] m_abilityColours;

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

        Transform[] ts = GetComponentsInChildren<Transform>();
        foreach (var t in ts)
        {
            if (t.name == "body")
            {
                m_spriteBody = t.gameObject;
                break;
            }
        }

        m_playerEntity = GetComponent<PlayerEntity>();

        m_LuvMuzzleFlashPrefab = Resources.Load<GameObject>("prefabs/GFX/LuvGunMuzzleFlash");
        m_dashBurstPrefab = Resources.Load<GameObject>("prefabs/GFX/DashBurstFX");

        //m_ppVolume = FindObjectOfType<Volume>();
        //m_ppVolume.profile.TryGet<Vignette>(out m_ppVignette);

        m_sprite.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
        m_spriteHead.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
        m_spriteBody.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
    }

    public void endAllSounds()
    {
        StopLuvParticles();
        StopDashChargeParticles();
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
        SetParticleColours();
        StopLuvParticles();
        StopDashChargeParticles();

        m_sprite.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
        m_spriteHead.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
        m_spriteBody.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Strength", 0);
    }

    public void SetParticleColours()
    {
        foreach (var p in m_luvGunSubParticles)
        {
            ParticleSystem.MainModule settings = p.main;

            settings.startColor = m_abilityColours[1];
        }

        foreach (var p in m_dashChargeSubParticles)
        {
            ParticleSystem.MainModule settings = p.main;

            settings.startColor = m_abilityColours[2];
        }
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
                StopDashChargeParticles();
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
                if (m_playerEntity.m_abilityDirection == Vector2Int.zero)
                    StopDashChargeParticles();
                else if (m_playerEntity.Energy < m_playerEntity.m_shootEnergyCost)
                {
                    StopDashChargeParticles();
                }
                else
                {
                    if (m_abilityMode)
                        StartDashChargeParticles();
                }
                break;

            case 3: // shield
                StopLuvParticles();
                StopDashChargeParticles();
                break;

            default:
                StopLuvParticles();
                StopDashChargeParticles();
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

    public void StartDashChargeParticles()
    {
        if (m_dashChargeParticles == null)
            return;

        if (!m_dashChargeParticles.isPlaying)
        {
            m_dashChargeParticles.Play();
            blu.App.GetModule<blu.AudioModule>().GetAudioEvent("event:/SFX/Player/sfx_dash_chrage").SetParameter("selecting", 1);
        }
    }

    public void StopDashChargeParticles()
    {
        if (m_dashChargeParticles == null)
            return;

        if (m_dashChargeParticles.isPlaying)
        {
            m_dashChargeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            blu.App.GetModule<blu.AudioModule>().GetAudioEvent("event:/SFX/Player/sfx_dash_chrage").SetParameter("selecting", 0);
        }
    }

    public void StartDashEffect(Vector2Int direction)
    {
        DashBurst(direction);
        StartCoroutine(DrawDash());
    }

    public void DashBurst(Vector2Int direction)
    {
        int index = (-direction).RotationToIndex(90);

        //Instantiate(m_LuvMuzzleFlashPrefab, m_muzzlePositions[index].position, Quaternion.identity);
        blu.App.GetModule<blu.AudioModule>().PlayAudioEvent("event:/SFX/Player/sfx_dash_release");

        GameObject flashObj = Instantiate(m_dashBurstPrefab, m_muzzlePositions[index]);

        flashObj.GetComponent<ParticleSystem>().Stop();

        switch (index)
        {
            case 0: // north
                flashObj.transform.Rotate(new Vector3(0, 0, 180));
                break;

            case 1: // east
                flashObj.transform.Rotate(new Vector3(0, 0, 90));
                break;

            case 2: // south
                flashObj.transform.Rotate(new Vector3(0, 0, 0));
                break;

            case 3: // west
                flashObj.transform.Rotate(new Vector3(0, 0, 270));
                break;
        }

        ParticleSystem[] particles = flashObj.GetComponentsInChildren<ParticleSystem>();

        foreach (var p in particles)
        {
            ParticleSystem.MainModule settings = p.main;
            settings.startColor = m_abilityColours[2];
        }

        flashObj.GetComponent<ParticleSystem>().Play();
    }

    public void CreateAfterImage()
    {
        GameObject afterImage = PlayerAfterEffectPool.Instance.GetFromPool();
        afterImage.transform.position = transform.position + (Vector3.up * 0.01f);
        afterImage.GetComponent<PlayerEntityAfterEffect>().m_activeTime = m_imageActiveTime;
        afterImage.GetComponent<PlayerEntityAfterEffect>().m_colour = m_abilityColours[2];
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
        GameObject flashObj = Instantiate(m_LuvMuzzleFlashPrefab, m_muzzlePositions[index]);
        ParticleSystem[] particles = flashObj.GetComponentsInChildren<ParticleSystem>();

        foreach (var p in particles)
        {
            ParticleSystem.MainModule settings = p.main;
            settings.startColor = m_abilityColours[1];
        }
    }

    protected override void Update()
    {
        m_ppVignette.color.value = m_vignetteColour;
        m_ppVignette.intensity.value = m_currentVignetteIntensityVal;

        m_ppChromaticAberration.intensity.value = m_currentChromaticIntensityVal;

        m_ppLensDistortion.intensity.value = m_currentLensIntensityVal;

        base.Update();
    }

    public void SetBulletColour(GameObject bullet)
    {
        LuvBulletParticleList particleObj = bullet.GetComponentInChildren<LuvBulletParticleList>();

        ParticleSystem[] particles = particleObj.particles;

        foreach (var p in particles)
        {
            ParticleSystem.MainModule settings = p.main;
            settings.startColor = m_abilityColours[1];
        }
    }

    public override void DamageFlash()
    {
        LeanTween.value(gameObject, 0, 1, 0.5f)
            .setEasePunch()
            .setOnUpdate((float value) =>
            {
                m_sprite.GetComponent<SpriteRenderer>().material.SetFloat("_Strength", value);
                m_spriteHead.GetComponent<SpriteRenderer>().material.SetFloat("_Strength", value);
                m_spriteBody.GetComponent<SpriteRenderer>().material.SetFloat("_Strength", value);
            });
    }

    public void CameraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin camNoise = App.CameraController.virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (camNoise is null)
            return;

        camNoise.m_AmplitudeGain = intensity;

        LeanTween.value(intensity, 0, time)
            .setOnUpdate((float value) =>
            {
                camNoise.m_AmplitudeGain = value;
            })
            .setOnComplete(() =>
            {
                App.CameraController.transform.rotation = Quaternion.identity;
            });
    }

    private void OnDestroy()
    {
        endAllSounds();
    }
}
