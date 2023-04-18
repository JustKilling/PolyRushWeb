using System.ComponentModel.DataAnnotations;

public class Achievement
{
    [Key] public int Idachievement;
    public string AchievementName;
    public string AchievementDescription;
}
