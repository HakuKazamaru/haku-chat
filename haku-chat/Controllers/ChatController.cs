using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NLog.Web;
using haku_chat.DbContexts;
using haku_chat.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace haku_chat.Controllers
{
    /// <summary>
    /// コントローラークラス：
    /// 　チャットルームページ
    /// </summary>
    public class ChatController : Controller
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
        /// ユーザーマネージャー
        /// </summary>
        private UserManager<UserMasterModel> _userManager;

        /// <summary>
        /// サインインマネージャー
        /// </summary>
        private readonly SignInManager<UserMasterModel> _signInManager;

        /// <summary>
        /// セッション情報
        /// </summary>
        private readonly Claim _claim;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context"></param>
        public ChatController(ChatDbContext context,
            UserManager<UserMasterModel> userManager,
            SignInManager<UserMasterModel> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// GET: Chat
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            string userId = "";
            string name = "";
            uint nameColor = (uint)Common.NameColor.ColorCodeID.BLACK, logCount = 5;
            logger.Info("========== Page Call! ==================================================");

            if (_claim == null || string.IsNullOrWhiteSpace(HttpContext.Session.GetString("SessionClear")))
            {
                // ログインしてないときにはホーム画面にリダイレクト
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.SetString("SessionClear", "Cleared");
                return RedirectToAction(nameof(LobbyController.Index), "Lobby");
            }

            // ユーザID取得
            userId = _claim.Value;

            // ユーザー設定取得
            var userConfig = await _context.UserConfig.SingleOrDefaultAsync(u => u.Id == userId);

            if (userConfig != null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Name")))
                {
                    name = HttpContext.Session.GetString("Name");
                }
                else
                {
                    var user = await _userManager!.FindByIdAsync(userId);
                    if (user != null)
                    {
                        name = user.UserName;
                    }
                    else
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return RedirectToAction(nameof(LobbyController.Index), "Lobby");
                    }
                }
                nameColor = userConfig.NameColorId;
                logCount = userConfig.ShowChatLogCount;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Name")))
                {
                    name = HttpContext.Session.GetString("Name");
                    nameColor = (uint)HttpContext.Session.GetInt32("NameColor");
                }
                else
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction(nameof(LobbyController.Index), "Lobby");
                }
            }

            logger.Info("Name      :{0}", name);
            logger.Info("Name Color:{0}", nameColor);

            ViewBag.Name = name;
            ViewBag.NameColor = nameColor;
            ViewBag.ShowLogCount = logCount;

            return View();
        }

        /// <summary>
        /// GET: Chat/Get
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Get()
        {
            logger.Info("========== API Call! ==================================================");
            return Get(5);
        }

        /// <summary>
        /// POST: Chat/Get
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Get(int num)
        {
            List<Models.ChatLogModel> returnLogs = new List<Models.ChatLogModel>();

            logger.Info("========== API Call Start! ==================================================");

            try
            {
                var chatLogs = _context.ChatNameColorMaster
                    .Join(
                        _context.ChatLog,
                        cncm => cncm.Id,
                        cl => cl.NameColorId,
                        (cncm, cl) => new
                        {
                            RegTime = cl.RegTime,
                            Name = cl.Name,
                            NameColorId = cncm.Id,
                            NameColorCode = cncm.Code,
                            Message = cl.Message
                        }
                    )
                    .OrderByDescending(c => c.RegTime)
                    .Take(num)
                    .ToList();

                foreach (var chatLog in chatLogs.Select((item, index) => new { item, index }))
                {
                    Models.ChatLogModel tmpChatLog = new Models.ChatLogModel();

                    tmpChatLog.RegTime = chatLog.item.RegTime;
                    tmpChatLog.Name = chatLog.item.Name;
                    tmpChatLog.NameColorId = chatLog.item.NameColorId;
                    tmpChatLog.NameColorCode = chatLog.item.NameColorCode;
                    tmpChatLog.Message = chatLog.item.Message;

                    returnLogs.Add(tmpChatLog);
                }

                logger.Debug("Result Count：{0}", returnLogs.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                returnLogs = null;
            }

            logger.Info("========== API Call End!   ==================================================");

            return Json(returnLogs);
        }

        /// <summary>
        /// POST: Chat/Post
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nameColor"></param>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(string name, uint nameColor, string message, uint messageColor)
        {
            string retVal = "";
            Models.ChatLogModel chatLogModel = new Models.ChatLogModel();

            logger.Info("========== API Call Start! ==================================================");

            try
            {
                logger.Debug("Parameter[name]        :{0}", name);
                logger.Debug("Parameter[nameColor]   :{0}", nameColor);
                logger.Debug("Parameter[message]     :{0}", message);
                logger.Debug("Parameter[messageColor]:{0}", messageColor);

                chatLogModel.RegTime = DateTime.Now;
                chatLogModel.Name = name;
                chatLogModel.NameColorId = nameColor;
                chatLogModel.Message = message;

                _context.ChatLog.Add(chatLogModel);

                // セッションが有効な場合はDBのセッションを更新
                if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Name")))
                {
                    string userName = HttpContext.Session.GetString("Name");
                    logger.Debug("Session parameter[Name]:{0}", userName);
                    var chatLoginUser = await _context.ChatLoginUser.SingleOrDefaultAsync(x => x.Name == userName);
                    if (chatLoginUser != null)
                    {
                        chatLoginUser.LastUpdate = DateTime.Now;
                    }
                }
                else
                {
                    logger.Warn("Session parameter data is NULL![Name]");
                }

                _context.SaveChanges();

                retVal = "OK";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = "NG";
            }

            logger.Info("========== API Call End!   ==================================================");

            return Json(new { result = retVal });
        }

        /// <summary>
        /// POST: Chat/Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            logger.Info("========== Page Call! ==================================================");

            try
            {
                // TODO: Add insert logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}