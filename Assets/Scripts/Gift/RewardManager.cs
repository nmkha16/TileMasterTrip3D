using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// formula for level system is XP = (Level/x)^y / Stars = (Level/x)^y
// we choose x = 0.07 and y = 2 // perfect value not too big nor too small
// => Level = Sqrt(Stars) * 0.07
public class RewardManager : MonoBehaviour
{
    [SerializeField] private int goldRewardStep = 5;
    private readonly float x = 0.07f;
    //private readonly float y = 2;
    [Header("Data Manager")]
    [SerializeField] private DataManager dataManager;
    [Header("Reward Data")]
    [SerializeField] private GoldRewardScriptableObject goldRewardData;
    [SerializeField] private SkillRewardScriptableObject skillRewardData;

    [Header("Reward Manager UI")]
    [SerializeField] private RewardManagerUI rewardManagerUI;

    private void Start() {
        rewardManagerUI.goldRewardBoxPointer.OnClicked += ClaimGoldReward;
        rewardManagerUI.skillRewardBoxPointer.OnClicked += ClaimSkillReward;
    }

    private void OnDestroy() {
        rewardManagerUI.goldRewardBoxPointer.OnClicked -= ClaimGoldReward;
        rewardManagerUI.skillRewardBoxPointer.OnClicked -= ClaimSkillReward;
    }

    public void SetRewardsProgress(UserData userData){
        bool goldReached, starsReached;
        // gold reward (use level)
        int currentModuleLevel = userData.level % goldRewardStep;
        // if we reach 5/5
        goldReached = currentModuleLevel == 0 && userData.level != 0;
        // we should check if user reach designated reward step like 5/5 to be eligible to claim reward
        if (goldReached){
            // we are eligible to claim
            // check if reward is already claim at this step
            // if user hasn't claim
            if (!userData.isClaimedGoldReward){
                // play gift box anim here
                rewardManagerUI.EnableGoldRewardToClaim();
                // this will make ui show user reaches current reward step
                // if reward step is 5 then it will show 5/5 if user hasn't claimed reward
                rewardManagerUI.UpdateGoldRewardProgress(goldRewardStep,goldRewardStep);
            }
            else{
                rewardManagerUI.UpdateGoldRewardProgress(currentModuleLevel,goldRewardStep);
            }
        }
        else{
            if (dataManager.IsClaimedGoldReward){
                dataManager.SetClaimGoldRewardStatus(false);
            }
            rewardManagerUI.UpdateGoldRewardProgress(currentModuleLevel,goldRewardStep);
        }

        // skill reward (use star)
        int requiredStars = GetRequiredStarsFromCurrentStarsProgress(userData.star, out int currentLevel);
        starsReached = userData.star == requiredStars;
        if (starsReached){
            if (!userData.isClaimedSkillReward){
                rewardManagerUI.EnabledSkillRewardToClaim();
                rewardManagerUI.UpdateSkillRewardProgress(userData.star, requiredStars);
            }
            else {
                // show required stars
                // example: if we reach 50/50 stars but we already claim reward so the UI will show 50/100 or something
                requiredStars = GetRequiredStarsFromCurrentLevelProgress(currentLevel+1);
                rewardManagerUI.UpdateSkillRewardProgress(userData.star,requiredStars);
            }
        }
        else{
            if (dataManager.IsClaimedSkillReward){
                dataManager.SetClaimSkillRewardStatus(false);
            }
            rewardManagerUI.UpdateSkillRewardProgress(userData.star,requiredStars);
        }
    }

    private void ClaimGoldReward(){
        // show opening box
        rewardManagerUI.OpenGoldRewardBox();
        // play sound
        SoundManager.Instance.PlayOneShotSound(SoundId.s_ui_gift_open);
        // handle the data
        var rewardIdx = Mathf.FloorToInt(dataManager.level/(float)goldRewardStep);
        
        var goldAmount = goldRewardData.data.gold[rewardIdx >= goldRewardData.data.gold.Count ? goldRewardData.data.gold.Count-1 : rewardIdx];
        dataManager.TransactGold(goldAmount);
        dataManager.SetClaimGoldRewardStatus(true);


        // close the box after milliseconds later
        rewardManagerUI.CloseGoldRewardBox(12500);
    }

    private void ClaimSkillReward(){
        // show opening box
        rewardManagerUI.OpenSkillRewardBox();
        // play sound
        SoundManager.Instance.PlayOneShotSound(SoundId.s_ui_gift_open);
        // handle the data
        var rewardIdx = GetLevelFromStars(dataManager.star);
        var rewards = skillRewardData.data[rewardIdx >= skillRewardData.data.Count ? skillRewardData.data.Count-1 : rewardIdx];
        dataManager.AddSkills(rewards.dataEntry);
        dataManager.SetClaimSkillRewardStatus(true);
        
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
