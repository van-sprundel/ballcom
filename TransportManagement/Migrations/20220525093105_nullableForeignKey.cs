using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportManagement.Migrations
{
    public partial class nullableForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TransportCompanies_TransportCompanyId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "TransportCompanyId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TransportCompanies_TransportCompanyId",
                table: "Orders",
                column: "TransportCompanyId",
                principalTable: "TransportCompanies",
                principalColumn: "TransportCompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TransportCompanies_TransportCompanyId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "TransportCompanyId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TransportCompanies_TransportCompanyId",
                table: "Orders",
                column: "TransportCompanyId",
                principalTable: "TransportCompanies",
                principalColumn: "TransportCompanyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
