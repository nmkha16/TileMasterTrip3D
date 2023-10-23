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
    public int tacNuke;
    public List<ClaimRewardState> goldRewardClaimStates;
    public List<ClaimRewardState> skillRewardClaimStates;

    public UserData(int level = 0, int star = 1, int gold = 0){
        this.level = level;
        this.star = star;
        this.gold = gold;
        this.undo = 50;
        this.tacNuke = 20;

        goldRewardClaimStates = new(new ClaimRewardState[10]);
        skillRewardClaimStates = new(new ClaimRewardState[10]);
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
                case SkillType.Undo:
                    this.undo += entry.amount;
                    break;
                case SkillType.Tactical_Nuke:
                    this.tacNuke += entry.amount;
                    break;
            }
        }
        OnUserDataChanged?.Invoke(this);
    }

    public void AddSkill(SkillType type, int amount){
        switch(type){
            case SkillType.Undo:
                this.undo += amount;
                break;
            case SkillType.Tactical_Nuke:
                this.tacNuke += amount;
                break;
        }
        OnUserDataChanged?.Invoke(this);
    }

    public void SetClaimGoldRewardStatus(int step, bool status, bool surpassSavefile = true){
        goldRewardClaimStates[step].status = status;
        if (!surpassSavefile) OnUserDataChanged?.Invoke(this);
    }

    public void SetClaimSkillRewardStatus(int step, bool status, bool surpassSavefile = true){
        skillRewardClaimStates[step].status = status;
        if (!surpassSavefile) OnUserDataChanged?.Invoke(this);
    }
}

[Serializable]
public class ClaimRewardState{
    public bool status;
    public ClaimRewardState(){
        status = false;
    }
}
