using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

using NLog;
using NLog.Web;

using haku_chat.Models;
using haku_chat.DbContexts;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace haku_chat.Controllers
{
    /// <summary>
    /// 利用規約、プライバシーポリシー、クッキーポリシー確認用ページコントローラー
    /// </summary>
    [AllowAnonymous]
    public class PolicyController : Controller
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
        public PolicyController(ChatDbContext context, SignInManager<UserMasterModel> signInManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Indexページ
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            logger.Info("========== Page Start! ==================================================");
            LobbyViewModel model = new LobbyViewModel
            {
                ReturnUrl = "",
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            PolicyModel service = new PolicyModel(), privacy = new PolicyModel(), cookie = new PolicyModel();
            // アッセンブリ情報の取得
            Assembly? assembly = Assembly.GetEntryAssembly();
            // exe配置場所取得
            string appPath = assembly.Location;
            string txtPath = Path.GetDirectoryName(appPath);

            string serviceTextPath = Common.Utility.Config.GetAppsettingsToSectionStringValue(_configuration, "ServicePolicyText");
            string privacyTextPath = Common.Utility.Config.GetAppsettingsToSectionStringValue(_configuration, "PrivacyPolicyText");
            string cookieTextPath = Common.Utility.Config.GetAppsettingsToSectionStringValue(_configuration, "CookiePolicyText");

            logger.Debug("環境情報");
            logger.Debug("-------------------------------------------------------------------------------");
            logger.Debug("カレントディレクトリ　：{0}", Directory.GetCurrentDirectory());
            logger.Debug("アプリケーションパス　：{0}", appPath);
            logger.Debug("コンフィグディレクトリ：{0}", txtPath);
            logger.Debug("-------------------------------------------------------------------------------");
            logger.Debug("利用規約　　　　　　　：{0}", serviceTextPath);
            logger.Debug("プライバシーポリシー　：{0}", privacyTextPath);
            logger.Debug("クッキーポリシー　　　：{0}", cookieTextPath);

            if (!string.IsNullOrWhiteSpace(serviceTextPath))
            {
                service.PolicyStrings = await ReadText(Path.Combine(txtPath, serviceTextPath));
                if (service.PolicyStrings != null)
                {
                    service.Use = service.PolicyStrings.Count > 0 ? true : false;
                }
                else
                {
                    service.Use = false;
                }
            }
            else
            {
                service.Use = false;
            }
            model.ServicePolicy = service;

            if (!string.IsNullOrWhiteSpace(privacyTextPath))
            {
                privacy.PolicyStrings = await ReadText(Path.Combine(txtPath, privacyTextPath));
                if (privacy.PolicyStrings != null)
                {
                    privacy.Use = privacy.PolicyStrings.Count > 0 ? true : false;
                }
                else
                {
                    privacy.Use = false;
                }
            }
            else
            {
                privacy.Use = false;
            }
            model.PrivacyPolicy = privacy;

            if (!string.IsNullOrWhiteSpace(cookieTextPath))
            {
                cookie.PolicyStrings = await ReadText(Path.Combine(txtPath, cookieTextPath));
                if (cookie.PolicyStrings != null)
                {
                    cookie.Use = cookie.PolicyStrings.Count > 0 ? true : false;
                }
                else
                {
                    cookie.Use = false;
                }
            }
            else
            {
                cookie.Use = false;
            }
            model.CookiePolicy = cookie;

            logger.Info("========== Page End!   ==================================================");
            return View(model);
        }

        /// <summary>
        /// ポリシーテキスト読み込み処理
        /// </summary>
        /// <param name="textPath"></param>
        /// <returns></returns>
        private async Task<List<PolicyString>> ReadText(string textPath)
        {
            logger.Info("========== Func Start! ==================================================");
            List<PolicyString> list = new List<PolicyString>();
            logger.Debug("ファイルパス　：{0}", textPath);

            try
            {
                using (var reader = new StreamReader(textPath))
                {
                    PolicyString policyString = new PolicyString();
                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        List<string> texts = new List<string>();
                        if (line.StartsWith("#"))
                        {
                            if (!string.IsNullOrWhiteSpace(policyString.Title))
                            {
                                list.Add(policyString);
                                policyString = new PolicyString();
                            }
                            policyString.Title = line;
                            policyString.Texts = new List<string>();
                        }
                        else
                        {
                            if (policyString.Texts == null)
                            {
                                policyString.Texts = new List<string>();
                            }
                            policyString.Texts.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, string.Format("ReadTextでエラーが発生しました。エラー：{0}", ex.Message));
                list = null;
            }

            logger.Info("========== Func End!   ==================================================");
            return list;
        }
    }
}
