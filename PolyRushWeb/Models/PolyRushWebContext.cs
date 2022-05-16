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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("user");
        builder.Entity<IdentityRole<int>>().ToTable("role");
        builder.Entity<IdentityUserRole<int>>().ToTable("userrole");
        builder.Entity<IdentityUserClaim<int>>().ToTable("userclaim");
        builder.Entity<IdentityUserLogin<int>>().ToTable("userlogin");

        //Seed identity admin user
        const int AdminRoleId = 1;
        const string AdminRoleName = "Admin";
        const int AdminUserId = 1;
        const string AdminUserName = "Admin";
        const string AdminEmail = "emiel.delaey@sintjozefbrugge.be";
        const string AdminUserPassword = "Admin123";
        IPasswordHasher<User> passwordHasher = new PasswordHasher<User>(); // Identity password hasher

        User adminUser = new()
        {
            Id = AdminUserId,
            UserName = AdminUserName,
            Firstname = AdminUserName,
            Lastname = AdminUserName,
            NormalizedUserName = AdminUserName.ToUpper(),
            Email = AdminEmail,
            NormalizedEmail = AdminUserName.ToUpper(),
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

        //builder.Entity<User>(entity =>
        //{
        //    entity.HasKey(e => e.Iduser)
        //        .HasName("PRIMARY");

        //    entity.ToTable("user");

        //    entity.HasCharSet("latin1")
        //        .UseCollation("latin1_swedish_ci");

        //    entity.Property(e => e.Iduser)
        //        .HasColumnType("int(11)")
        //        .HasColumnName("IDUser");

        //    entity.Property(e => e.Coins).HasColumnType("int(11)");

        //    entity.Property(e => e.Coinsgathered).HasColumnType("int(11)");

        //    entity.Property(e => e.Coinsspent).HasColumnType("int(11)");

        //    entity.Property(e => e.CreatedDate)
        //        .HasColumnType("datetime")
        //        .HasDefaultValueSql("CURRENT_TIMESTAMP");

        //    entity.Property(e => e.Email).HasColumnType("text");

        //    entity.Property(e => e.Firstname).HasColumnType("text");

        //    entity.Property(e => e.Highscore).HasColumnType("int(11)");

        //    entity.Property(e => e.IsActive)
        //        .IsRequired()
        //        .HasDefaultValueSql("'1'");

        //    entity.Property(e => e.Itemspurchased).HasColumnType("int(11)");

        //    entity.Property(e => e.Lastname).HasColumnType("text");

        //    //entity.Property(e => e.Password).HasColumnType("text");

        //    //entity.Property(e => e.RefreshToken).HasColumnType("text");

        //    //entity.Property(e => e.Salt).HasColumnType("text");

        //    entity.Property(e => e.Scoregathered).HasColumnType("int(11)");

        //    entity.Property(e => e.SeesAds)
        //        .IsRequired()
        //        .HasDefaultValueSql("'1'");

        //    entity.Property(e => e.Timespassed).HasColumnType("int(11)");

        //    entity.Property(e => e.Username).HasColumnType("text");
        //});

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


    }

}