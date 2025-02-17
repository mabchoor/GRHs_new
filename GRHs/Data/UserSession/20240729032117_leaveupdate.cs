﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace GRHs.Data.UserSession
{
    public partial class leaveupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDays",
                table: "Leaves",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDays",
                table: "Leaves");
        }
    }
}
