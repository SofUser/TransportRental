using System.Configuration;
using System.Data.SqlClient;

namespace TransportRental
{
    class ConnectDB
    {
        private SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBaseTransportRental"].ConnectionString);

        public void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();
        }

        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();
        }

        public SqlConnection GetConnetcion() => sqlConnection;
    }
}
