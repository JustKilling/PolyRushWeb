using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using PolyRushLibrary;

namespace PolyRushAPI.Helper
{
    public class DatabaseConnector
    {
        public DatabaseConnector()
        {
            MakeConnection();
        }

        //Verbinding openen en doorgeven aan DA
        public static MySqlConnection MakeConnection()
        {
            //Opstellen connectiestring
            MySqlConnectionStringBuilder connStringBuilder = new();
            var type = ApiType.Local;
            switch (type)
            {
                case ApiType.Cloud:{}
                    connStringBuilder.Server = "polyrushmysql.mysql.database.azure.com";
                    connStringBuilder.Port = 3306;
                    connStringBuilder.UserID = "emield";
                    connStringBuilder.Database = "polyrush";
                    string text;
                    var fileStream = new FileStream(@"password.txt", FileMode.Open, FileAccess.Read);
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        text = streamReader.ReadToEnd();
                    }
                    connStringBuilder.Password = text;
                    break;
                case ApiType.Local:
                    connStringBuilder.Server = "localhost";
                    connStringBuilder.Port = 3307;
                    connStringBuilder.UserID = "root";
                    connStringBuilder.Password = "usbw";
                    connStringBuilder.Database = "polyrush";
                    break;
            }
            
            var conn = new MySqlConnection(connStringBuilder.ConnectionString);
           
            conn.Open();
            return conn;
        }
        
        //Verbinding sluiten
        public static void CloseConnection(MySqlConnection conn)
        {
            if (conn.State == ConnectionState.Open) conn.Close();
        }
        
    }
}