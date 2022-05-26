using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PolyRushWeb.Models;


namespace PolyRushWeb.Data;

public class PolyRushWebContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public PolyRushWebContext(DbContextOptions<PolyRushWebContext> options)
        : base(options)
    {
    }

    protected PolyRushWebContext(DbContextOptions options)
        : base(options)
    {
    }
    public DbSet<Discount> Discount { get; set; } = null!;
    public DbSet<Gamesession> Gamesession { get; set; } = null!;
    public DbSet<Item> Item { get; set; } = null!;
    public DbSet<Itemtype> Itemtype { get; set; } = null!;
    public DbSet<Setting> Setting { get; set; } = null!;
    public DbSet<Useritem> Useritem { get; set; } = null!;
    public DbSet<Usersetting> Usersetting { get; set; } = null!;
    public DbSet<Achievement> Achievement { get; set; } = null!;
    public DbSet<UserAchievement> UserAchievement { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("user");
        builder.Entity<IdentityRole<int>>().ToTable("role");
        builder.Entity<IdentityUserRole<int>>().ToTable("userrole");
        builder.Entity<IdentityUserClaim<int>>().ToTable("userclaim");
        builder.Entity<IdentityUserLogin<int>>().ToTable("userlogin");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("roleclaim");
        builder.Entity<IdentityUserToken<int>>().ToTable("usertoken");

        //Seed identity admin user
        const int AdminRoleId = 1;
        const string AdminRoleName = "Admin";
        const int AdminUserId = 1;
        const string AdminUserName = "Admin";
        const string AdminEmail = "emiel.delaey@sintjozefbrugge.be";
        Random? random = new Random();
        Byte[] b = new Byte[16]; random.NextBytes(b); //random 32 bytes
        string AdminUserPassword = Convert.ToBase64String(b); //admin should reset password before he can login! This generates a random password
        IPasswordHasher<User> passwordHasher = new PasswordHasher<User>(); // Identity password hasher

        User adminUser = new()
        {
            Id = AdminUserId,
            UserName = AdminUserName,
            Firstname = AdminUserName,
            Lastname = AdminUserName,
            NormalizedUserName = AdminUserName.ToUpper(),
            Email = AdminEmail,
            NormalizedEmail = AdminEmail.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = "V3PFRDAS3MJWQD5TSW2GWPRADBFEZINA", //Random 
            ConcurrencyStamp = "n8754226-b405-4519-9beb-a9281053f355", //Random
        };

        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, AdminUserPassword);
        builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int>
        {
            Id = AdminRoleId,
            Name = AdminRoleName,
            NormalizedName = AdminRoleName.ToUpper()
        });

        builder.Entity<User>().HasData(adminUser);
        builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
        {
            RoleId = AdminRoleId,
            UserId = AdminUserId
        });


        //isactive is true by default
        builder.Entity<User>(entity =>
        {
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });


        builder.UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        builder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Iddiscount)
                .HasName("PRIMARY");

            entity.ToTable("discount");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.Iddiscount)
                .HasColumnType("int(11)")
                .HasColumnName("IDDiscount");

            entity.Property(e => e.DiscountPercentage).HasColumnType("int(11)");

            entity.Property(e => e.Enddate).HasColumnType("datetime");

            entity.Property(e => e.ItemId)
                .HasColumnType("int(11)")
                .HasColumnName("ItemID");

            entity.Property(e => e.Startdate).HasColumnType("datetime");
        });



        builder.Entity<Gamesession>(entity =>
        {
            entity.HasKey(e => e.IdgameSession)
                .HasName("PRIMARY");

            entity.ToTable("gamesession");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.IdgameSession)
                .HasColumnType("int(11)")
                .HasColumnName("IDGameSession");

            entity.Property(e => e.CoinsGathered).HasColumnType("int(11)");

            entity.Property(e => e.EndDateTime).HasColumnType("datetime");

            entity.Property(e => e.PeoplePassed).HasColumnType("int(11)");

            entity.Property(e => e.ScoreGathered).HasColumnType("int(11)");

            entity.Property(e => e.StartDateTime).HasColumnType("datetime");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");
        });


        builder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Iditem)
                .HasName("PRIMARY");

            entity.ToTable("item");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.Iditem)
                .HasColumnType("int(11)")
                .HasColumnName("IDItem");

            entity.Property(e => e.ItemTypeId)
                .HasColumnType("int(11)")
                .HasColumnName("ItemTypeID");

            entity.Property(e => e.Name).HasColumnType("text");

            entity.Property(e => e.Price).HasColumnType("int(11)");


        });


        //Seed items
        List<Item>? itemsList = new()
        {
            new Item { Iditem = 1, ItemTypeId = 2, Name = "Double score", Price = 69 },
            new Item { Iditem = 2, ItemTypeId = 2, Name = "Forcefield", Price = 200 },
            new Item { Iditem = 3, ItemTypeId = 1, Name = "Playerskin1", Price = 200 },
            new Item { Iditem = 4, ItemTypeId = 1, Name = "Playerskin2", Price = 1000 },
            new Item { Iditem = 5, ItemTypeId = 1, Name = "Playerskin3", Price = 5000 },
            new Item { Iditem = 6, ItemTypeId = 1, Name = "Playerskin4", Price = 20000 },

        };

        foreach (Item? item in itemsList)
        {
            builder.Entity<Item>().HasData(item);
        }

        builder.Entity<Itemtype>(entity =>
        {
            entity.HasKey(e => e.IditemType)
                .HasName("PRIMARY");

            entity.ToTable("itemtype");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.IditemType)
                .HasColumnType("int(11)")
                .HasColumnName("IDItemType");

            entity.Property(e => e.Name).HasColumnType("text");
        });

        //Seed itemType
        List<Itemtype>? itemTypes = new()
        {
            new Itemtype { IditemType = 1, Name = "Skin" },
            new Itemtype { IditemType = 2, Name = "Ability" },

        };

        foreach (Itemtype? itemType in itemTypes)
        {
            builder.Entity<Itemtype>().HasData(itemType);
        }

        builder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Idsetting)
                .HasName("PRIMARY");

            entity.ToTable("setting");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.Idsetting)
                .HasColumnType("int(11)")
                .HasColumnName("IDSetting");

            entity.Property(e => e.Description).HasColumnType("text");

            entity.Property(e => e.Name).HasColumnType("text");
        });


        //SEeding settings
        List<Setting>? settings = new()
        {
            new Setting { Idsetting = 1, Name = "Master Volume", Description = "The master volume of the game (0-100)" },
            new Setting { Idsetting = 2, Name = "Sfx", Description = "Are the sounds effects of the game enabled?" },
            new Setting { Idsetting = 3, Name = "Music", Description = "Is the music in the game enabled?" }
        };
        foreach (Setting? setting in settings)
        {
            builder.Entity<Setting>().HasData(setting);
        }


        builder.Entity<Useritem>(entity =>
        {
            entity.HasKey(e => e.IduserItem)
                .HasName("PRIMARY");

            entity.ToTable("useritem");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.IduserItem)
                .HasColumnType("int(11)")
                .HasColumnName("IDUserItem");

            entity.Property(e => e.Amount).HasColumnType("int(11)");

            entity.Property(e => e.ItemId)
                .HasColumnType("int(11)")
                .HasColumnName("ItemID");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");
        });

        builder.Entity<Usersetting>(entity =>
        {
            entity.HasKey(e => e.IduserSetting)
                .HasName("PRIMARY");

            entity.ToTable("usersetting");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.IduserSetting)
                .HasColumnType("int(11)")
                .HasColumnName("IDUserSetting");

            entity.Property(e => e.SettingId)
                .HasColumnType("int(11)")
                .HasColumnName("SettingID");

            entity.Property(e => e.State)
                .HasColumnType("int(11)")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");
        });

        builder.Entity<UserAchievement>(entity =>
        {
            entity.HasKey(e => e.IduserAchievement)
                .HasName("PRIMARY");

            entity.ToTable("userachievement");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)");
           

            entity.Property(e => e.AchievementId)
                .HasColumnType("int(11)");


        });

        builder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Idachievement)
                .HasName("PRIMARY");

            entity.ToTable("achievement");

            entity.HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.AchievementDescription);


            entity.Property(e => e.AchievementName);
        });
        //make the list of different achievements
        List<Achievement>? achievements = new List<Achievement>()
        {
            new Achievement()
            {
                Idachievement = 1, AchievementName = "25 coins in one game",
                AchievementDescription = "Gather 25 coins in a single game"
            },
            new Achievement()
            {
                Idachievement = 2, AchievementName = "100 coins in one game",
                AchievementDescription = "Gather 100 coins in a single game"
            },  
            new Achievement()
            {
                Idachievement = 3, AchievementName = "250 coins in one game",
                AchievementDescription = "Gather 250 coins in a single game"
            },
            new Achievement()
            {
                Idachievement = 4, AchievementName = "500 coins in one game",
                AchievementDescription = "Gather 500 coins in a single game"
            },
            new Achievement()
            {
                Idachievement = 5, AchievementName = "1000 coins in one game",
                AchievementDescription = "Gather 1000 coins in a single game"
            },
            new Achievement()
            {
                Idachievement = 6, AchievementName = "Numero uno",
                AchievementDescription = "Take the number one position on the leaderboard"
            },
            new Achievement()
            {
                Idachievement = 7, AchievementName = "Shopper",
                AchievementDescription = "Buy something from the shop"
            },
            new Achievement()
            {
                Idachievement = 8, AchievementName = "Player",
                AchievementDescription = "Play the game for the first time."
            },
            new Achievement()
            {
                Idachievement = 9, AchievementName = "1000 highscore",
                AchievementDescription = "Reach a highscore of 1000."
            },   
            new Achievement()
            {
                Idachievement = 10, AchievementName = "2500 highscore",
                AchievementDescription = "Reach a highscore of 2500."
            },
            new Achievement()
            {
                Idachievement = 11, AchievementName = "10000 highscore",
                AchievementDescription = "Reach a highscore of 10000."
            },
            new Achievement()
            {
                Idachievement = 12, AchievementName = "50000 highscore",
                AchievementDescription = "Reach a highscore of 50000."
            }
        };
        //seed them to the table
        foreach (Achievement? achievement in achievements)
        {
            builder.Entity<Achievement>().HasData(achievement);
        }
    }

}