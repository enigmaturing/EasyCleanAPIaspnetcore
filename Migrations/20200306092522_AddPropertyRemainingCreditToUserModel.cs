﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyClean.API.Migrations
{
    public partial class AddPropertyRemainingCreditToUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RemainingCredit",
                table: "Users",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingCredit",
                table: "Users");
        }
    }
}
