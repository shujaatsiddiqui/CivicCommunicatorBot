using Microsoft.EntityFrameworkCore.Migrations;

namespace CivicCommunicator.Migrations
{
    public partial class Adddomainpropertytouser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HandlingDomain",
                table: "Users",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandlingDomain",
                table: "Users");
        }
    }
}
