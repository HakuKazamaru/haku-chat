using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Extensions;
using NLog.Extensions.Logging;
using NLog.Web;

namespace haku_chat
{
    public class Program
    {

        /// <summary>
        /// メインメソッド
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            /// <summary>
            /// ロガー
            /// </summary>
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Info("========== App Start! ==================================================");
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "例外のためにプログラムを停止しました。");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
            logger.Info("========== App End!   ==================================================");
        }

        /// <summary>
        /// ASP.NET Core 設定
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    // config.AddJsonFile("nlog.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    // logging.AddNLog(new NLogLoggingConfiguration(hostContext.Configuration.GetSection("NLog")));
                })
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string bindAddress = Common.Utility.Config.GetAppsettingsToSectionStringValue("BindAddress").ToLower();
                    webBuilder.UseStartup<Startup>();
                    if (!string.IsNullOrWhiteSpace(bindAddress))
                    {
                        webBuilder.UseUrls(bindAddress);
                    }
                });
        }
    }
}
