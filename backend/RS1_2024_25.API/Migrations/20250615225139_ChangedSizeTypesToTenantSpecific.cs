using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RS1_2024_25.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangedSizeTypesToTenantSpecific : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SizeTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SizeTypes_TenantId",
                table: "SizeTypes",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SizeTypes_Tenants_TenantId",
                table: "SizeTypes",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizeTypes_Tenants_TenantId",
                table: "SizeTypes");

            migrationBuilder.DropIndex(
                name: "IX_SizeTypes_TenantId",
                table: "SizeTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SizeTypes");
        }
    }
}
