using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    private void Awake() {
        SoundManager.OnVolumeSettingUpdated += ConfigurateVolumeSetting;
    }

    private void Start(){
#if UNITY_EDITOR
        appVersionText.text = appVersionPrefix + Application.version; // only work in editor
#endif
    }

    private void OnDestroy() {
        SoundManager.OnVolumeSettingUpdated -= ConfigurateVolumeSetting;

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
        float musicVolume = PlayerPrefs.GetFloat(VolumeSettings.MIXER_MUSIC_VOLUME,1f);
        float sfxVolume = PlayerPrefs.GetFloat(VolumeSettings.MIXER_SFX_VOLUME,1f);

        musicSlider1.SetValueWithoutNotify(musicVolume);
        musicSlider2.SetValueWithoutNotify(musicVolume);

        sfxSlider1.SetValueWithoutNotify(sfxVolume);
        sfxSlider2.SetValueWithoutNotify(sfxVolume);
    }
}
