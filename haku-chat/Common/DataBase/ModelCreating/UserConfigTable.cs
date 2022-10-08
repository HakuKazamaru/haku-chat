using Microsoft.EntityFrameworkCore;

using NLog;
using NLog.Web;

using haku_chat.Models;

namespace haku_chat.Common.DataBase.ModelCreating
{
    /// <summary>
    /// ユーザー設定管理テーブル
    /// </summary>
    public class UserConfigTable
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        ///  ユーザー設定管理テーブルモデル初期化メソッド
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="modelBuilder"></param>
        public static void Init(string dbType, ref ModelBuilder modelBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            // ユーザー設定管理テーブル
            modelBuilder.Entity<UserConfigModel>(entity =>
            {
                // 主キー設定
                entity.HasKey(e => e.Id);
                // テーブル名の設定
                entity.HasComment("ユーザー設定管理テーブル");

                // DB共通のカラム処理
                // ユーザーID（システム）
                entity.Property(c => c.Id)
                    .HasColumnType("varchar(95)")
                    .HasComment("ユーザーID（システム）");
                // 投稿者名文字色ID
                entity.Property(c => c.NameColorId)
                    .HasColumnType("smallint unsigned")
                    .HasComment("投稿者名文字色ID");

                // DBごとのカラム処理
                switch (dbType)
                {
                    case "mysql":
                        {
                            logger.Debug("MySQLモードで実行します。");
                            // 通知メール送信フラグ
                            entity.Property(c => c.SendInfoMail)
                                .HasColumnType("tinyint(1)")
                                .HasDefaultValue(0)
                                .HasComment("通知メール送信フラグ");

                            // チャットログ表示件数
                            entity.Property(c => c.ShowChatLogCount)
                                .HasColumnType("smallint")
                                .HasDefaultValue(0)
                                .HasComment("チャットログ表示件数");
                            // 更新日時
                            entity.Property(c => c.UpdateDatetime)
                                .HasColumnType("datetime")
                                .HasComment("更新日時");
                            break;
                        }
                    case "pgsql":
                        {
                            logger.Debug("PostgerSQLモードで実行します。");
                            // 通知メール送信フラグ
                            entity.Property(c => c.SendInfoMail)
                                .HasColumnType("boolean")
                                .HasComment("通知メール送信フラグ");

                            // チャットログ表示件数
                            entity.Property(c => c.ShowChatLogCount)
                                .HasColumnType("smallint")
                                .HasComment("チャットログ表示件数");
                            // 更新日時
                            entity.Property(c => c.UpdateDatetime)
                                .HasColumnType("timestamp")
                                .HasComment("更新日時");
                            break;
                        }
                    case "mssql":
                        {
                            logger.Debug("MicrosoftSQLモードで実行します。");
                            // 通知メール送信フラグ
                            entity.Property(c => c.SendInfoMail)
                                .HasColumnType("BIT")
                                .HasComment("通知メール送信フラグ");

                            // チャットログ表示件数
                            entity.Property(c => c.ShowChatLogCount)
                                .HasColumnType("smallint")
                                .HasComment("チャットログ表示件数");
                            // 更新日時
                            entity.Property(c => c.UpdateDatetime)
                                .HasColumnType("datetime")
                                .HasComment("更新日時");
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
