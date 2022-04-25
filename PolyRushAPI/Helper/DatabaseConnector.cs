using MySql.Data.MySqlClient;

namespace Helper
{
    public static class DatabaseConnector
    {
        //Verbinding openen en doorgeven aan DA
        public static MySqlConnection MakeConnection()
        {
            //Opstellen connectiestring
            MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder();
            connStringBuilder.Server = "localhost";
            connStringBuilder.Port = 3306;
            connStringBuilder.UserID = "root";
            connStringBuilder.Password = "";
            connStringBuilder.Database = "polyrush";

            //Connection-object aanmaken voor de verbinding, geven we uiteindelijk door aan DA.
            MySqlConnection conn = new MySqlConnection(connStringBuilder.ToString());
            conn.Open();
            return conn;
        }

        //Verbinding sluiten
        public static void CloseConnection(MySqlConnection conn)
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}