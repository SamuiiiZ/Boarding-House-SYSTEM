using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using BoardingHouseSys.Models;
using BoardingHouseSys.Services;

namespace BoardingHouseSys.Data
{
    public class UserRepository
    {
        private DatabaseHelper _dbHelper;

        public UserRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public User? Authenticate(string username, string password)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Username", username)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string? storedHash = row["PasswordHash"].ToString();

                // Check if password matches (Hashed)
                if (!string.IsNullOrEmpty(storedHash) && SecurityHelper.VerifyPassword(password, storedHash))
                {
                     return MapToUser(row);
                }
            }
            return null;
        }

        public int CreateUser(User user, string plainPassword)
        {
            // 1. Hash the password
            string passwordHash = SecurityHelper.HashPassword(plainPassword);

            // 2. Insert
            string query = "INSERT INTO Users (Username, PasswordHash, Role, IsActive) VALUES (@Username, @PasswordHash, @Role, 1); SELECT LAST_INSERT_ID();";
            
            MySqlParameter[] parameters = {
                new MySqlParameter("@Username", user.Username),
                new MySqlParameter("@PasswordHash", passwordHash),
                new MySqlParameter("@Role", user.Role)
            };

            // ExecuteScalar returns the ID of the new user
            return Convert.ToInt32(_dbHelper.ExecuteScalar(query, parameters));
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            string query = "SELECT * FROM Users WHERE IsActive = 1 ORDER BY Role, Username";
            
            DataTable dt = _dbHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                users.Add(MapToUser(row));
            }
            return users;
        }

        public void UpdateUser(User user, string? newPassword = null)
        {
            string query = "UPDATE Users SET Username = @Username, Role = @Role WHERE Id = @Id";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Id", user.Id),
                new MySqlParameter("@Username", user.Username),
                new MySqlParameter("@Role", user.Role)
            };

            if (!string.IsNullOrEmpty(newPassword))
            {
                query = "UPDATE Users SET Username = @Username, Role = @Role, PasswordHash = @PasswordHash WHERE Id = @Id";
                parameters.Add(new MySqlParameter("@PasswordHash", SecurityHelper.HashPassword(newPassword)));
            }

            _dbHelper.ExecuteNonQuery(query, parameters.ToArray());
        }

        public void DeleteUser(int id)
        {
            string query = "UPDATE Users SET IsActive = 0 WHERE Id = @Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", id)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        private User MapToUser(DataRow row)
        {
            return new User
            {
                Id = Convert.ToInt32(row["Id"]),
                Username = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString(),
                Role = row["Role"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"])
            };
        }
    }
}
