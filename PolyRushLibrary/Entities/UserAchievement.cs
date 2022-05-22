using System;
using System.ComponentModel.DataAnnotations;

public class UserAchievement
{
    [Key] public int IduserAchievement;

    public int UserId;
    public int AchievementId;
}

