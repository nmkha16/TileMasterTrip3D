using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // store sound to instant find said audio clip in list instead of searching
    private Dictionary<SoundId, AudioClip> soundDict = new();

    [Header("Audio Data")]
    public AudioDataScriptableObject audioData;

    [Header("Volume Setting")]
    private VolumeSettings setting;

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

        setting = new(audioMixer);
    }

    private void Start(){
        foreach(var clip in audioData.musics){
            soundDict.Add(clip.id, clip.audioClip);
        }
        foreach(var clip in audioData.sfxs){
            soundDict.Add(clip.id,clip.audioClip);
        }
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

    public async void PlayOneShotSound(SoundId id, int miliseconds){
        await Task.Delay(miliseconds);
        if (soundDict.TryGetValue(id,out var clip)){
            if (IsSFX(id)){
                sfxSource.PlayOneShot(clip);
            }
            else{
                musicSource.PlayOneShot(clip);
            }
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

        if (Enum.TryParse<SoundId>(nextSongId,true, out var result)){
            PlayMusic(result);
        }
    }

    private bool IsBattleSong(SoundId id){
        return (int)id >= (int)SoundId.m_battle_1;
    }
}
