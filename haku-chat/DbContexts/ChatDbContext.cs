using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MLog = Microsoft.Extensions.Logging;

using NLog;
using NLog.Web;
using NLog.Extensions.Logging;

using haku_chat.Common.DataBase.ModelCreating;
using haku_chat.Common.Utility;
using haku_chat.Models;

namespace haku_chat.DbContexts
{
    /// <summary>
    /// チャットDBコンテキストクラス
    /// </summary>
    public class ChatDbContext : DbContext
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// 使用DB
        /// </summary>
        private string dbType = "mysql";
        
        /// <summary>
        /// データベース接続文字列
        /// </summary>
        private string connectionString = "server=192.168.101.251;database=haku-chat-db;user=haku-chat;password=haku-chat";

        #region テーブルモデル宣言
        /// <summary>
        /// チャットログ管理テーブル
        /// </summary>
        public DbSet<ChatLogModel> ChatLog { get; set; }
        /// <summary>
        /// チャット入室ステータステーブル
        /// </summary>
        public DbSet<ChatLoginUserModel> ChatLoginUser { get; set; }
        /// <summary>
        /// 投稿者名文字色マスターテーブル
        /// </summary>
        public DbSet<ChatNameColorMasterModel> ChatNameColorMaster { get; set; }
        /// <summary>
        /// ユーザーマスターテーブル
        /// </summary>
        public DbSet<UserMasterModel> UserMaster { get; set; }
        /// <summary>
        /// ユーザー設定管理テーブル
        /// </summary>
        public DbSet<UserConfigModel> UserConfig { get; set; }
        #endregion

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="options"></param>
        public ChatDbContext(DbContextOptions options) : base(options)
        {
            logger.Info("========== Func Start! ==================================================");
            // 使用するDBの指定
            dbType = Config.GetAppsettingsToSectionStringValue("DBType");
            connectionString = Config.GetConnectionStrings();

            logger.Info("使用データベース：{0}", dbType);
            if (dbType != "mysql" && dbType != "pgsql" && dbType != "mssql")
            {
                dbType = "mysql";
            }
            logger.Info("========== Func End!   ==================================================");
        }

        /// <summary>
        /// コンテキスト初期化時処理
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            // ロガーの設定
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new NLogLoggerProvider())
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name &&
                        level == MLog.LogLevel.Information);
            });

            // DB接続先指定
            switch (dbType)
            {
                case "mysql":
                    {
                        logger.Info("MySQLモードで実行します。");
                        optionsBuilder
                            .UseMySQL(connectionString)
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging()
                            .UseLoggerFactory(loggerFactory);
                        break;
                    }
                case "pgsql":
                    {
                        logger.Info("PostgerSQLモードで実行します。");
                        optionsBuilder
                            .UseNpgsql(connectionString)
                            .EnableSensitiveDataLogging()
                            .UseLoggerFactory(loggerFactory);
                        break;
                    }
                case "mssql":
                    {
                        logger.Info("MicrosoftSQLモードで実行します。");
                        optionsBuilder
                            .UseSqlServer(connectionString)
                            .EnableSensitiveDataLogging()
                            .UseLoggerFactory(loggerFactory);
                        break;
                    }
                default:
                    {
                        logger.Warn("DBTypeに不正な値が指定されています。設定値：{0}", dbType);
                        goto case "mysql";
                    }
            }

            logger.Info("========== Func End!   ==================================================");
        }

        /// <summary>
        /// データモデル生成時処理
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            logger.Info("========== Func Start! ==================================================");
            base.OnModelCreating(modelBuilder);

            // 各データモデルの初期化
            ChatLogTable.Init(dbType, ref modelBuilder);
            ChatNameColorMasterTable.Init(dbType, ref modelBuilder);
            ChatLoginUserTable.Init(dbType, ref modelBuilder);
            UserMasterTable.Init(dbType, ref modelBuilder);
            UserConfigTable.Init(dbType, ref modelBuilder);

            logger.Info("========== Func End!   ==================================================");
        }

    }
}
