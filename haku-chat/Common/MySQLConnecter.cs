using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NLog;
using NLog.Web;

namespace haku_chat.Common
{
    public class MySQLConnecter : IDisposable
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DataBase { get; set; }

        private string ConnectionString = "Server={0};Database={1};Uid={2};Pwd={3}";

        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        private MySqlConnection connection = new MySqlConnection();

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MySQLConnecter()
        {
            Server = "localhost";
            Port = "3306";
            User = "root";
            Password = "";
            DataBase = "mysql";
        }

        /// <summary>
        /// コンストラクター(引数あり)
        /// </summary>
        public MySQLConnecter(
            string server = "localhost",
            string port = "3306",
            string user = "root",
            string password = "",
            string database = "mysql")
        {
            Server = server;
            Port = port;
            User = user;
            Password = password;
            DataBase = database;
        }

        /// <summary>
        /// MySQLサーバに接続する。
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                if (connection.State != ConnectionState.Open || connection.State != ConnectionState.Executing || connection.State != ConnectionState.Fetching)
                {
                    this.connection.ConnectionString = string.Format(ConnectionString, Server, DataBase, User, Password);
                    this.connection.Open();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 返り値を伴うSQLを実行する
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public MySqlDataReader ExecuteSql(string sql, List<MySqlParameter> parameters = null)
        {
            try
            {
                if (Connect())
                {
                    MySqlCommand command = new MySqlCommand(sql, this.connection);
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    MySqlDataReader dataReader = command.ExecuteReader();
                    return dataReader;
                }
                else
                {
                    logger.Error("DBに接続できませんでした。");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 返り値を伴うSQLを実行する
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteSqlNonQuery(string sql, List<MySqlParameter> parameters = null)
        {
            int returnVal = -1;
            try
            {
                if (Connect())
                {
                    MySqlCommand command = new MySqlCommand(sql, this.connection);
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    returnVal = command.ExecuteNonQuery();
                    return returnVal;
                }
                else
                {
                    logger.Error("DBに接続できませんでした。");
                    return returnVal;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return returnVal;
            }
        }

        /// <summary>
        /// デスコンストラクター
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.connection != null)
            {
                try
                {
                    connection.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

    }
}
