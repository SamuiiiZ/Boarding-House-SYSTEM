using System;
using System.Data;
using MySql.Data.MySqlClient;
using BoardingHouseSys.Models;

namespace BoardingHouseSys.Data
{
    public class BoarderRepository
    {
        private DatabaseHelper _dbHelper;

        public BoarderRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public DataTable GetAllBoarders()
        {
            string query = @"
                SELECT 
                    b.Id, 
                    b.FullName, 
                    b.Address, 
                    b.Phone, 
                    b.RoomId,
                    b.BoardingHouseId,
                    b.UserId,
                    b.ProfilePicturePath,
                    r.RoomNumber, 
                    bh.Name AS BoardingHouseName,
                    u.Username 
                FROM Boarders b
                LEFT JOIN Rooms r ON b.RoomId = r.Id
                LEFT JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                LEFT JOIN Users u ON b.UserId = u.Id
                WHERE b.IsActive = 1";
            return _dbHelper.ExecuteQuery(query);
        }

        public void UpdateProfilePicture(int boarderId, string relativePath)
        {
            string query = "UPDATE Boarders SET ProfilePicturePath = @Path WHERE Id = @Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Path", relativePath),
                new MySqlParameter("@Id", boarderId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void AddBoarder(Boarder boarder)
        {
            string query = @"
                INSERT INTO Boarders (UserId, FullName, Address, Phone, RoomId, BoardingHouseId, ProfilePicturePath, IsActive) 
                VALUES (@UserId, @FullName, @Address, @Phone, @RoomId, @BoardingHouseId, @ProfilePicturePath, 1)";
            
            MySqlParameter[] parameters = {
                new MySqlParameter("@UserId", boarder.UserId ?? (object)DBNull.Value),
                new MySqlParameter("@FullName", boarder.FullName),
                new MySqlParameter("@Address", boarder.Address),
                new MySqlParameter("@Phone", boarder.Phone),
                new MySqlParameter("@RoomId", boarder.RoomId ?? (object)DBNull.Value),
                new MySqlParameter("@BoardingHouseId", boarder.BoardingHouseId ?? (object)DBNull.Value),
                new MySqlParameter("@ProfilePicturePath", boarder.ProfilePicturePath ?? (object)DBNull.Value)
            };
            
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void UpdateBoarder(Boarder boarder)
        {
            string query = @"
                UPDATE Boarders 
                SET FullName = @FullName, Address = @Address, Phone = @Phone, RoomId = @RoomId, BoardingHouseId = @BoardingHouseId, ProfilePicturePath = @ProfilePicturePath
                WHERE Id = @Id";
            
            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", boarder.Id),
                new MySqlParameter("@FullName", boarder.FullName),
                new MySqlParameter("@Address", boarder.Address),
                new MySqlParameter("@Phone", boarder.Phone),
                new MySqlParameter("@RoomId", boarder.RoomId ?? (object)DBNull.Value),
                new MySqlParameter("@BoardingHouseId", boarder.BoardingHouseId ?? (object)DBNull.Value),
                new MySqlParameter("@ProfilePicturePath", boarder.ProfilePicturePath ?? (object)DBNull.Value)
            };
            
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void DeleteBoarder(int id)
        {
            string query = "UPDATE Boarders SET IsActive = 0 WHERE Id = @Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", id)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public Boarder? GetBoarderById(int id)
        {
            string query = @"
                SELECT b.*, r.RoomNumber, r.MonthlyRate, bh.Name AS BoardingHouseName 
                FROM Boarders b 
                LEFT JOIN Rooms r ON b.RoomId = r.Id 
                LEFT JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                WHERE b.Id = @Id AND b.IsActive = 1";

            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", id)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return MapRowToModel(row);
            }
            return null;
        }

        private Boarder MapRowToModel(DataRow row)
        {
            return new Boarder
            {
                Id = Convert.ToInt32(row["Id"]),
                UserId = row["UserId"] != DBNull.Value ? Convert.ToInt32(row["UserId"]) : null,
                FullName = row["FullName"].ToString()!,
                Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : null,
                RoomId = row["RoomId"] != DBNull.Value ? Convert.ToInt32(row["RoomId"]) : null,
                BoardingHouseId = row["BoardingHouseId"] != DBNull.Value ? Convert.ToInt32(row["BoardingHouseId"]) : null,
                ProfilePicturePath = row["ProfilePicturePath"] != DBNull.Value ? row["ProfilePicturePath"].ToString() : null,
                RoomNumber = row["RoomNumber"] != DBNull.Value ? row["RoomNumber"].ToString() : null,
                BoardingHouseName = row["BoardingHouseName"] != DBNull.Value ? row["BoardingHouseName"].ToString() : null
            };
        }

        public Boarder? GetBoarderByUserId(int userId)
        {
            string query = @"
                SELECT b.*, r.RoomNumber, r.MonthlyRate, bh.Name AS BoardingHouseName 
                FROM Boarders b 
                LEFT JOIN Rooms r ON b.RoomId = r.Id 
                LEFT JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                WHERE b.UserId = @UserId AND b.IsActive = 1";

            MySqlParameter[] parameters = {
                new MySqlParameter("@UserId", userId)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                return MapRowToModel(dt.Rows[0]);
            }
            return null;
        }

        public DataTable GetBoarderDetailsByUserId(int userId)
        {
             string query = @"
                SELECT 
                    b.Id, 
                    b.FullName, 
                    b.Address, 
                    b.Phone, 
                    b.ProfilePicturePath,
                    r.RoomNumber, 
                    bh.Name AS BoardingHouseName,
                    r.MonthlyRate
                FROM Boarders b
                LEFT JOIN Rooms r ON b.RoomId = r.Id
                LEFT JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                WHERE b.UserId = @UserId AND b.IsActive = 1";

            MySqlParameter[] parameters = {
                new MySqlParameter("@UserId", userId)
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public DataTable SearchBoarders(string keyword)
        {
            string query = @"
                SELECT 
                    b.Id, 
                    b.FullName, 
                    b.Address, 
                    b.Phone, 
                    b.RoomId,
                    b.BoardingHouseId,
                    b.UserId,
                    b.ProfilePicturePath,
                    r.RoomNumber, 
                    bh.Name AS BoardingHouseName,
                    u.Username 
                FROM Boarders b
                LEFT JOIN Rooms r ON b.RoomId = r.Id
                LEFT JOIN BoardingHouses bh ON b.BoardingHouseId = bh.Id
                LEFT JOIN Users u ON b.UserId = u.Id
                WHERE b.IsActive = 1 
                AND (b.FullName LIKE @Keyword OR u.Username LIKE @Keyword OR r.RoomNumber LIKE @Keyword OR bh.Name LIKE @Keyword)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@Keyword", "%" + keyword + "%")
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }
    }
}
