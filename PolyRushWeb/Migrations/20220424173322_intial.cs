using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyRushWeb.Migrations
{
    public partial class intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
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
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
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
                    Avatar = table.Column<string>(type: "longtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LastLoginTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: true),
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
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

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
                name: "AspNetRoleClaims",
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
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
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
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
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
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
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
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8")
                .Annotation("Relational:Collation", "utf8_general_ci");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 1, "1a600408-17af-43f3-8f32-c9ae0cf2ae45", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "Coins", "Coinsgathered", "Coinsspent", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "Firstname", "Highscore", "IsActive", "IsAdmin", "Itemspurchased", "LastLoginTime", "Lastname", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Scoregathered", "SecurityStamp", "SeesAds", "Timespassed", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, "", 0, 0, 0, "c8554266-b401-4519-9aeb-a9283053fc58", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "emiel.delaey@sintjozefbrugge.be", true, "Admin", 0, null, false, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", false, null, "ADMIN", "ADMIN", "AQAAAAEAACcQAAAAEBnFQVbm3pkD4EN3ZNctLIOtg+BO76XDaFTsp0gLoRRe065QxnYZI+iIBXVz4oolvw==", null, false, 0, "VVPCRDAS3MJWQD5CSW2GWPRADBXEZINA", null, 0, false, "Admin" });

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
                table: "setting",
                columns: new[] { "IDSetting", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "The master volume of the game (0-100)", "Master Volume" },
                    { 2, "Are the sounds effects of the game enabled?", "Sfx" },
                    { 3, "Is the music in the game enabled?", "Music" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "discount");

            migrationBuilder.DropTable(
                name: "gamesession");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "itemtype");

            migrationBuilder.DropTable(
                name: "setting");

            migrationBuilder.DropTable(
                name: "useritem");

            migrationBuilder.DropTable(
                name: "usersetting");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
