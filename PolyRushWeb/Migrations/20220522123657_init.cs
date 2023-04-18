using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyRushWeb.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "achievement",
                columns: table => new
                {
                    Idachievement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AchievementDescription = table.Column<string>(type: "longtext", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    AchievementName = table.Column<string>(type: "longtext", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Idachievement);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "discount",
                columns: table => new
                {
                    IDDiscount = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscountPercentage = table.Column<int>(type: "int(11)", nullable: false),
                    Enddate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ItemID = table.Column<int>(type: "int(11)", nullable: false),
                    Startdate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDDiscount);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "gamesession",
                columns: table => new
                {
                    IDGameSession = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CoinsGathered = table.Column<int>(type: "int(11)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    PeoplePassed = table.Column<int>(type: "int(11)", nullable: false),
                    ScoreGathered = table.Column<int>(type: "int(11)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDGameSession);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    IDItem = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemTypeID = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    Price = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDItem);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "itemtype",
                columns: table => new
                {
                    IDItemType = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDItemType);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "setting",
                columns: table => new
                {
                    IDSetting = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "text", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    Name = table.Column<string>(type: "text", nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDSetting);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Firstname = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Lastname = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IsAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SeesAds = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Coins = table.Column<int>(type: "int", nullable: false),
                    Highscore = table.Column<int>(type: "int", nullable: false),
                    Scoregathered = table.Column<int>(type: "int", nullable: false),
                    Itemspurchased = table.Column<int>(type: "int", nullable: false),
                    Coinsspent = table.Column<int>(type: "int", nullable: false),
                    Coinsgathered = table.Column<int>(type: "int", nullable: false),
                    Timespassed = table.Column<int>(type: "int", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "userachievement",
                columns: table => new
                {
                    IduserAchievement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AchievementId = table.Column<int>(type: "int(11)", nullable: false),
                    UserId = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IduserAchievement);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "useritem",
                columns: table => new
                {
                    IDUserItem = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(type: "int(11)", nullable: false),
                    ItemID = table.Column<int>(type: "int(11)", nullable: false),
                    UserID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDUserItem);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "usersetting",
                columns: table => new
                {
                    IDUserSetting = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SettingID = table.Column<int>(type: "int(11)", nullable: false),
                    State = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'1'"),
                    UserID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IDUserSetting);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "roleclaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roleclaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roleclaim_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "userclaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userclaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userclaim_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "userlogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userlogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_userlogin_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "userrole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userrole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_userrole_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userrole_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "usertoken",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Value = table.Column<string>(type: "longtext", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertoken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_usertoken_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.InsertData(
                table: "achievement",
                columns: new[] { "Idachievement", "AchievementDescription", "AchievementName" },
                values: new object[,]
                {
                    { 1, "Gather 10 coins in a single game", "10 coins in one game" },
                    { 2, "Gather 50 coins in a single game", "50 coins in one game" },
                    { 3, "Gather 100 coins in a single game", "100 coins in one game" },
                    { 4, "Gather 250 coins in a single game", "250 coins in one game" },
                    { 5, "Gather 500 coins in a single game", "500 coins in one game" },
                    { 6, "Take the number one position on the leaderboard", "Numero uno" },
                    { 7, "Buy something from the shop", "Shopper" },
                    { 8, "Play the game for the first time.", "Player" },
                    { 9, "Reach a highscore of 1000.", "1000 highscore" },
                    { 10, "Reach a highscore of 2500.", "2500 highscore" },
                    { 11, "Reach a highscore of 10000.", "10000 highscore" },
                    { 12, "Reach a highscore of 50000.", "50000 highscore" }
                });

            migrationBuilder.InsertData(
                table: "item",
                columns: new[] { "IDItem", "ItemTypeID", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 2, "Double score", 69 },
                    { 2, 2, "Forcefield", 200 },
                    { 3, 1, "Playerskin1", 200 },
                    { 4, 1, "Playerskin2", 1000 },
                    { 5, 1, "Playerskin3", 5000 },
                    { 6, 1, "Playerskin4", 20000 }
                });

            migrationBuilder.InsertData(
                table: "itemtype",
                columns: new[] { "IDItemType", "Name" },
                values: new object[,]
                {
                    { 1, "Skin" },
                    { 2, "Ability" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 1, "c7789d9d-a0f0-414a-8e17-17af38af1dc4", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "setting",
                columns: new[] { "IDSetting", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "The master volume of the game (0-100)", "Master Volume" },
                    { 2, "Are the sounds effects of the game enabled?", "Sfx" },
                    { 3, "Is the music in the game enabled?", "Music" }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "Id", "AccessFailedCount", "Coins", "Coinsgathered", "Coinsspent", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "Firstname", "Highscore", "IsAdmin", "Itemspurchased", "LastLoginTime", "Lastname", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Scoregathered", "SecurityStamp", "SeesAds", "Timespassed", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, 0, 0, 0, "n8754226-b405-4519-9beb-a9281053f355", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "emiel.delaey@sintjozefbrugge.be", true, "Admin", 0, false, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", false, null, "EMIEL.DELAEY@SINTJOZEFBRUGGE.BE", "ADMIN", "AQAAAAEAACcQAAAAEBYJbwrhwRfzoMl6+D6hS6jQyHC7LwFmxc6z09ENIhVznFW1Wm5VlmJTG4u5LIFbhg==", null, false, 0, "V3PFRDAS3MJWQD5TSW2GWPRADBFEZINA", null, 0, false, "Admin" });

            migrationBuilder.InsertData(
                table: "userrole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roleclaim_RoleId",
                table: "roleclaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "user",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userclaim_UserId",
                table: "userclaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userlogin_UserId",
                table: "userlogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userrole_RoleId",
                table: "userrole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "achievement");

            migrationBuilder.DropTable(
                name: "discount");

            migrationBuilder.DropTable(
                name: "gamesession");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "itemtype");

            migrationBuilder.DropTable(
                name: "roleclaim");

            migrationBuilder.DropTable(
                name: "setting");

            migrationBuilder.DropTable(
                name: "userachievement");

            migrationBuilder.DropTable(
                name: "userclaim");

            migrationBuilder.DropTable(
                name: "useritem");

            migrationBuilder.DropTable(
                name: "userlogin");

            migrationBuilder.DropTable(
                name: "userrole");

            migrationBuilder.DropTable(
                name: "usersetting");

            migrationBuilder.DropTable(
                name: "usertoken");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
