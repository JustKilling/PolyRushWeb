using System.ComponentModel.DataAnnotations;

namespace PolyRushLibrary;

public class Usersetting
{
    [Key] public int IduserSetting;

    public int SettingId;
    public int State;
    public int UserId;
}