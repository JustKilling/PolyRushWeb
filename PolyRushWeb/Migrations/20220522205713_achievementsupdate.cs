using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyRushWeb.Migrations
{
    public partial class achievementsupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                value: "a3dee2e6-f2af-4c08-9962-4bd7508f65ba");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEL4bmfrY0CXBJT+1E8t3nnwgDeSR2bye4gpyA3j+dpGyiZK06nf1xuzQTdKzgyRf7g==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
