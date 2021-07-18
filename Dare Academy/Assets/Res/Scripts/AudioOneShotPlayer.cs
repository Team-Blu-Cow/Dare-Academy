using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOneShotPlayer : MonoBehaviour
{
    public void Play(string in_string)
    {
        FMODUnity.RuntimeManager.PlayOneShot(in_string);
    }
}