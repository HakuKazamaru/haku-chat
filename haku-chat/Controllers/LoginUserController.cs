using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;

using NLog;
using NLog.Web;

using haku_chat.DbContexts;
using haku_chat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace haku_chat.Controllers
{
    /// <summary>
    /// コントローラークラス：
    /// 　ログインユーザー確認クラス
    /// </summary>
    [AllowAnonymous]
    public class LoginUserController : Controller
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ChatDbContext _context;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context"></param>
        public LoginUserController(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// API名確認
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            logger.Info("========== API Call Start! ==================================================");
            return Json(new Dictionary<string, string>() {
                { "name", "LoginUser" },
            });
        }

        /// <summary>
        /// オンラインユーザー数確認メソッド
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckOnlineUserCount()
        {
            int count = -1;
            logger.Debug("========== API Call Start! ==================================================");
            try
            {
                var loginUser = _context.ChatLoginUser.Where(clu => clu.Status == 1).ToArray();
                count = loginUser.Length;
                logger.Debug("オンライン人数：{0}", count);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                count = -1;
            }
            logger.Debug("========== API Call End!   ==================================================");
            return Json(new Dictionary<string, int>() {
                { "count", count },
            });
        }

        /// <summary>
        /// オンラインユーザー名取得メソッド
        /// </summary>
        /// <returns></returns>
        public IActionResult GetOnlineUser()
        {
            List<string> onlineUser = new List<string>();
            logger.Debug("========== API Call Start! ==================================================");
            try
            {
                int count = -1;
                var loginUser = _context.ChatLoginUser.Where(clu => clu.Status == 1).ToArray();

                count = loginUser.Length;
                logger.Debug("オンライン人数：{0}", count);

                foreach (var user in loginUser.Select((item, index) => new { item, index }))
                {
                    string userName = user.item.Name;
                    onlineUser.Add(userName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                onlineUser = null;
            }
            logger.Debug("========== API Call End!   ==================================================");
            return Json(onlineUser);
        }

        /// <summary>
        /// オンラインユーザーのタイムアウトチェック
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CheckOnlineUser()
        {
            int offline = -1;
            logger.Debug("========== API Call Start! ==================================================");
            offline = await Common.DataBase.ChatLogFunc.CheckSessionTimeout(_context);
            logger.Debug("========== API Call End!   ==================================================");
            return Json(new Dictionary<string, int>() {
                { "offline", offline },
            });
        }

    }
}
