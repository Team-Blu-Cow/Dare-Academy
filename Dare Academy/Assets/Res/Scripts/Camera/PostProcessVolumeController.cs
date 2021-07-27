using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessVolumeController : MonoBehaviour
{
    [SerializeField] private Volume m_ppVolume;
    [SerializeField] private Vignette m_ppVignette;
    [SerializeField] private ChromaticAberration m_ppChromaticAberration;
    [SerializeField] private LensDistortion m_ppLensDistortion;
    [SerializeField] private float m_vignetteIntensityVal = 0.4f;

    void OnValidate()
    {
        m_ppVolume = GetComponent<Volume>();
        m_ppVolume.profile.TryGet<Vignette>(out m_ppVignette);
        m_ppVolume.profile.TryGet<ChromaticAberration>(out m_ppChromaticAberration);
        m_ppVolume.profile.TryGet<LensDistortion>(out m_ppLensDistortion);

        if (m_ppVignette != null)
        {
            m_ppVignette.intensity.value = m_vignetteIntensityVal;
            m_ppVignette.intensity.overrideState = false;
        }

        if(m_ppChromaticAberration != null)
        {
            m_ppChromaticAberration.intensity.value = m_vignetteIntensityVal;
        }

        if(m_ppLensDistortion != null)
        {
            m_ppLensDistortion.intensity.value = 0;
        }
    }

    void Awake()
    {
        if (m_ppVignette != null)
        {
            m_ppVignette.intensity.value = m_vignetteIntensityVal;
            //m_ppVignette.intensity.overrideState = false;
        }

        if (m_ppChromaticAberration != null)
        {
            m_ppChromaticAberration.intensity.value = 0;
        }

        if (m_ppLensDistortion != null)
        {
            m_ppLensDistortion.intensity.value = 0;
        }
    }

    private void Update()
    {
        Destroy(this);
    }
}
