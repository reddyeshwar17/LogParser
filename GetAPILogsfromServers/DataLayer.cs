using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GetAPILogsfromServers
{
    internal class DataLayer
    {
        public void Insert(ref string machineName,ref string subscriber1)
        {
            try
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["SQLInfoDBConnectionString"].ToString();

                using (SqlConnection sqlconnection = new SqlConnection(connectionstring))
                {
                    SqlCommand command = new SqlCommand("InsertIntoSubScriber", sqlconnection);

                    SqlParameter ServerName = command.Parameters.AddWithValue("@Server", machineName);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter TNumber = command.Parameters.AddWithValue("@subscriber", subscriber1);
                    command.CommandType = CommandType.StoredProcedure;

                    sqlconnection.Open();
                    command.ExecuteNonQuery();
                    sqlconnection.Close();

                    subscriber1 = null;
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }
    }
}