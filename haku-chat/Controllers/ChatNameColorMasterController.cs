using Microsoft.AspNetCore.Authorization;
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

namespace haku_chat.Controllers
{
    /// <summary>
    /// コントローラークラス：
    /// 　投稿者名文字カラーコードマスター
    /// </summary>
    [AllowAnonymous]
    public class ChatNameColorMasterController : Controller
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
        public ChatNameColorMasterController(ChatDbContext context)
        {
            _context = context;
        }

        // GET: ChatNameColorMaster
        public IActionResult Index()
        {
            logger.Info("========== API Call! ==================================================");
            return GetAll();
        }

        // GET: ChatNameColorMaster/GetAll
        public IActionResult GetAll()
        {
            List<Models.ChatNameColorMasterModel> chatNameColorMasters = new List<Models.ChatNameColorMasterModel>();

            logger.Info("========== API Call Start! ==================================================");
            chatNameColorMasters = GetChatNameColorMaster();
            logger.Info("========== API Call End!   ==================================================");

            return Json(chatNameColorMasters);
        }

        private List<Models.ChatNameColorMasterModel> GetChatNameColorMaster()
        {
            List<Models.ChatNameColorMasterModel> chatNameColorMasters = new List<Models.ChatNameColorMasterModel>();

            logger.Info("========== Func Start! ==================================================");
            try
            {
                chatNameColorMasters = _context.ChatNameColorMaster.ToList();

                foreach (var tmpChatNameColorMaster in chatNameColorMasters.Select((Item, Index) => new { Item, Index }))
                {
                    logger.Trace("[{0}]============================================================", tmpChatNameColorMaster.Index);
                    logger.Trace("[{0}]Id   :{1}", tmpChatNameColorMaster.Index, tmpChatNameColorMaster.Item.Id);
                    logger.Trace("[{0}]Name :{1}", tmpChatNameColorMaster.Index, tmpChatNameColorMaster.Item.Name);
                    logger.Trace("[{0}]Code :{1}", tmpChatNameColorMaster.Index, tmpChatNameColorMaster.Item.Code);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            logger.Info("========== Func End!   ==================================================");

            return chatNameColorMasters;
        }
    }
}
