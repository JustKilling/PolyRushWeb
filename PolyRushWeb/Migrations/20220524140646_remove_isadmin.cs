using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyRushWeb.Migrations
{
    public partial class remove_isadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "user");

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 1,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 25 coins in a single game", "25 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 2,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 100 coins in a single game", "100 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 3,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 250 coins in a single game", "250 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 4,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 500 coins in a single game", "500 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 5,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 1000 coins in a single game", "1000 coins in one game" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "80d8ef83-8777-4763-a1df-b5954d208a3f");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAECXyt6FzojMSckIeKQbn/woZvzzTaRCIyumUasLJbxfluPcIrfhhnz698Xp25MqIgw==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "user",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 1,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 10 coins in a single game", "10 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 2,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 50 coins in a single game", "50 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 3,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 100 coins in a single game", "100 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 4,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 250 coins in a single game", "250 coins in one game" });

            migrationBuilder.UpdateData(
                table: "achievement",
                keyColumn: "Idachievement",
                keyValue: 5,
                columns: new[] { "AchievementDescription", "AchievementName" },
                values: new object[] { "Gather 500 coins in a single game", "500 coins in one game" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "c7789d9d-a0f0-414a-8e17-17af38af1dc4");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEBYJbwrhwRfzoMl6+D6hS6jQyHC7LwFmxc6z09ENIhVznFW1Wm5VlmJTG4u5LIFbhg==");
        }
    }
}
