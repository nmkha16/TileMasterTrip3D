using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings
{
    private AudioMixer audioMixer;
    public readonly string MIXER_MUSIC_VOLUME = "MusicVolume";
    public readonly string MIXER_SFX_VOLUME = "SFXVolume";

    public VolumeSettings(AudioMixer mixer){
        this.audioMixer = mixer;
        LoadVolume();
    }

    public void SetMusicVolume(float value){
        audioMixer.SetFloat(MIXER_MUSIC_VOLUME, value == 0 ? -80f : Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MIXER_MUSIC_VOLUME,value);
    }

    public void SetSFXVolume(float value){
        audioMixer.SetFloat(MIXER_SFX_VOLUME, value == 0 ? -80f : Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MIXER_SFX_VOLUME,value);
    }

    private void LoadVolume(){
        var musicVolume = PlayerPrefs.GetFloat(MIXER_MUSIC_VOLUME,1f);
        var sfxVolume = PlayerPrefs.GetFloat(MIXER_SFX_VOLUME,1f);
        
        audioMixer.SetFloat(MIXER_MUSIC_VOLUME, musicVolume == 0 ? -80f : Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat(MIXER_SFX_VOLUME, sfxVolume == 0 ? -80f : Mathf.Log10(sfxVolume) * 20);
    }
}
