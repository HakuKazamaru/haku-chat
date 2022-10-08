using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace haku_chat.Migrations
{
    public partial class AddDbComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "user_master",
                comment: "ユーザーマスターテーブル");

            migrationBuilder.AlterTable(
                name: "chat_name_color_master",
                comment: "投稿者文字色マスターテーブル");

            migrationBuilder.AlterTable(
                name: "chat_login_user",
                comment: "チャット入室ステータス管理テーブル");

            migrationBuilder.AlterTable(
                name: "chat_log",
                comment: "チャットログ管理テーブル");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "user_master",
                type: "varchar(64)",
                nullable: false,
                comment: "ユーザー名",
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "user_master",
                type: "varchar(64)",
                nullable: false,
                comment: "ユーザーID（ユーザー）",
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "passwordhash",
                table: "user_master",
                type: "varchar(256)",
                nullable: true,
                comment: "パスワードハッシュ",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "user_master",
                type: "varchar(256)",
                nullable: true,
                comment: "パスワード",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "user_master",
                type: "varchar(256)",
                nullable: false,
                comment: "メールアドレス",
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "user_master",
                type: "varchar(95)",
                nullable: false,
                comment: "ユーザーID（システム）",
                oldClrType: typeof(string),
                oldType: "varchar(95)");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "chat_name_color_master",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                comment: "投稿者名文字色名称",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "chat_name_color_master",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                comment: "投稿者名文字色カラーコード",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<ushort>(
                name: "id",
                table: "chat_name_color_master",
                type: "smallint unsigned",
                nullable: false,
                comment: "投稿者名文字色ID",
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "chat_login_user",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                comment: "ブラウザユーザーエージェント",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<ushort>(
                name: "status",
                table: "chat_login_user",
                type: "smallint unsigned",
                nullable: false,
                comment: "入室ステータス",
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned");

            migrationBuilder.AlterColumn<string>(
                name: "os_name",
                table: "chat_login_user",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "OS名称",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "machine_name",
                table: "chat_login_user",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "コンピューター名",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "logout_time",
                table: "chat_login_user",
                type: "datetime",
                nullable: true,
                comment: "退室日時",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "login_time",
                table: "chat_login_user",
                type: "datetime",
                nullable: true,
                comment: "入室日時",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_update",
                table: "chat_login_user",
                type: "datetime",
                nullable: false,
                comment: "最終更新日時",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "chat_login_user",
                type: "varchar(43)",
                maxLength: 43,
                nullable: true,
                comment: "IPアドレス",
                oldClrType: typeof(string),
                oldType: "varchar(43)",
                oldMaxLength: 43,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "host_name",
                table: "chat_login_user",
                type: "varchar(128)",
                nullable: true,
                comment: "ホスト名",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "chat_login_user",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "入室者名",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<ushort>(
                name: "name_color_id",
                table: "chat_log",
                type: "smallint unsigned",
                nullable: true,
                comment: "投稿者文字色ID",
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "chat_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "投稿者名",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "chat_log",
                type: "text",
                nullable: false,
                comment: "チャットメッセージ",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "reg_date",
                table: "chat_log",
                type: "datetime",
                nullable: false,
                comment: "投稿日時",
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "user_master",
                oldComment: "ユーザーマスターテーブル");

            migrationBuilder.AlterTable(
                name: "chat_name_color_master",
                oldComment: "投稿者文字色マスターテーブル");

            migrationBuilder.AlterTable(
                name: "chat_login_user",
                oldComment: "チャット入室ステータス管理テーブル");

            migrationBuilder.AlterTable(
                name: "chat_log",
                oldComment: "チャットログ管理テーブル");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "user_master",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldComment: "ユーザー名");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "user_master",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldComment: "ユーザーID（ユーザー）");

            migrationBuilder.AlterColumn<string>(
                name: "passwordhash",
                table: "user_master",
                type: "varchar(256)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldNullable: true,
                oldComment: "パスワードハッシュ");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "user_master",
                type: "varchar(256)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldNullable: true,
                oldComment: "パスワード");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "user_master",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldComment: "メールアドレス");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "user_master",
                type: "varchar(95)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(95)",
                oldComment: "ユーザーID（システム）");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "chat_name_color_master",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldComment: "投稿者名文字色名称");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "chat_name_color_master",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldComment: "投稿者名文字色カラーコード");

            migrationBuilder.AlterColumn<ushort>(
                name: "id",
                table: "chat_name_color_master",
                type: "smallint unsigned",
                nullable: false,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldComment: "投稿者名文字色ID")
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "user_agent",
                table: "chat_login_user",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldNullable: true,
                oldComment: "ブラウザユーザーエージェント");

            migrationBuilder.AlterColumn<ushort>(
                name: "status",
                table: "chat_login_user",
                type: "smallint unsigned",
                nullable: false,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldComment: "入室ステータス");

            migrationBuilder.AlterColumn<string>(
                name: "os_name",
                table: "chat_login_user",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "OS名称");

            migrationBuilder.AlterColumn<string>(
                name: "machine_name",
                table: "chat_login_user",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "コンピューター名");

            migrationBuilder.AlterColumn<DateTime>(
                name: "logout_time",
                table: "chat_login_user",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "退室日時");

            migrationBuilder.AlterColumn<DateTime>(
                name: "login_time",
                table: "chat_login_user",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "入室日時");

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_update",
                table: "chat_login_user",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "最終更新日時");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "chat_login_user",
                type: "varchar(43)",
                maxLength: 43,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(43)",
                oldMaxLength: 43,
                oldNullable: true,
                oldComment: "IPアドレス");

            migrationBuilder.AlterColumn<string>(
                name: "host_name",
                table: "chat_login_user",
                type: "varchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldNullable: true,
                oldComment: "ホスト名");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "chat_login_user",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "入室者名");

            migrationBuilder.AlterColumn<ushort>(
                name: "name_color_id",
                table: "chat_log",
                type: "smallint unsigned",
                nullable: true,
                oldClrType: typeof(ushort),
                oldType: "smallint unsigned",
                oldNullable: true,
                oldComment: "投稿者文字色ID");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "chat_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "投稿者名");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "chat_log",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "チャットメッセージ");

            migrationBuilder.AlterColumn<DateTime>(
                name: "reg_date",
                table: "chat_log",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "投稿日時");
        }
    }
}
