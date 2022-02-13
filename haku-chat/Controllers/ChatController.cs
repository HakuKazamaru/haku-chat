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
    /// 　チャットルームページ
    /// </summary>
    public class ChatController : Controller
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// GET: Chat
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            logger.Info("========== Page Call! ==================================================");
            return View();
        }

        /// <summary>
        /// GET: Chat/Get
        /// </summary>
        /// <returns></returns>
        public ActionResult Get()
        {
            logger.Info("========== API Call! ==================================================");
            return Get(5);
        }

        /// <summary>
        /// POST: Chat/Get
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Get(int num)
        {
            List<Models.ChatLogModel> returnLogs = new List<Models.ChatLogModel>();
            List<Models.ChatLogModel> chatLogs = new List<Models.ChatLogModel>();

            logger.Info("========== API Call Start! ==================================================");

            chatLogs = Common.ChatLog.GetChatLog(num);

            foreach (var chatLog in chatLogs.Select((item, index) => new { item, index }))
            {
                if (chatLog.index >= num)
                {
                    break;
                }
                returnLogs.Add(chatLog.item);
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
        [HttpPost]
        public ActionResult Post(string name, uint nameColor, string message, uint messageColor)
        {
            string retVal = "";
            
            logger.Info("========== API Call Start! ==================================================");
            
            if (Common.ChatLog.PostChatLog(name, nameColor, message, messageColor))
            {
                retVal = "OK";
            }
            else
            {
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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