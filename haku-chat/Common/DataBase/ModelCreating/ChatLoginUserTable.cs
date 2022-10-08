using Microsoft.EntityFrameworkCore;

using NLog;
using NLog.Web;

using haku_chat.Models;

namespace haku_chat.Common.DataBase.ModelCreating
{
    /// <summary>
    /// チャット入室ステータス管理テーブル用クラス
    /// </summary>
    public class ChatLoginUserTable
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// チャット入室ステータス管理テーブルモデル初期化メソッド
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="modelBuilder"></param>
        public static void Init(string dbType, ref ModelBuilder modelBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            // チャットログ管理テーブル
            modelBuilder.Entity<ChatLoginUserModel>(entity =>
            {
                // 主キーの設定
                entity.HasKey(e => e.Name);
                // テーブル名の設定
                entity.HasComment("チャット入室ステータス管理テーブル");

                // DB共通のカラム処理
                // 入室者名
                entity.Property(c => c.Name)
                    .HasColumnType("varchar(100)")
                    .HasComment("入室者名");
                // 入室ステータス
                entity.Property(c => c.Status)
                    .HasColumnType("smallint unsigned")
                    .HasComment("入室ステータス");
                // IPアドレス
                entity.Property(c => c.IpAddress)
                    .HasColumnType("varchar(43)")
                    .HasComment("IPアドレス");
                // ホスト名
                entity.Property(c => c.HostName)
                    .HasColumnType("varchar(128)")
                    .HasComment("ホスト名");
                // コンピューター名
                entity.Property(c => c.MachineName)
                    .HasColumnType("varchar(128)")
                    .HasComment("コンピューター名");
                // OS名称
                entity.Property(c => c.OsName)
                    .HasColumnType("varchar(128)")
                    .HasComment("OS名称");
                // ブラウザユーザーエージェント
                entity.Property(c => c.UserAgent)
                    .HasColumnType("varchar(256)")
                    .HasComment("ブラウザユーザーエージェント");

                // DBごとのカラム処理
                switch (dbType)
                {
                    case "mysql":
                        {
                            logger.Debug("MySQLモードで実行します。");
                            // 入室日時
                            entity.Property(c => c.LoginTime)
                                .HasColumnType("datetime")
                                .HasComment("入室日時");
                            // 退室日時
                            entity.Property(c => c.LogoutTime)
                                .HasColumnType("datetime")
                                .HasComment("退室日時");
                            // 最終更新日時
                            entity.Property(c => c.LastUpdate)
                                .HasColumnType("datetime")
                                .HasComment("最終更新日時");
                            break;
                        }
                    case "pgsql":
                        {
                            logger.Debug("PostgerSQLモードで実行します。");
                            // 入室日時
                            entity.Property(c => c.LoginTime)
                                .HasColumnType("timestamp with time zone")
                                .HasComment("入室日時");
                            // 退室日時
                            entity.Property(c => c.LogoutTime)
                                .HasColumnType("timestamp with time zone")
                                .HasComment("退室日時");
                            // 最終更新日時
                            entity.Property(c => c.LastUpdate)
                                .HasColumnType("timestamp with time zone")
                                .HasComment("最終更新日時");
                            break;
                        }
                    case "mssql":
                        {
                            logger.Debug("MicrosoftSQLモードで実行します。");
                            // 入室日時
                            entity.Property(c => c.LoginTime)
                                .HasColumnType("datetimeoffset")
                                .HasComment("入室日時");
                            // 退室日時
                            entity.Property(c => c.LogoutTime)
                                .HasColumnType("datetimeoffset")
                                .HasComment("退室日時");
                            // 最終更新日時
                            entity.Property(c => c.LastUpdate)
                                .HasColumnType("datetimeoffset")
                                .HasComment("最終更新日時");
                            break;
                        }
                    default:
                        {
                            logger.Warn("DBTypeに不正な値が指定されています。設定値：{0}", dbType);
                            goto case "mysql";
                        }
                }

            });
            logger.Info("========== Func End!   ==================================================");
        }
    }
}
