using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryData.Migrations
{
    public partial class try1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckoutHistories_LibraryBranches_LibraryBranchId",
                table: "CheckoutHistories");

            migrationBuilder.DropIndex(
                name: "IX_CheckoutHistories_LibraryBranchId",
                table: "CheckoutHistories");

            migrationBuilder.DropColumn(
                name: "LibraryBranchId",
                table: "CheckoutHistories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckedIn",
                table: "CheckoutHistories",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LibraryCardId",
                table: "CheckoutHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutHistories_LibraryCardId",
                table: "CheckoutHistories",
                column: "LibraryCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckoutHistories_LibraryCards_LibraryCardId",
                table: "CheckoutHistories",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckoutHistories_LibraryCards_LibraryCardId",
                table: "CheckoutHistories");

            migrationBuilder.DropIndex(
                name: "IX_CheckoutHistories_LibraryCardId",
                table: "CheckoutHistories");

            migrationBuilder.DropColumn(
                name: "LibraryCardId",
                table: "CheckoutHistories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckedIn",
                table: "CheckoutHistories",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "LibraryBranchId",
                table: "CheckoutHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutHistories_LibraryBranchId",
                table: "CheckoutHistories",
                column: "LibraryBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckoutHistories_LibraryBranches_LibraryBranchId",
                table: "CheckoutHistories",
                column: "LibraryBranchId",
                principalTable: "LibraryBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
