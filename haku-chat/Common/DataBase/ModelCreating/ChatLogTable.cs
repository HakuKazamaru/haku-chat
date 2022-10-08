using Microsoft.EntityFrameworkCore;

using NLog;
using NLog.Web;

using haku_chat.Models;

namespace haku_chat.Common.DataBase.ModelCreating
{
    /// <summary>
    /// チャットログ管理テーブル用クラス
    /// </summary>
    public class ChatLogTable
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// チャットログ管理テーブルモデル初期化メソッド
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="modelBuilder"></param>
        public static void Init(string dbType, ref ModelBuilder modelBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            // チャットログ管理テーブル
            modelBuilder.Entity<ChatLogModel>(entity =>
            {
                // 主キー設定
                entity.HasKey(e => e.RegTime);
                // テーブル名の設定
                entity.HasComment("チャットログ管理テーブル");

                // DB共通のカラム処理
                // 投稿者名
                entity.Property(c => c.Name)
                    .HasColumnType("varchar(100)")
                    .HasComment("投稿者名");
                // 投稿者文字色ID
                entity.Property(c => c.NameColorId)
                    .HasColumnType("smallint unsigned")
                    .HasComment("投稿者文字色ID");

                // DBごとのカラム処理
                switch (dbType)
                {
                    case "mysql":
                        {
                            logger.Debug("MySQLモードで実行します。");
                            // 投稿日時
                            entity.Property(c => c.RegTime)
                                .HasColumnType("datetime(3)")
                                .HasComment("投稿日時");
                            // チャットメッセージ
                            entity.Property(c => c.Message)
                                .HasColumnType("text")
                                .HasComment("チャットメッセージ");
                            break;
                        }
                    case "pgsql":
                        {
                            logger.Debug("PostgerSQLモードで実行します。");
                            // 投稿日時
                            entity.Property(c => c.RegTime)
                                .HasColumnType("timestamp with time zone")
                                .HasComment("投稿日時");
                            // チャットメッセージ
                            entity.Property(c => c.Message)
                                .HasColumnType("text")
                                .HasComment("チャットメッセージ");
                            break;
                        }
                    case "mssql":
                        {
                            logger.Debug("MicrosoftSQLモードで実行します。");
                            // 投稿日時
                            entity.Property(c => c.RegTime)
                                .HasColumnType("datetimeoffset")
                                .HasComment("投稿日時");
                            // チャットメッセージ
                            entity.Property(c => c.Message)
                                .HasColumnType("varchar(MAX)")
                                .HasComment("チャットメッセージ");
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
