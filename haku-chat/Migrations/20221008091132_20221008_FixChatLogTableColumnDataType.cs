using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haku_chat.Migrations
{
    public partial class _20221008_FixChatLogTableColumnDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "reg_date",
                table: "chat_log",
                type: "datetime(3)",
                nullable: false,
                comment: "投稿日時",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "投稿日時");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "reg_date",
                table: "chat_log",
                type: "datetime",
                nullable: false,
                comment: "投稿日時",
                oldClrType: typeof(DateTime),
                oldType: "datetime(3)",
                oldComment: "投稿日時");
        }
    }
}
