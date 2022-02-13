using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace haku_chat
{
    public class Program
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// メインメソッド
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            logger.Info("========== App Start! ==================================================");
            CreateWebHostBuilder(args).Build().Run();
            logger.Info("========== App End!   ==================================================");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
