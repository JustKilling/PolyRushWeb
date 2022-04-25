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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
            Avatar = "",
            NormalizedUserName = AdminUserName.ToUpper(),
            Email = AdminEmail,
            NormalizedEmail = AdminUserName.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = "VVPCRDAS3MJWQD5CSW2GWPRADBXEZINA", //Random string
            ConcurrencyStamp = "c8554266-b401-4519-9aeb-a9283053fc58", //Random guid string,
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
        
    }

}