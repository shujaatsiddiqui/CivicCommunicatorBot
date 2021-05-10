using Microsoft.EntityFrameworkCore.Migrations;

namespace CivicCommunicator.Migrations
{
    public partial class NullableAgentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AgentId",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AgentId",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
