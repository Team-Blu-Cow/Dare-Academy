using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorblindLookup : MonoBehaviour
{
    public bool FilterEnabled = false;

    private enum ColorblindType
    {
        Anomalous_Deuteranomaly,
        Anomalous_Protanomaly,
        Anomalous_Tritanomaly,
        Dichromatic_Deuteranopia,
        Dichromatic_Protanopia,
        Dichromatic_Tritanopia,
        Monochromatic_Achromatopsia,
        Monochromatic_BlueCone
    }

    public Volume _postProcessVolume;
    private ColorLookup _lookup;

    [SerializeField] private TextureParameter[] textures;
    [SerializeField] private ColorblindType _currentType = ColorblindType.Anomalous_Deuteranomaly;

    private void Start()
    {
        _postProcessVolume.profile.TryGet(out _lookup);
    }

    private void Update()
    {
        _lookup.active = FilterEnabled;

        if (FilterEnabled)
        {
            switch (_currentType)
            {
                case ColorblindType.Anomalous_Deuteranomaly:
                    _lookup.texture.value = textures[0].value;
                    break;

                case ColorblindType.Anomalous_Protanomaly:
                    _lookup.texture.value = textures[1].value;
                    break;

                case ColorblindType.Anomalous_Tritanomaly:
                    _lookup.texture.value = textures[2].value;
                    break;

                case ColorblindType.Dichromatic_Deuteranopia:
                    _lookup.texture.value = textures[3].value;
                    break;

                case ColorblindType.Dichromatic_Protanopia:
                    _lookup.texture.value = textures[4].value;
                    break;

                case ColorblindType.Dichromatic_Tritanopia:
                    _lookup.texture.value = textures[5].value;
                    break;

                case ColorblindType.Monochromatic_Achromatopsia:
                    _lookup.texture.value = textures[6].value;
                    break;

                case ColorblindType.Monochromatic_BlueCone:
                    _lookup.texture.value = textures[7].value;
                    break;
            }
        }
    }
}