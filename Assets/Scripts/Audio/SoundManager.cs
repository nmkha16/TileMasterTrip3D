using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static Action OnVolumeSettingUpdated;
    public static SoundManager Instance;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // store sound to instant find said audio clip in list instead of searching
    private Dictionary<SoundId, AudioClip> soundDict = new();

    [Header("Audio Data")]
    public AudioDataScriptableObject audioData;

    [Header("Volume Setting")]
    [HideInInspector] public VolumeSettings setting;

    [Header("Songs")]
    public int totalMenuSong = 3;
    public int totalBattleSong = 3;
    
    private SoundId currentSongId;
    private float currentSongDuration = 1000f;    

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start(){
        setting = new(audioMixer);
        // construct sound dictionary
        foreach(var clip in audioData.musics){
            soundDict.Add(clip.id, clip.audioClip);
        }
        foreach(var clip in audioData.sfxs){
            soundDict.Add(clip.id,clip.audioClip);
        }
        OnVolumeSettingUpdated?.Invoke();
        // select a random menu music to play
        PlayRandomMenuMusic();
    }

    private void Update(){
        currentSongDuration -= Time.deltaTime;
        if (currentSongDuration < 0){
            NextSong();
        }
    }
    
    private void OnDestroy(){
        soundDict.Clear();

    }
    public async void PlayOneShotSound(SoundId id, int miliseconds = 0){
        await Task.Delay(miliseconds);
        if (soundDict.TryGetValue(id,out var clip)){
            if (IsSFX(id)){
                sfxSource.PlayOneShot(clip);
            }
            else{
                musicSource.PlayOneShot(clip);
            }
        }
        else{
            Debug.LogWarning("No sound clip found for " + id.ToString() +".\nPlease check your scene setup.");
        }
    }

    public void PlayMusic(SoundId id){
        if (IsSFX(id)){
            Debug.LogWarning("You are trying to call play sfx using play music function.\nPlease check your setup.");
            return;
        }

        if (soundDict.TryGetValue(id, out var clip)){
            musicSource.clip = clip;
            currentSongId = id;
            currentSongDuration = clip.length;
            musicSource.Play();
        }
    }

    private bool IsSFX(SoundId id){
        return (int)id < (int)SoundId.m_menu_1;
    }

    private void NextSong(){
        var id = new string(currentSongId.ToString().Where(char.IsDigit).ToArray());
        int intID = int.Parse(id);
        string nextSongId;

        if (IsBattleSong(currentSongId)){
            intID = (intID + totalBattleSong) % totalBattleSong + 1;
            nextSongId = "m_battle_";
            nextSongId += intID.ToString();
        }
        else{
            intID = (intID + totalMenuSong) % totalMenuSong + 1;
            nextSongId = "m_menu_";
            nextSongId += intID.ToString();

        }

        ParseSongIdAndPlay(nextSongId);
    }

    private bool IsBattleSong(SoundId id){
        return (int)id >= (int)SoundId.m_battle_1;
    }

    public void PlayRandomMenuMusic(){
        var id = UnityEngine.Random.Range(1,4);
        string nextSongId;

        id = (id + totalMenuSong) % totalMenuSong + 1;
        nextSongId = "m_menu_";
        nextSongId += id.ToString();

        ParseSongIdAndPlay(nextSongId);
    }

    public void PlayRandomBattleMusic(){
        var id = UnityEngine.Random.Range(1,4);
        string nextSongId;

        id = (id + totalMenuSong) % totalMenuSong + 1;
        nextSongId = "m_battle_";
        nextSongId += id.ToString();

        ParseSongIdAndPlay(nextSongId);
    }

    private void ParseSongIdAndPlay(string songId){
        if (Enum.TryParse<SoundId>(songId,true, out var result)){
            PlayMusic(result);
        }
        else{
            Debug.LogWarning("Error trying to parse sound id.\nPlease check your string or enum.");
        }
    }

    public void SetMusicVolume(float value){
        setting.SetMusicVolume(value);
        OnVolumeSettingUpdated?.Invoke();
    }

    public void SetSFXVolume(float value){
        setting.SetSFXVolume(value);
        OnVolumeSettingUpdated?.Invoke();
    }
}
