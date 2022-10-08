using Microsoft.EntityFrameworkCore;

using NLog;
using NLog.Web;

using haku_chat.Models;

namespace haku_chat.Common.DataBase.ModelCreating
{
    /// <summary>
    /// 投稿者文字色マスターテーブル用クラス
    /// </summary>
    public class ChatNameColorMasterTable
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        ///  投稿者文字色マスターテーブルモデル初期化メソッド
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="modelBuilder"></param>
        public static void Init(string dbType, ref ModelBuilder modelBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            // 投稿者文字色マスターテーブル
            modelBuilder.Entity<ChatNameColorMasterModel>(entity =>
            {
                // 主キー設定
                entity.HasKey(e => e.Id);
                // テーブル名の設定
                entity.HasComment("投稿者文字色マスターテーブル");

                // DB共通のカラム処理
                // 投稿者名文字色ID
                entity.Property(c => c.Id)
                    .HasColumnType("smallint unsigned")
                    .HasComment("投稿者名文字色ID");

                // 投稿者名文字色名称
                entity.Property(c => c.Name)
                    .HasColumnType("varchar(32)")
                    .HasComment("投稿者名文字色名称");

                // 投稿者名文字色カラーコード
                entity.Property(c => c.Code)
                    .HasColumnType("varchar(32)")
                    .HasComment("投稿者名文字色カラーコード");
            });
            logger.Info("========== Func End!   ==================================================");
        }
    }
}
