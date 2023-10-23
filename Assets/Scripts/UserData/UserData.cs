using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public Action<UserData> OnUserDataChanged;
    public int level;
    public int star;
    public int gold;
    public int undo;
    public bool isClaimedGoldReward;
    public bool isClaimedSkillReward;

    public UserData(int level = 0, int star = 0, int gold = 0){
        this.level = level;
        this.star = star;
        this.gold = gold;
        this.undo = 10;
    }

    public void SetLevel(int level){
        this.level = level;
        OnUserDataChanged?.Invoke(this);
    }

    public void SetStar(int star){
        this.star = star;
        OnUserDataChanged?.Invoke(this);
    }

    public void SetGold(int gold){
        this.gold = gold;
        OnUserDataChanged?.Invoke(this);
    }

    public void AddLevel(int level){
        this.level += level;
        OnUserDataChanged?.Invoke(this);
    }

    public void AddStar(int star){
        this.star += star;
        OnUserDataChanged?.Invoke(this);
    }

    public void AddGold(int gold){
        if (this.gold + gold < 0){
            this.gold = 0;
        } 
        else this.gold += gold;
        OnUserDataChanged?.Invoke(this);
    }

    public void AddSkills(List<SkillReward> rewards){
        foreach(var entry in rewards){
            switch(entry.type){
                case RewardSkill.r_undo:
                    this.undo += entry.amount;
                    break;
            }
        }
        OnUserDataChanged?.Invoke(this);
    }

    public void SetClaimGoldRewardStatus(bool status){
        isClaimedGoldReward = status;
        OnUserDataChanged?.Invoke(this);
    }

    public void SetClaimSkillRewardStatus(bool status){
        isClaimedSkillReward = status;
        OnUserDataChanged?.Invoke(this);
    }
}
