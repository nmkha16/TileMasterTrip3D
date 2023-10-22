using System;

[Serializable]
public class UserData
{
    public Action<UserData> OnUserDataChanged;
    public int level;
    public int star;
    public int gold;

    public UserData(int level = 0, int star = 0, int gold = 0){
        this.level = level;
        this.star = star;
        this.gold = gold;
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

    
}
