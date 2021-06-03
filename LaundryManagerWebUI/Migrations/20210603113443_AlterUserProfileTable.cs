using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryManagerWebUI.Migrations
{
    public partial class AlterUserProfileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Laundries_LaundryId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_LaundryId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LaundryId",
                table: "UserProfiles");

            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LaundryId",
                table: "AspNetUsers",
                column: "LaundryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Laundries_LaundryId",
                table: "AspNetUsers",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Laundries_LaundryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LaundryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LaundryId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_LaundryId",
                table: "UserProfiles",
                column: "LaundryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Laundries_LaundryId",
                table: "UserProfiles",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
