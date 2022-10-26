using Microsoft.Extensions.Configuration;
using System;
using System.IO;

using NLog;
using NLog.Web;

using Models = haku_chat.Models;

namespace haku_chat.Common.Utility
{
    /// <summary>
    /// アプリケーション設定補助クラス
    /// </summary>
    public class Config
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// 設定値管理用オブジェクト
        /// </summary>
        public Models.CofingModel AppConfig { get; set; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public Config()
        {
            AppConfig = new Models.CofingModel();

            AppConfig.ConnectionStrings = GetConnectionStrings();
            AppConfig.DBType = GetAppsettingsToSectionStringValue("DBType");
        }

        /// <summary>
        /// appsettings.jsonの"appsettings"セクションから
        /// 指定したセクションの設定値を文字列で取得
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetAppsettingsToSectionStringValue(string sectionName)
        {
            string sectionValue = string.Empty;

            logger.Info("========== Func Start! ==================================================");
            logger.Debug("Parameter[sectionName]：{0}", sectionName);

            try
            {
                var configuration =
                    new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true).Build();
                IConfigurationSection section = configuration.GetSection("appSettings");

                sectionValue = section[sectionName];
                logger.Debug("Value                 ：{0}", sectionValue);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                sectionValue = string.Empty;
            }

            logger.Info("========== Func End!   ==================================================");

            return sectionValue;
        }

        /// <summary>
        /// appsettings.jsonの"appsettings"セクションから
        /// 指定したセクションの設定値を文字列で取得
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetAppsettingsToSectionStringValue(IConfiguration configure, string sectionName)
        {
            string sectionValue = string.Empty;

            logger.Info("========== Func Start! ==================================================");
            logger.Debug("Parameter[sectionName]：{0}", sectionName);

            try
            {
                IConfigurationSection section = configure.GetSection("appSettings");
                sectionValue = section[sectionName];
                logger.Debug("Value                 ：{0}", sectionValue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                sectionValue = string.Empty;
            }

            logger.Info("========== Func End!   ==================================================");

            return sectionValue;
        }

        /// <summary>
        /// DB接続文字列取得メソッド
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static string GetConnectionStrings(string sectionName = "Context")
        {
            string sectionValue = string.Empty;

            logger.Info("========== Func Start! ==================================================");
            logger.Debug("Parameter[sectionName]：{0}", sectionName);

            try
            {
                var configuration =
                    new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true).Build();
                IConfigurationSection section = configuration.GetSection("ConnectionStrings");

                sectionValue = section[sectionName];
                logger.Debug("Value                 ：{0}", sectionValue);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                sectionValue = string.Empty;
            }

            logger.Info("========== Func End!   ==================================================");

            return sectionValue;
        }

    }
}
