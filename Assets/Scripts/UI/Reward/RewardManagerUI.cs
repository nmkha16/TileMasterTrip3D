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

    public void ToggleGoldRewardToClaim(bool toggle){
        goldRewardBoxAnimator.enabled = toggle;
        goldRewardBoxPointer.enabled = toggle;
    }

    public void ToggleSkillRewardToClaim(bool toggle){
        skillRewardBoxAnimator.enabled = toggle;
        skillRewardBoxPointer.enabled = toggle;
    }

    public void OpenGoldRewardBox(){
        goldRewardBoxAnimator.enabled = false;
        goldRewardBoxAnimator.gameObject.SetActive(false);
        goldRewardBoxPointer.enabled = false;
        openGoldRewardBox.SetActive(true);
    }

    public void OpenSkillRewardBox(){
        skillRewardBoxAnimator.enabled = false;
        skillRewardBoxPointer.gameObject.SetActive(false);
        skillRewardBoxPointer.enabled = false;
        openSkillRewardBox.SetActive(true);
    }

    public async void CloseGoldRewardBox(int milliseconds){
        //goldRewardBoxAnimator.enabled = false;
        await Task.Delay(milliseconds);
        goldRewardBoxAnimator.gameObject.SetActive(true);
        openGoldRewardBox.SetActive(false);
    }

    public async void CloseSkillRewardBox(int milliseconds){
        //skillRewardBoxAnimator.enabled = false;
        await Task.Delay(milliseconds);
        skillRewardBoxAnimator.gameObject.SetActive(true);
        openSkillRewardBox.SetActive(false);
    }
}
