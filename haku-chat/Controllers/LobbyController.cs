using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NLog.Web;
using haku_chat.Models;
using haku_chat.DbContexts;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace haku_chat.Controllers
{
    /// <summary>
    /// コントローラークラス：
    /// 　入室ページ
    /// </summary>
    [AllowAnonymous]
    public class LobbyController : Controller
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
        /// app.json読み取り用
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// サインインマネージャー
        /// </summary>
        private readonly SignInManager<UserMasterModel> _signInManager;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context"></param>
        public LobbyController(ChatDbContext context, SignInManager<UserMasterModel> signInManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// GET Lobby
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            logger.Info("========== Page Start! ==================================================");
            string useGuestMode = Common.Utility.Config.GetAppsettingsToSectionStringValue(_configuration, "UseGestUser").ToLower();
            LobbyViewModel model = new LobbyViewModel
            {
                ReturnUrl = "",
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("SessionClear")))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.SetString("SessionClear", "Cleared");
                return RedirectToAction(nameof(LobbyController.Index), "Lobby");
            }

            // ゲスト機能が無効な場合、ログインページへリダイレクト
            if (useGuestMode != "true")
            {
                return RedirectToAction(nameof(AuthController.LogIn), "Auth");
            }

            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                logger.Info("ログイン状態：ログイン済み");
                logger.Debug(Request.HttpContext.User.Claims);
            }
            else
            {
                logger.Info("ログイン状態：未ログイン");
            }

            logger.Info("========== Page End!   ==================================================");
            return View(model);
        }

        /// <summary>
        /// POST Lobby/Login
        ///      ログイン用API
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameColor"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string name, uint nameColor, string message = "")
        {
            string retVal = "";
            int retCode = 0;

            logger.Info("========== API Call Start! ==================================================");
            logger.Debug("Parameter[name]     :{0}", name);
            logger.Debug("Parameter[nameColor]:{0}", nameColor);
            logger.Debug("Parameter[message]  :{0}", message);

            retCode = await Common.DataBase.ChatLogFunc.LoginChatRoom(HttpContext, _context, _configuration, name, nameColor, message);

            if (retCode == 0)
            {
                logger.Info("ログインに成功しました。");

                // セッションに名前と名前の色を格納
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetInt32("NameColor", (int)nameColor);

                // サインインに必要なプリンシパルを作る
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, "cookie"),
                    new Claim(ClaimTypes.Name, name)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // 認証クッキーをレスポンスに追加
                await HttpContext.SignInAsync(principal);

                retVal = "OK";
            }
            else
            {
                logger.Warn("ログインに失敗しました。");
                if (retCode == 1)
                {
                    retVal = "NG1";
                }
                else
                {
                    retVal = "NG";
                }
            }
            logger.Info("========== API Call End!   ==================================================");

            return Json(new { result = retVal });
        }

    }
}
