using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataManagerUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private TMP_Text starText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text playCanvasHeader;
    [SerializeField] private TMP_Text mainMenuLevelNumberText;

    [Header("Ingame")]
    [SerializeField] private TMP_Text ingameStarsText;

    [Header("Ingame Win")]
    [SerializeField] private TMP_Text ingameWinStarsText;

    [Header("Game Version")]
    [SerializeField] private TMP_Text appVersionText;
    private readonly string appVersionPrefix = "Application version: v";

    [Header("Volume Setting")]
    [SerializeField] private Slider musicSlider1;
    [SerializeField] private Slider musicSlider2;
    [SerializeField] private Slider sfxSlider1;
    [SerializeField] private Slider sfxSlider2;

    [Header("Skill")]
    [SerializeField] private TMP_Text undoSkillRemainingText;
    [SerializeField] private TMP_Text nukeSkillRemainingText;

    private void Start(){
        appVersionText.text = appVersionPrefix + Application.version;
        ConfigurateVolumeSetting();
    }

    private void OnDestroy() {
        musicSlider1.onValueChanged.RemoveAllListeners();
        musicSlider2.onValueChanged.RemoveAllListeners();

        sfxSlider1.onValueChanged.RemoveAllListeners();
        sfxSlider2.onValueChanged.RemoveAllListeners();
    }

    public void UpdateStatUI(UserData userData){
        starText.text = userData.star.ToString();
        goldText.text = userData.gold.ToString();
        mainMenuLevelNumberText.text = (userData.level + 1).ToString();
        playCanvasHeader.text = "Level " + (userData.level + 1).ToString();
    }

    public void UpdateIngameStar(int stars){
        ingameStarsText.text = stars.ToString();
        ingameWinStarsText.text = "+" + stars.ToString();
    }

    public void UpdateRemainingSkill(UserData userData){
        undoSkillRemainingText.text = userData.undo.ToString();
        nukeSkillRemainingText.text = userData.tacNuke.ToString();
    }

    private void ConfigurateVolumeSetting(){
        var setting = SoundManager.Instance.setting;
        // set slider value on first load
        var musicVolume = PlayerPrefs.GetFloat(setting.MIXER_MUSIC_VOLUME,1f);
        var sfxVolume = PlayerPrefs.GetFloat(setting.MIXER_SFX_VOLUME,1f);

        musicSlider1.value = musicVolume;
        musicSlider2.value = musicVolume;

        sfxSlider1.value = sfxVolume;
        sfxSlider2.value = sfxVolume;

        // bind slider to volume settings
        musicSlider1.onValueChanged.AddListener(setting.SetMusicVolume);
        musicSlider2.onValueChanged.AddListener(setting.SetMusicVolume);

        sfxSlider1.onValueChanged.AddListener(setting.SetSFXVolume);
        sfxSlider1.onValueChanged.AddListener(setting.SetSFXVolume);
    }
}
