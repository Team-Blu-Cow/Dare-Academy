using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessVolumeController : MonoBehaviour
{
    [SerializeField] private Volume m_ppVolume;
    [SerializeField] private Vignette m_ppVignette;

    void OnValidate()
    {
        m_ppVolume = GetComponent<Volume>();
        m_ppVolume.profile.TryGet<Vignette>(out m_ppVignette);

        if (m_ppVignette != null)
            m_ppVignette.intensity.value = 0f;
    }

    void Awake()
    {
        if (m_ppVignette != null)
            m_ppVignette.intensity.value = 0f;
    }
}
