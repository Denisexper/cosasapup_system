using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace system_cosasapup.Migrations
{
    /// <inheritdoc />
    public partial class AddPegueIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pegues_pagos_idPago",
                table: "pegues");

            migrationBuilder.DropIndex(
                name: "IX_pegues_idPago",
                table: "pegues");

            migrationBuilder.DropColumn(
                name: "idPago",
                table: "pegues");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "pegues",
                newName: "PegueId");

            migrationBuilder.AddColumn<int>(
                name: "PegueId",
                table: "pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_pagos_PegueId",
                table: "pagos",
                column: "PegueId");

            migrationBuilder.AddForeignKey(
                name: "FK_pagos_pegues_PegueId",
                table: "pagos",
                column: "PegueId",
                principalTable: "pegues",
                principalColumn: "PegueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pagos_pegues_PegueId",
                table: "pagos");

            migrationBuilder.DropIndex(
                name: "IX_pagos_PegueId",
                table: "pagos");

            migrationBuilder.DropColumn(
                name: "PegueId",
                table: "pagos");

            migrationBuilder.RenameColumn(
                name: "PegueId",
                table: "pegues",
                newName: "id");

            migrationBuilder.AddColumn<int>(
                name: "idPago",
                table: "pegues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_pegues_idPago",
                table: "pegues",
                column: "idPago");

            migrationBuilder.AddForeignKey(
                name: "FK_pegues_pagos_idPago",
                table: "pegues",
                column: "idPago",
                principalTable: "pagos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
