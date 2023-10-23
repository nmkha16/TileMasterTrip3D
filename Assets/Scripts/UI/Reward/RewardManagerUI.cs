using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManagerUI : MonoBehaviour
{
    [Header("Gold Reward")]
    [SerializeField] private Slider goldRewardSlider;
    [SerializeField] private TMP_Text goldRewardProgressText;

    [Header("Skill Reward")]
    [SerializeField] private Slider skillRewardSlider;
    [SerializeField] private TMP_Text skillRewardProgressText;

    [Header("Reward Box")]
    [SerializeField] private Animator goldRewardBoxAnimator;
    [SerializeField] private Animator skillRewardBoxAnimator;
    public RewardBoxPointer goldRewardBoxPointer;
    public RewardBoxPointer skillRewardBoxPointer;
    
    [Header("Open reward Box")]
    [SerializeField] private GameObject openGoldRewardBox;
    [SerializeField] private GameObject openSkillRewardBox;

    public void UpdateGoldRewardProgress(int current, int maxBound){
        goldRewardSlider.value = current/(float)maxBound;
        goldRewardProgressText.text = current + "/" + maxBound;
    }

    public void UpdateSkillRewardProgress(int current, int maxBound){
        skillRewardSlider.value = current/(float)maxBound;
        skillRewardProgressText.text = current + "/" + maxBound;
    }

    public void EnableGoldRewardToClaim(){
        goldRewardBoxAnimator.enabled = true;
        goldRewardBoxPointer.enabled = true;
    }

    public void EnabledSkillRewardToClaim(){
        // play anim box
        skillRewardBoxAnimator.enabled = true;
        skillRewardBoxPointer.enabled = true;
    }

    public void OpenGoldRewardBox(){
        goldRewardBoxAnimator.gameObject.SetActive(false);
        openGoldRewardBox.SetActive(true);
    }

    public void OpenSkillRewardBox(){
        skillRewardBoxPointer.gameObject.SetActive(false);
        openSkillRewardBox.SetActive(true);
    }

    public async void CloseGoldRewardBox(int milliseconds){
        await Task.Delay(milliseconds);
        goldRewardBoxPointer.enabled = false;
        goldRewardBoxAnimator.gameObject.SetActive(true);
        goldRewardBoxAnimator.enabled = false;
        openGoldRewardBox.SetActive(false);
    }

    public async void CloseSkillRewardBox(int milliseconds){
        await Task.Delay(milliseconds);
        skillRewardBoxPointer.enabled = false;
        skillRewardBoxAnimator.gameObject.SetActive(true);
        skillRewardBoxAnimator.enabled = false;
        openSkillRewardBox.SetActive(false);
    }
}
