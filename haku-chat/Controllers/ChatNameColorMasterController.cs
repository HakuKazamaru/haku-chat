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
    public class ChatNameColorMasterController : Controller
    {
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();


        // GET: ChatNameColorMaster
        public ActionResult Index() {
            logger.Info("========== API Call! ==================================================");
            return GetAll();
        }

        // GET: ChatNameColorMaster/GetAll
        public ActionResult GetAll() {
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
                StringBuilder sqlString = new StringBuilder();

                Common.MySQLConnecter connecter = new Common.MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                );

                sqlString.Append("select ");
                sqlString.Append(" id, name, code ");
                sqlString.Append("from ");
                sqlString.Append(" chat_name_color_master ");
                sqlString.Append("order by  ");
                sqlString.Append(" id asc ");

                logger.Debug("SQL:{0}", sqlString.ToString());

                using (MySqlDataReader dataReader = connecter.ExecuteSql(sqlString.ToString()))
                {
                    while (dataReader.Read())
                    {
                        Models.ChatNameColorMasterModel chatNameColorMaster = new Models.ChatNameColorMasterModel();
                        chatNameColorMaster.Id = dataReader.GetUInt32("id");
                        chatNameColorMaster.Name = dataReader.GetString("name");
                        chatNameColorMaster.Code = dataReader.GetString("code");
                        chatNameColorMasters.Add(chatNameColorMaster);
                    }
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
