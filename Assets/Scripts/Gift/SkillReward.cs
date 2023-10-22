
using System;
using System.Collections.Generic;

[Serializable]
public class SkillReward{
    public RewardSkill type;
    public int amount;
}

[Serializable]
public class SkillRewards{
    public List<SkillReward> dataEntry;
}