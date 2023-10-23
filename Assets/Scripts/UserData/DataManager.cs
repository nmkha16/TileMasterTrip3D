
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Action<int> OnIngameStarsUpdated; 
    [Header("User Data")]
    private UserData userData;
    public int gold {get => userData.gold;}
    public int star {get => userData.star;}
    public int level {get => userData.level;}
    private string saveFile;
    [Header("Ingame Star")]
    private int ingameStars = 0;
    private int IngameStars{
        get{
            return ingameStars;
        }
        set{
            ingameStars = value;
            OnIngameStarsUpdated?.Invoke(ingameStars);
        }
    }

    [Header("Reward Manager")]
    [SerializeField] private RewardManager rewardManager;

    [Header("Data Manager UI")]
    [SerializeField] private DataManagerUI dataManagerUI;
    [Header("Game Manager UI")]
    [SerializeField] private GameManagerUI gameManagerUI;

    private void Awake() {
        saveFile = Application.persistentDataPath + "/userdata.dat";
    }

    private async void Start(){
        this.userData = await LoadData();

        userData.OnUserDataChanged += (o) => SaveData();
        userData.OnUserDataChanged += dataManagerUI.UpdateStatUI;
        userData.OnUserDataChanged += dataManagerUI.UpdateRemainingSkill;
        userData.OnUserDataChanged += gameManagerUI.ValidatePlayOnButton;

        userData.OnUserDataChanged += rewardManager.SetRewardsProgress;

        userData.OnUserDataChanged?.Invoke(userData);

        GameManager.Instance.OnGameStarted += ResetIngameStarsCount;
        OnIngameStarsUpdated += dataManagerUI.UpdateIngameStar;
    }

    private void OnDestroy(){
        userData.OnUserDataChanged -= dataManagerUI.UpdateStatUI;
        userData.OnUserDataChanged -= gameManagerUI.ValidatePlayOnButton;
        userData.OnUserDataChanged -= dataManagerUI.UpdateRemainingSkill;
        userData.OnUserDataChanged -= rewardManager.SetRewardsProgress;
        GameManager.Instance.OnGameStarted -= ResetIngameStarsCount;
        OnIngameStarsUpdated -= dataManagerUI.UpdateIngameStar;
    }

    private async Task<UserData> LoadData(){
        UserData data = new();
        TaskCompletionSource<UserData> tcs = new TaskCompletionSource<UserData>();
        if (File.Exists(saveFile)){
            string content = File.ReadAllText(saveFile);
            if (!string.IsNullOrEmpty(content)){
                data = JsonUtility.FromJson<UserData>(content);
            }
        }
        tcs.SetResult(data);
        return await tcs.Task;
    }

    public void SaveData(){
        string content = JsonUtility.ToJson(userData);
        File.WriteAllText(saveFile,content);
    }

    public void IncrementLevelProgress(int maximum){
        if (userData.level >= maximum-1) return;
        userData.AddLevel(1);
    }

    public int GetLevel(){
        return userData.level;
    }

    public int GetGold(){
        return userData.gold;
    }

    // use negative value to do minus calculation
    public void TransactGold(int gold){
        userData.AddGold(gold);
    }

    public void AddIngameStars(int value){
        IngameStars += value;
    }

    private void ResetIngameStarsCount(){
        IngameStars = 0;
    }

    public void AddIngameStarsToUserData(){
        userData.AddStar(IngameStars);
    }

    public void AddSkills(List<SkillReward> rewards){
        userData.AddSkills(rewards);
    }

    public void SetClaimGoldRewardStatus(int step, bool toggle){
        userData.SetClaimGoldRewardStatus(step,toggle);
    }

    public void SetClaimSkillRewardStatus(int step, bool  toggle){
        userData.SetClaimSkillRewardStatus(step, toggle);
    }

    public void Undo(){
        if (userData.undo > 0){
            CommandInvoker.UndoCommand();
            userData.AddSkill(SkillType.Undo,-1);
        }
    }

    public bool CanNuke(){
        return userData.tacNuke > 0;
    }

    public void DecrementTacticalNukeCount(){
        userData.AddSkill(SkillType.Tactical_Nuke,-1);
    }

    #region prototype
    public void AddGoldPrototype(int value){
        userData.AddGold(value);
    }
    #endregion
}
