using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using NLog;
using NLog.Web;

using haku_chat.DbContexts;
using haku_chat.Models;

namespace haku_chat.Controllers
{
    /// <summary>
    /// コントローラークラス：
    /// 　ユーザー設定管理ページ
    /// </summary>
    public class UserConfigController : Controller
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
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="httpContextAccessor"></param>
        public UserConfigController(ChatDbContext context,
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
        /// GET: UserConfig
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            string userId = "";
            logger.Info("========== Page Start! ==================================================");

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

            if (userConfig == null && userId != "cookie")
            {
                // ユーザー設定が存在しない場合、新規作成
                UserConfigModel tmpUserConfig = new UserConfigModel();
                tmpUserConfig.Id = userId;
                tmpUserConfig.NameColorId = (uint)Common.NameColor.ColorCodeID.BLACK;
                tmpUserConfig.ShowChatLogCount = 5;
                tmpUserConfig.SendInfoMail = false;
                tmpUserConfig.UpdateDatetime = DateTime.UtcNow;

                _context.Add(tmpUserConfig);
                await _context.SaveChangesAsync();

                return View(tmpUserConfig);
            }
            else if (userId == "cookie")
            {
                return RedirectToAction(nameof(AuthController.Register), "Auth");
            }
            else
            {
                return View(userConfig);
            }
        }

        /// <summary>
        /// GET: UserConfig/Details/{ID}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(string id)
        {
            logger.Info("========== Page Start! ==================================================");
            if (id == null || _context.UserConfig == null)
            {
                return NotFound();
            }

            var userConfigModel = await _context.UserConfig
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userConfigModel == null)
            {
                return NotFound();
            }

            return View(userConfigModel);
        }

        /// <summary>
        /// GET: UserConfig/Create
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            logger.Info("========== Page Start! ==================================================");
            return View();
        }

        /// <summary>
        /// POST: UserConfig/Create
        /// To protect from overposting attacks, enable the specific properties you want to bind to.
        /// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="userConfigModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameColorId,SendInfoMail,ShowChatLogCount,UpdateDatetime")] UserConfigModel userConfigModel)
        {
            logger.Info("========== Page Start! ==================================================");
            if (ModelState.IsValid)
            {
                _context.Add(userConfigModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userConfigModel);
        }

        /// <summary>
        /// GET: UserConfig/Edit/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string id)
        {
            logger.Info("========== Page Start! ==================================================");
            if (id == null || _context.UserConfig == null)
            {
                return NotFound();
            }

            var userConfigModel = await _context.UserConfig.FindAsync(id);
            if (userConfigModel == null)
            {
                return NotFound();
            }
            return View(userConfigModel);
        }

        /// <summary>
        /// POST: UserConfig/Edit/5
        /// To protect from overposting attacks, enable the specific properties you want to bind to.
        /// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userConfigModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,NameColorId,SendInfoMail,ShowChatLogCount,UpdateDatetime")] UserConfigModel userConfigModel)
        {
            logger.Info("========== Page Start! ==================================================");
            if (id != userConfigModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    userConfigModel.UpdateDatetime = DateTime.UtcNow;
                    _context.Update(userConfigModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserConfigModelExists(userConfigModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userConfigModel);
        }

        /// <summary>
        /// GET: UserConfig/Delete/5
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(string id)
        {
            logger.Info("========== Page Start! ==================================================");
            if (id == null || _context.UserConfig == null)
            {
                return NotFound();
            }

            var userConfigModel = await _context.UserConfig
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userConfigModel == null)
            {
                return NotFound();
            }

            return View(userConfigModel);
        }

        /// <summary>
        /// POST: UserConfig/Delete/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            logger.Info("========== Page Start! ==================================================");
            if (_context.UserConfig == null)
            {
                return Problem("Entity set 'ChatDbContext.UserConfig'  is null.");
            }
            var userConfigModel = await _context.UserConfig.FindAsync(id);
            if (userConfigModel != null)
            {
                _context.UserConfig.Remove(userConfigModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 指定したIDのレコードが存在しているか確認
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserConfigModelExists(string id)
        {
            logger.Info("========== Page Start! ==================================================");
            return _context.UserConfig.Any(e => e.Id == id);
        }
    }
}
