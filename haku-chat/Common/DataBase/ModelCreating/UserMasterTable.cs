using Microsoft.EntityFrameworkCore;

using NLog;
using NLog.Web;

using haku_chat.Models;

namespace haku_chat.Common.DataBase.ModelCreating
{
    /// <summary>
    /// ユーザーマスターテーブル用クラス
    /// </summary>
    public class UserMasterTable
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        ///  ユーザーマスターテーブルモデル初期化メソッド
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="modelBuilder"></param>
        public static void Init(string dbType, ref ModelBuilder modelBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            // ユーザーマスターテーブル
            modelBuilder.Entity<UserMasterModel>(entity =>
            {
                // 主キー設定
                entity.HasKey(e => e.Id);
                // テーブル名の設定
                entity.HasComment("ユーザーマスターテーブル");

                // DB共通のカラム処理
                // ユーザーID（システム）
                entity.Property(c => c.Id)
                    .HasColumnType("varchar(95)")
                    .HasComment("ユーザーID（システム）");

                // ユーザーID（ユーザー）
                entity.Property(c => c.UserId)
                    .HasColumnType("varchar(64)")
                    .HasComment("ユーザーID（ユーザー）");

                // ユーザー名
                entity.Property(c => c.UserName)
                    .HasColumnType("varchar(64)")
                    .HasComment("ユーザー名");

                // メールアドレス
                entity.Property(c => c.Email)
                    .HasColumnType("varchar(256)")
                    .HasComment("メールアドレス");

                // パスワード
                entity.Property(c => c.Password)
                    .HasColumnType("varchar(256)")
                    .HasComment("パスワード");

                // パスワードハッシュ
                entity.Property(c => c.PasswordHash)
                    .HasColumnType("varchar(256)")
                    .HasComment("パスワードハッシュ");

            });
            logger.Info("========== Func End!   ==================================================");
        }
    }
}
