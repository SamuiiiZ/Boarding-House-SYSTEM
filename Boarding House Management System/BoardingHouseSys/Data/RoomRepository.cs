using System;
using System.Data;
using MySql.Data.MySqlClient;
using BoardingHouseSys.Models;

namespace BoardingHouseSys.Data
{
    public class RoomRepository
    {
        private DatabaseHelper _dbHelper;

        public RoomRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public DataTable GetAllRooms()
        {
            string query = "SELECT Id, RoomNumber, Capacity, MonthlyRate, BoardingHouseId FROM Rooms WHERE IsActive = 1";
            return _dbHelper.ExecuteQuery(query);
        }

        public DataTable GetRoomsByBoardingHouse(int boardingHouseId)
        {
            string query = "SELECT Id, RoomNumber, Capacity, MonthlyRate, BoardingHouseId FROM Rooms WHERE IsActive = 1 AND BoardingHouseId = @BoardingHouseId";
            MySqlParameter[] parameters = {
                new MySqlParameter("@BoardingHouseId", boardingHouseId)
            };
            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public DataTable SearchRooms(string keyword)
        {
            string query = @"
                SELECT Id, RoomNumber, Capacity, MonthlyRate, BoardingHouseId 
                FROM Rooms 
                WHERE IsActive = 1 
                AND (RoomNumber LIKE @Keyword OR CAST(Capacity AS CHAR) LIKE @Keyword OR CAST(MonthlyRate AS CHAR) LIKE @Keyword)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@Keyword", "%" + keyword + "%")
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public DataTable SearchRoomsByBoardingHouse(int boardingHouseId, string keyword)
        {
            string query = @"
                SELECT Id, RoomNumber, Capacity, MonthlyRate, BoardingHouseId 
                FROM Rooms 
                WHERE IsActive = 1 
                AND BoardingHouseId = @BoardingHouseId
                AND (RoomNumber LIKE @Keyword OR CAST(Capacity AS CHAR) LIKE @Keyword OR CAST(MonthlyRate AS CHAR) LIKE @Keyword)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@BoardingHouseId", boardingHouseId),
                new MySqlParameter("@Keyword", "%" + keyword + "%")
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public void AddRoom(Room room)
        {
            string query = "INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate, BoardingHouseId, IsActive) VALUES (@RoomNumber, @Capacity, @MonthlyRate, @BoardingHouseId, 1)";
            MySqlParameter[] parameters = {
                new MySqlParameter("@RoomNumber", room.RoomNumber),
                new MySqlParameter("@Capacity", room.Capacity),
                new MySqlParameter("@MonthlyRate", room.MonthlyRate),
                new MySqlParameter("@BoardingHouseId", room.BoardingHouseId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }
        
        public void UpdateRoom(Room room)
        {
            string query = "UPDATE Rooms SET RoomNumber = @RoomNumber, Capacity = @Capacity, MonthlyRate = @MonthlyRate, BoardingHouseId = @BoardingHouseId WHERE Id = @Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", room.Id),
                new MySqlParameter("@RoomNumber", room.RoomNumber),
                new MySqlParameter("@Capacity", room.Capacity),
                new MySqlParameter("@MonthlyRate", room.MonthlyRate),
                new MySqlParameter("@BoardingHouseId", room.BoardingHouseId)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void DeleteRoom(int id)
        {
            string query = "UPDATE Rooms SET IsActive = 0 WHERE Id = @Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", id)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
