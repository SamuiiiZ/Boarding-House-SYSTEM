using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BoardingHouseSys.Data
{
    public class DatabaseHelper
    {
        // Default MySQL connection string
        private static string _globalConnectionString = "Server=localhost;Database=BoardingHouseDB;Uid=root;Pwd=root;"; 
        private string _connectionString;

        public static void SetConnectionString(string connString)
        {
            _globalConnectionString = connString;
        }

        public DatabaseHelper()
        {
            _connectionString = _globalConnectionString;
        }

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string query, MySqlParameter[]? parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    
                    DataTable dt = new DataTable();
                    try
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database Error: " + ex.Message);
                    }
                    return dt;
                }
            }
        }

        public int ExecuteNonQuery(string query, MySqlParameter[]? parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    
                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database Error: " + ex.Message);
                    }
                }
            }
        }

        public object? ExecuteScalar(string query, MySqlParameter[]? parameters = null)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    
                    try
                    {
                        conn.Open();
                        return cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database Error: " + ex.Message);
                    }
                }
            }
        }
    }
}
