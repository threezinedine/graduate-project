using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityStation.API.Migrations
{
    /// <inheritdoc />
    public partial class Updatetablename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormatAttributes_Stations_StationId",
                table: "FormatAttributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormatAttributes",
                table: "FormatAttributes");

            migrationBuilder.RenameTable(
                name: "FormatAttributes",
                newName: "RecordAttributes");

            migrationBuilder.RenameIndex(
                name: "IX_FormatAttributes_StationId",
                table: "RecordAttributes",
                newName: "IX_RecordAttributes_StationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecordAttributes",
                table: "RecordAttributes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordAttributes_Stations_StationId",
                table: "RecordAttributes",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordAttributes_Stations_StationId",
                table: "RecordAttributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecordAttributes",
                table: "RecordAttributes");

            migrationBuilder.RenameTable(
                name: "RecordAttributes",
                newName: "FormatAttributes");

            migrationBuilder.RenameIndex(
                name: "IX_RecordAttributes_StationId",
                table: "FormatAttributes",
                newName: "IX_FormatAttributes_StationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormatAttributes",
                table: "FormatAttributes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormatAttributes_Stations_StationId",
                table: "FormatAttributes",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
