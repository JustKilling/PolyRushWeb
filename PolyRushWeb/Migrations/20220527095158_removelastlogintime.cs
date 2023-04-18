using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyRushWeb.Migrations
{
    public partial class removelastlogintime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "user");

            migrationBuilder.DropColumn(
                name: "LastLoginTime",
                table: "user");

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "dd78cd7e-5217-44c8-9c17-59630aa8042a");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEBgyq7L5n8QQ7Cj2i2Z8QHT1o5Bt32xC7iZ6rt5sHS7SdjAsO0Rkwr44m81TQRMxsw==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginTime",
                table: "user",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
    }
}
