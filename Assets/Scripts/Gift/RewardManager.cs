using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// formula for level system is XP = (Level/x)^y / Stars = (Level/x)^y
// we choose x = 0.07 and y = 2 // perfect value not too big nor too small
// => Level = Sqrt(Stars) * 0.07
public class RewardManager : MonoBehaviour
{
    [SerializeField] private int goldRewardStepOffset = 5;
    private readonly float x = 0.07f;
    //private readonly float y = 2;
    [Header("Data Manager")]
    [SerializeField] private DataManager dataManager;
    [Header("Reward Data")]
    [SerializeField] private GoldRewardScriptableObject goldRewardData;
    [SerializeField] private SkillRewardScriptableObject skillRewardData;

    [Header("Reward Manager UI")]
    [SerializeField] private RewardManagerUI rewardManagerUI;

    private int currentGoldRewardStep;
    private int currentSkillRewardStep;

    private void Start() {
        rewardManagerUI.goldRewardBoxPointer.OnClicked += ClaimGoldReward;
        rewardManagerUI.skillRewardBoxPointer.OnClicked += ClaimSkillReward;
    }

    private void OnDestroy() {
        rewardManagerUI.goldRewardBoxPointer.OnClicked -= ClaimGoldReward;
        rewardManagerUI.skillRewardBoxPointer.OnClicked -= ClaimSkillReward;
    }

    public void SetRewardsProgress(UserData userData){
        bool levelReached, starsReached;
        // gold reward (use level)
        int currentModuleLevel = userData.level % goldRewardStepOffset;
        currentGoldRewardStep = Mathf.Clamp(Mathf.CeilToInt(userData.level/(float)goldRewardStepOffset) - 1,0,int.MaxValue);

        bool isPreviousClaimed = true;
        // check if user has claimed previous rewards yet
        for (int i = 0; i < currentGoldRewardStep; ++i){
            if (!userData.goldRewardClaimStates[i].status){
                isPreviousClaimed = false;
                currentGoldRewardStep = i;
                break;
            }
        }

        // if we reach 5/5
        levelReached = (currentModuleLevel == 0 || !isPreviousClaimed) && userData.level != 0 ;
        // we should check if user reach designated reward step like 5/5 to be eligible to claim reward
        if (levelReached){

            // if user hasn't claim
            if (!userData.goldRewardClaimStates[currentGoldRewardStep].status){
                // play gift box anim here
                rewardManagerUI.ToggleGoldRewardToClaim(true);
                // this will make ui show user reaches current reward step
                // if reward step is 5 then it will show 5/5 if user hasn't claimed reward
                rewardManagerUI.UpdateGoldRewardProgress(goldRewardStepOffset,goldRewardStepOffset);
            }
            else{
                rewardManagerUI.ToggleGoldRewardToClaim(false);
                rewardManagerUI.UpdateGoldRewardProgress(currentModuleLevel,goldRewardStepOffset);
            }
        }
        else{
            rewardManagerUI.UpdateGoldRewardProgress(currentModuleLevel,goldRewardStepOffset);
        }

        // skill reward (use star)
        int requiredStars = GetRequiredStarsFromCurrentStarsProgress(userData.star, out int currentLevel);
        currentSkillRewardStep = currentLevel - 1;

        isPreviousClaimed = true;
        // check if user has claimed previous rewards yet
        for (int i = 0; i < currentSkillRewardStep; ++i){
            if (!userData.skillRewardClaimStates[i].status){
                isPreviousClaimed = false;
                currentSkillRewardStep = i;
                break;
            }
        }

        starsReached = userData.star >= requiredStars || !isPreviousClaimed;
        if (starsReached){
            if (!userData.skillRewardClaimStates[currentSkillRewardStep].status){
                rewardManagerUI.ToggleSkillRewardToClaim(true);
                rewardManagerUI.UpdateSkillRewardProgress(userData.star, requiredStars);
            }
            else {
                // show required stars
                // example: if we reach 50/50 stars but we already claim reward so the UI will show 50/100 or something
                rewardManagerUI.ToggleSkillRewardToClaim(false);
                requiredStars = GetRequiredStarsFromCurrentLevelProgress(currentLevel+1);
                rewardManagerUI.UpdateSkillRewardProgress(userData.star,requiredStars);
            }
        }
        else{
            rewardManagerUI.UpdateSkillRewardProgress(userData.star,requiredStars);
        }
    }

    private void ClaimGoldReward(){
        // handle the data, get amount to reward
        var goldAmount = goldRewardData.data.gold[currentGoldRewardStep >= goldRewardData.data.gold.Count ? goldRewardData.data.gold.Count-1 : currentGoldRewardStep];
        // show opening box
        rewardManagerUI.OpenGoldRewardBox(goldAmount);
        // play sound
        SoundManager.Instance.PlayOneShotSound(SoundId.s_ui_gift_open);
        dataManager.SetClaimGoldRewardStatus(currentGoldRewardStep, true);
        dataManager.TransactGold(goldAmount);

        // close the box after milliseconds later
        rewardManagerUI.CloseGoldRewardBox(12500);
    }

    private void ClaimSkillReward(){
        // handle the data
        var rewards = skillRewardData.data[currentSkillRewardStep >= skillRewardData.data.Count ? skillRewardData.data.Count-1 : currentSkillRewardStep];
        // show opening box
        rewardManagerUI.OpenSkillRewardBox(rewards.dataEntry);
        // play sound
        SoundManager.Instance.PlayOneShotSound(SoundId.s_ui_gift_open);
        dataManager.SetClaimSkillRewardStatus(currentSkillRewardStep, true);
        dataManager.AddSkills(rewards.dataEntry);
        
        // close the box
        rewardManagerUI.CloseSkillRewardBox(12500);
    }

    #region Stars Calculation for Skill Reward
    private int GetLevelFromStars(int stars){
        float level = Mathf.Sqrt(stars) * 0.07f;
        return Mathf.CeilToInt(level);
    }

    private int GetRequiredStarsFromCurrentStarsProgress(int stars, out int currentLevel){
        currentLevel = GetLevelFromStars(stars);
        return Mathf.FloorToInt(Mathf.Pow(currentLevel/x,2));
    }

    private int GetRequiredStarsFromCurrentLevelProgress(int level){
        return Mathf.FloorToInt(Mathf.Pow(level/x,2));
    }
    #endregion
}
