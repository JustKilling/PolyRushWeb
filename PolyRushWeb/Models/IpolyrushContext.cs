using Microsoft.EntityFrameworkCore;

namespace PolyRushWeb.Models
{
    public interface IpolyrushContext
    {
        DbSet<Discount> Discounts { get; set; }
        DbSet<Gamesession> Gamesessions { get; set; }
        DbSet<Item> Items { get; set; }
        DbSet<Itemtype> Itemtypes { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<Useritem> Useritems { get; set; }
        DbSet<Usersetting> Usersettings { get; set; }
    }
}