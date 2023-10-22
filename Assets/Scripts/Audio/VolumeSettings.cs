using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings
{
    public Action<float> OnSfxVolumeChanged;
    public Action<float> OnMusicVolumeChanged;
    private AudioMixer audioMixer;

    public VolumeSettings(AudioMixer mixer){
        this.audioMixer = mixer;
    }
}
