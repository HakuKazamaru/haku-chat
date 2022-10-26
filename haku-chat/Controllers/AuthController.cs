using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NuGet.Protocol;

using NLog;
using NLog.Web;

using haku_chat.Common;
using haku_chat.DbContexts;
using haku_chat.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace haku_chat.Controllers
{
    /// <summary>
    /// 認証処理ページ
    /// </summary>
    [AllowAnonymous]
    public class AuthController : Controller
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
        /// 認証スキームプロバイダー
        /// </summary>
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        /// <summary>
        /// ユーザーマネージャー
        /// </summary>
        private UserManager<UserMasterModel> _userManager;

        /// <summary>
        /// サインインマネージャー
        /// </summary>
        private SignInManager<UserMasterModel> _signInManager;

        /// <summary>
        /// セッション情報
        /// </summary>
        private readonly Claim _claim;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="authenticationSchemeProvider"></param>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="httpContextAccessor"></param>
        public AuthController(
            IAuthenticationSchemeProvider authenticationSchemeProvider,
            ChatDbContext context,
            UserManager<UserMasterModel> userManager,
            SignInManager<UserMasterModel> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
            )
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            _configuration = configuration;
        }

        /// <summary>
        /// SSOコールバック処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SignInCallback(string returnUrl = null, string remoteError = null)
        {
            //returnUrlがnullだったらルートUrlを返す
            returnUrl = returnUrl ?? Url.Content("~/");

            LobbyViewModel loginViewModel = new LobbyViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            //外部認証プロバイダからエラーが帰ってきている場合は,
            //ログインページに戻す
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider:{remoteError}");

                return RedirectToAction(nameof(LobbyController.Index), "Lobby");
            }

            //signInManagerがOAuthの認証結果を持っているので,それを取得する
            var info = await _signInManager.GetExternalLoginInfoAsync();
            //空だったらログインページ行き
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return RedirectToAction(nameof(LobbyController.Index), "Lobby");
            }

            //これはこのアプリケーション側のAspNetUserLoginsテーブルにログインユーザーとして登録するためか?
            var signInResult = await _signInManager
                .ExternalLoginSignInAsync(info.LoginProvider,
                    info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            //わかった.
            // AspNetUserLoginsテーブルは外部認証によるログイン情報が格納されるんだ.
            //初回は記録がないからfalse

            if (signInResult.Succeeded)
            {
                //以前にその外部認証でログインしたことがある.
                //問題なし
                return LocalRedirect(returnUrl);
            }
            else
            {
                //signInResultがfalseってどういうとき?
                //初めて外部認証でログインしたユーザーの場合だ.こっちは.

                //メールアドレスを取得する.
                var userId = string.Format("{0}:{1}", info.LoginProvider, info.ProviderKey);

                // このアプリにアカウントを持っているかどうかをEmailで検索する.
                //内部のユーザー情報を検索する
                if (userId != null)
                {
                    var user = await _userManager.FindByIdAsync(userId);

                    //user==nullとは,そんなユーザーはいない...の場合
                    //つまり,このアプリを初めて使うユーザーで,
                    //外部認証を使った人.

                    if (user == null)
                    {
                        user = new UserMasterModel
                        {
                            Id = userId,
                            UserId = info.ProviderKey,
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Name)
                        };
                        //このアプリ用のユーザー作成
                        await _userManager.CreateAsync(user);
                    }

                    //AspNetUserLoginsテーブルに追加.
                    await _userManager.AddLoginAsync(user, info);
                    //このアプリにおけるサインイン状態にする
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
            }

            return RedirectToAction(nameof(ChatController.Index), "Lobby");
        }

        /// <summary>
        /// ログイン処理（SSO）
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public IActionResult SignIn(string provider)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = Url.Action(nameof(SignInCallback)) }, provider);
        }

        /// <summary>
        /// ログイン処理（SSO）
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignIn(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(nameof(SignInCallback), new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// ログインページ
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LogIn(string returnUrl = null)
        {
            logger.Info("========== Page Start! ==================================================");
            //returnUrlがnullだったらルートUrlを返す
            returnUrl = returnUrl ?? Url.Content("~/");
            LobbyViewModel model = new LobbyViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            logger.Info("========== Page End!   ==================================================");
            return View(model);
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogIn(string email, string password, bool rememberMe = false)
        {
            logger.Info("========== Page Start! ==================================================");
            LobbyViewModel model = new LobbyViewModel
            {
                ReturnUrl = "~/",
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            logger.Debug("Parameter[email]   :{0}", email);
            logger.Debug("Parameter[password]:{0}", password);

            // 入力チェック
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                string errMsg = "{0}が入力されていません。";
                string errObj = "";

                // 入力項目のいずれかが空欄
                if (string.IsNullOrWhiteSpace(email))
                {
                    errObj += "Email Address";
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    if (!string.IsNullOrWhiteSpace(errObj))
                    {
                        errObj += ", ";
                    }
                    errObj += "Password";
                }

                // セッションにエラーメッセージを格納
                ViewBag.ErrorMessage = string.Format(errMsg, errObj);
            }
            else
            {
                var user = await _userManager!.FindByEmailAsync(email);
                if (user != null)
                {
                    // var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
                    var result = await _signInManager!.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        int retCode = 0;

                        logger.Debug("対象ユーザーはログインできます。UserID：{0}", email);

                        // UserMasterModel userMasterModel = new UserMasterModel();
                        // userMasterModel.UserName = email;

                        string id = await _userManager.GetUserIdAsync(user);
                        string name = await _userManager.GetUserNameAsync(user);

                        retCode = await Common.DataBase.ChatLogFunc.LoginChatRoom(HttpContext, _context, _configuration, name);

                        if (retCode == 0)
                        {

                            // セッションにユーザー情報を格納
                            HttpContext.Session.SetString("SessionClear", "Cleared");
                            HttpContext.Session.SetString("Name", name);
                            HttpContext.Session.SetInt32("NameColor", (int)Common.NameColor.ColorCodeID.BLACK);

                            // サインインに必要なプリンシパルを作る
                            var claims = new[] {
                                new Claim(ClaimTypes.NameIdentifier, id),
                                new Claim(ClaimTypes.Name, name)
                            };
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // 認証クッキーをレスポンスに追加
                            await HttpContext.SignInAsync(principal);

                            return RedirectToAction(nameof(ChatController.Index), "Chat");
                        }
                        else
                        {
                            string strMsg = "";
                            logger.Error("ログインに失敗しました。");
                            if (retCode == 1)
                            {
                                strMsg = "NG1";
                            }
                            else
                            {
                                strMsg = "NG";
                            }
                            logger.Error("エラータイプ：{0}", strMsg);
                            return Problem(strMsg);
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "「Email Address」、もしくは「Password」が誤っています。";
                        logger.Debug("対象ユーザーはログインできません。UserID：{0}", email);
                        logger.Debug("IsNotAllowed     ：{0}", result.IsNotAllowed);
                        logger.Debug("IsLockedOut      ：{0}", result.IsLockedOut);
                        logger.Debug("RequiresTwoFactor：{0}", result.RequiresTwoFactor);
                        logger.Debug("ResultJson       ：{0}", result.ToJson());
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "「Email Address」、もしくは「Password」が誤っています。";
                    logger.Debug("対象ユーザーは存在しません。UserID：{0}", email);
                }
            }
            return View(model);
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LogOut(string name = "")
        {
            logger.Info("========== API Call Start! ==================================================");
            logger.Debug("Parameter[name]     :{0}", name);

            // 認証クッキーをレスポンスから削除
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!string.IsNullOrWhiteSpace(name))
            {
                int retCode = await Common.DataBase.ChatLogFunc.LogoutChatRoom(HttpContext, _context, name);

                HttpContext.Session.Clear();

                if (retCode == 0)
                {
                    logger.Info("ログアウトに成功しました。");
                    return RedirectToAction(nameof(LobbyController.Index), "Lobby");
                }
                else
                {
                    string retVal = "";
                    logger.Warn("ログアウトに失敗しました。");
                    if (retCode == 1)
                    {
                        retVal = "NG1";
                    }
                    else
                    {
                        retVal = "NG";
                    }
                    logger.Error("エラータイプ：{0}", retVal);

                    return Problem(retVal);
                }

            }
            else
            {
                name = HttpContext.Session.GetString("Name");
                HttpContext.Session.Clear();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    int retCode = await Common.DataBase.ChatLogFunc.LogoutChatRoom(HttpContext, _context, name);

                    if (retCode == 0)
                    {
                        logger.Info("ログアウトに成功しました。");
                        return RedirectToAction(nameof(LobbyController.Index), "Lobby");
                    }
                    else
                    {
                        string retVal = "";
                        logger.Warn("ログアウトに失敗しました。");
                        if (retCode == 1)
                        {
                            retVal = "NG1";
                        }
                        else
                        {
                            retVal = "NG";
                        }
                        logger.Error("エラータイプ：{0}", retVal);

                        return Problem(retVal);
                    }
                }
            }
            return RedirectToAction(nameof(LobbyController.Index), "Lobby");
        }

        /// <summary>
        /// ユーザー登録ページ
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            logger.Info("========== Page Start! ==================================================");
            //returnUrlがnullだったらルートUrlを返す
            returnUrl = returnUrl ?? Url.Content("~/");
            LobbyViewModel model = new LobbyViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (_claim != null)
            {
                // ユーザID取得
                string userId = _claim.Value;
                if (userId == "cookie")
                {
                    ViewBag.Login = true;
                }
                else
                {
                    return RedirectToAction(nameof(ChatController.Index), "Chat");
                }
            }
            else
            {
                ViewBag.Login = false;
            }

            logger.Info("========== Page End!   ==================================================");
            return View(model);
        }

        /// <summary>
        /// ユーザー登録処理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            logger.Info("========== Page Start! ==================================================");
            LobbyViewModel model = new LobbyViewModel
            {
                ReturnUrl = "~/",
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            logger.Debug("Parameter[name]    :{0}", name);
            logger.Debug("Parameter[email]   :{0}", email);
            logger.Debug("Parameter[password]:{0}", password);

            // 入力チェック
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                string errMsg = "{0}が入力されていません。";
                string errObj = "";

                // 入力項目のいずれかが空欄
                if (string.IsNullOrWhiteSpace(name))
                {
                    errObj += "User Name";
                }
                if (string.IsNullOrWhiteSpace(email))
                {
                    if (!string.IsNullOrWhiteSpace(errObj))
                    {
                        errObj += ", ";
                    }
                    errObj += "Email Address";
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    if (!string.IsNullOrWhiteSpace(errObj))
                    {
                        errObj += ", ";
                    }
                    errObj += "Password";
                }

                // セッションにエラーメッセージを格納
                ViewBag.ErrorMessage = string.Format(errMsg, errObj);
            }
            else
            {
                // 存在確認
                if (await _userManager.FindByEmailAsync(email) == null)
                {
                    UserMasterModel userMasterModel = new UserMasterModel
                    {
                        UserId = email,
                        UserName = name,
                        Email = email,
                        Password = password,
                    };

                    var result = await _userManager.CreateAsync(userMasterModel, password);

                    if (result.Succeeded)
                    {
                        // セッションにユーザー情報を格納
                        HttpContext.Session.SetString("Name", name);
                        HttpContext.Session.SetInt32("NameColor", (int)Common.NameColor.ColorCodeID.BLACK);

                        // サインインに必要なプリンシパルを作る
                        var claims = new[] { new Claim(ClaimTypes.Name, name) };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // 認証クッキーをレスポンスに追加
                        await HttpContext.SignInAsync(principal);

                        return RedirectToAction(nameof(ChatController.Index), "Chat");
                    }
                    else
                    {
                        logger.Error(result.Errors.First().Code);
                        logger.Error(result.Errors.First().Description);
                        ViewBag.ErrorMessage = "ユーザー登録に失敗しました。";
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "入力された「Email Address」はすでに登録済みです。";
                }
            }

            logger.Info("========== Page End!   ==================================================");
            return View(model);
        }

        /// <summary>
        /// 登録解除処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UnRegister()
        {
            return await LogOut();
        }

        /// <summary>
        /// ログインセッション確認処理
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckLoginSession(string userName)
        {
            string returnVal = "NULL";
            int checkStatus = -1;
            logger.Debug("========== Api Start! ==================================================");

            checkStatus = await Common.DataBase.ChatLogFunc.CheckSessionTimeout(_context, userName);
            logger.Debug("Check status     :{0}", checkStatus);

            switch (checkStatus)
            {
                case (int)Login.LoginSessionStatus.SESSION_ONLINE:
                    {
                        returnVal = "ONLINE";
                        break;
                    }
                case (int)Login.LoginSessionStatus.SESSION_OFFLINE
                  or (int)Login.LoginSessionStatus.SESSION_TIMEOUT
                  or (int)Login.LoginSessionStatus.SESSION_NOTFOUND:
                    {
                        returnVal = "OFFLINE";
                        break;
                    }
                default:
                    {
                        returnVal = "UNKOWN_ERROR";
                        break;
                    }
            }
            logger.Debug("Check status name:{0}", returnVal);

            logger.Debug("========== Api End!   ==================================================");
            return Json(new Dictionary<string, string>() {
                { "result", returnVal },
            });
        }

    }
}