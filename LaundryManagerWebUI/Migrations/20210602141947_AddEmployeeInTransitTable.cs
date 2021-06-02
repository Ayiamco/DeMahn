using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryManagerWebUI.Migrations
{
    public partial class AddEmployeeInTransitTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeCount",
                table: "Laundries");

            migrationBuilder.CreateTable(
                name: "EmployeesInTransit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaundryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesInTransit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeesInTransit_Laundries_LaundryId",
                        column: x => x.LaundryId,
                        principalTable: "Laundries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesInTransit_LaundryId",
                table: "EmployeesInTransit",
                column: "LaundryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesInTransit");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeCount",
                table: "Laundries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
