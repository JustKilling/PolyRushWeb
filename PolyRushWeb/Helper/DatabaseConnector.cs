using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace PolyRushWeb.Helper
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

            //if debugging, use the local database, otherwise use the clous database.
            #if DEBUG
                connStringBuilder.Server = "polyrushmysql.mysql.database.azure.com";
                connStringBuilder.Port = 3306;
                connStringBuilder.UserID = "emield";
                connStringBuilder.Database = "polyrush";
                string text = "PolyRush123";
                //var fileStream = new FileStream(@"password.txt", FileMode.Open, FileAccess.Read);
                //using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                //{
                //    text = streamReader.ReadToEnd();
                //}
                connStringBuilder.Password = text;

            #else
                connStringBuilder.Server = "localhost";
                connStringBuilder.Port = 3307;
                connStringBuilder.UserID = "root";
                connStringBuilder.Password = "usbw";
                connStringBuilder.Database = "polyrush";
            #endif
                

          
            
            
            MySqlConnection conn = new(connStringBuilder.ConnectionString);
           
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