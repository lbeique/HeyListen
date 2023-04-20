using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeyListen.Migrations
{
    /// <inheritdoc />
    public partial class Songs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2023, 4, 20, 1, 21, 46, 952, DateTimeKind.Utc).AddTicks(2390));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2023, 4, 20, 0, 11, 0, 452, DateTimeKind.Utc).AddTicks(7468));
        }
    }
}
