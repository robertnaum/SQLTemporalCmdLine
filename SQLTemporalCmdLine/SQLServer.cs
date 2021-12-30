using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTemporalCmdLine
{
    class SQLServer
    {
        private string _connectionString;
        private IConfiguration _config;
        private bool verbose;
        public SQLServer(IConfiguration config, string connectionString)
        {
            _connectionString = connectionString;
            _config = config;
            verbose = Convert.ToBoolean(config.GetSection("verbose").Value);
        }
        public void AddColumn(string TableName, string ColumnName, string ColumnType, bool nullable, string @default)
        {
//            StringBuilder sb = new StringBuilder();
            using (var con = new SqlConnection(_connectionString))
            {

            }

            if (verbose)
            {
                Console.Write($"added column name [{ColumnName}] to table [{TableName}]");
                Console.Write($" of type {ColumnType} ");
                Console.Write($"{(nullable ? "NULL" : "NOT NULL")}");
                if (!string.IsNullOrEmpty(@default))
                    Console.Write ($" DEFAULT \"{@default}\"");
                Console.WriteLine();
            }

        }

        public void DeleteColumn(string TableName, string ColumnName)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                string sql = $"ALTER TABLE {TableName} DROP COLUMN {ColumnName}";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
