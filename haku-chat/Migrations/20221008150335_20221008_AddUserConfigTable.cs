using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haku_chat.Migrations
{
    public partial class _20221008_AddUserConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_config",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(95)", maxLength: 95, nullable: false, comment: "ユーザーID（システム）"),
                    name_color_id = table.Column<ushort>(type: "smallint unsigned", nullable: false, comment: "投稿者名文字色ID"),
                    send_info_mail = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false, comment: "通知メール送信フラグ"),
                    show_chat_log_count = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0, comment: "チャットログ表示件数"),
                    update_datetime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "更新日時")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_config", x => x.id);
                },
                comment: "ユーザー設定管理テーブル");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_config");
        }
    }
}
