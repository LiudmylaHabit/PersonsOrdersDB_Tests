using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Tests
{
    class DataBaseMethods
    {
        string _connectionString;

        public void ConnectToCatalog(string catalog)
        {
            _connectionString = @"Data Source=.\SQLEXPRESS;"+ $"Initial Catalog={catalog};Integrated Security=True";
        }

        public DataTable Execute(string sqlRequest)
        {
            var dataSet = new DataSet();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var sqlCommand = new SqlCommand(sqlRequest, connection))
                {
                    sqlCommand.CommandText = sqlRequest;
                    var adapter = new SqlDataAdapter(sqlCommand);
                    adapter.Fill(dataSet);
                }
            }
            if (sqlRequest.StartsWith("SELECT") && dataSet.Tables[0].Rows[0] != null)
                return dataSet.Tables[0];
            DataTable emptyTable = new DataTable();
            return emptyTable;           
        }  

        public List<int> GetAllUsersID(DataTable users)
        {
            List<int> usersId = new List<int>();
            foreach (DataRow row in users.Rows)
            {
                usersId.Add(Convert.ToInt32(row[0].ToString()));
            }
            return usersId;
        }
    }
}
