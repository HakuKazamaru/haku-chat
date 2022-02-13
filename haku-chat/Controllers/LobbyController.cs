using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NLog;
using NLog.Web;

namespace haku_chat.Controllers
{
    /// <summary>
    /// コントローラークラス：
    /// 　入室ページ
    /// </summary>
    public class LobbyController : Controller
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// GET Lobby
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            logger.Info("========== Page Call! ==================================================");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, uint nameColor)
        {
            string retVal = "";
            int retCode = 0;

            logger.Info("========== API Call Start! ==================================================");
            retCode = LoginChatRoom(name);
            if (retCode == 0)
            {
                logger.Info("ログインに成功しました。");
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

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int LoginChatRoom(string name)
        {
            int retVal = -1;

            logger.Info("========== Func Start! ==================================================");
            try
            {
                List<Models.ChatLoginUserModel> chatLoginUserModels = Common.UserSession.GetUserLogin(name);

                if (chatLoginUserModels != null)
                {
                    if (chatLoginUserModels.Count > 0)
                    {
                        foreach (var chatLoginUserModel in chatLoginUserModels.Select((item, index) => new { item, index }))
                        {
                            if (chatLoginUserModel.item.Status == 1)
                            {
                                logger.Debug("ログイン済みを確認しました。");
                                if ((DateTime.Now - chatLoginUserModel.item.LastUpdate).TotalSeconds >= (30 * 60))
                                {
                                    logger.Debug("セッションがタイムアウトしています。");
                                    retVal = 2;
                                }
                                else
                                {
                                    logger.Debug("セッションが有効です。");
                                    retVal = 1;
                                }
                            }
                            else
                            {
                                logger.Debug("ログアウト済みを確認しました。");
                                retVal = 0;
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("ログイン情報が見つかりませんでした。");
                        retVal = 0;
                    }
                }
                else
                {
                    logger.Debug("ログイン情報が見つかりませんでした。");
                    retVal = 0;
                }

                if (retVal != 1)
                {
                    logger.Debug("ログイン処理を試行します。");
                    if (Common.UserSession.SetUserLogin(name))
                    {
                        logger.Debug("ログイン処理に成功しました。システムメッセージを投稿します。");
                        if (Common.ChatLog.PostChatLog("システム", 0, string.Format("{0}さんが入室しました。", name), 0))
                        {
                            logger.Debug("システムメッセージを投稿に成功しました。");
                            retVal = 0;
                        }
                        else
                        {
                            logger.Warn("システムメッセージの投稿に失敗しました。");
                            retVal = -1;
                        }
                    }
                    else
                    {
                        logger.Warn("ログイン処理に失敗しました。");
                        retVal = -1;
                    }
                }
                else
                {
                    logger.Warn("すでにセッションが有効です。");
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = -1;
            }
            logger.Info("========== Func End!   ==================================================");

            return retVal;
        }

    }
}
