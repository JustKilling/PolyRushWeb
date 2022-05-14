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
        
    }

}