using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyRushWeb.Migrations
{
    public partial class remove_seesads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeesAds",
                table: "user");

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "c01c0608-c0f4-484d-bf3a-5093195c7d5e");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJI32iqW/Hrt7mKKCqJpxeBGFR8ieeFN98Hf7l1EdGemT2TIxyeHrzyiIp6g/wuUyg==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SeesAds",
                table: "user",
                type: "tinyint(1)",
                nullable: true);

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
    }
}
