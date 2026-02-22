using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using BoardingHouseSys.Models;

namespace BoardingHouseSys.Data
{
    public class BoardingHouseRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=BoardingHouseDB;Uid=root;Pwd=root;";

        public List<BoardingHouse> GetAllByOwner(int ownerId)
        {
            var list = new List<BoardingHouse>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM BoardingHouses WHERE OwnerId = @OwnerId AND IsActive = 1 ORDER BY CreatedAt DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerId", ownerId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapToModel(reader));
                        }
                    }
                }
            }
            return list;
        }

        public List<BoardingHouse> GetAll()
        {
            var list = new List<BoardingHouse>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT bh.*, u.Username as OwnerName 
                    FROM BoardingHouses bh
                    JOIN Users u ON bh.OwnerId = u.Id
                    WHERE bh.IsActive = 1 
                    ORDER BY bh.CreatedAt DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bh = MapToModel(reader);
                            if (HasColumn(reader, "OwnerName"))
                            {
                                bh.OwnerName = reader["OwnerName"].ToString() ?? "Unknown";
                            }
                            list.Add(bh);
                        }
                    }
                }
            }
            return list;
        }

        public List<BoardingHouse> Search(string keyword)
        {
            var list = new List<BoardingHouse>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT bh.*, u.Username as OwnerName 
                    FROM BoardingHouses bh
                    JOIN Users u ON bh.OwnerId = u.Id
                    WHERE bh.IsActive = 1 
                    AND (bh.Name LIKE @Keyword OR bh.Address LIKE @Keyword OR bh.Description LIKE @Keyword)
                    ORDER BY bh.CreatedAt DESC";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bh = MapToModel(reader);
                            if (HasColumn(reader, "OwnerName"))
                            {
                                bh.OwnerName = reader["OwnerName"].ToString() ?? "Unknown";
                            }
                            list.Add(bh);
                        }
                    }
                }
            }
            return list;
        }

        public List<BoardingHouse> SearchByOwner(int ownerId, string keyword)
        {
            var list = new List<BoardingHouse>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT * 
                    FROM BoardingHouses 
                    WHERE OwnerId = @OwnerId AND IsActive = 1
                    AND (Name LIKE @Keyword OR Address LIKE @Keyword OR Description LIKE @Keyword OR Rules LIKE @Keyword OR Amenities LIKE @Keyword)
                    ORDER BY CreatedAt DESC";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerId", ownerId);
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapToModel(reader));
                        }
                    }
                }
            }
            return list;
        }

        public BoardingHouse? GetById(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM BoardingHouses WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToModel(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void Add(BoardingHouse bh)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    INSERT INTO BoardingHouses (OwnerId, Name, Address, Description, Rules, Amenities, ImagePath1, ImagePath2, ImagePath3, IsActive, CreatedAt)
                    VALUES (@OwnerId, @Name, @Address, @Description, @Rules, @Amenities, @ImagePath1, @ImagePath2, @ImagePath3, 1, NOW())";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerId", bh.OwnerId);
                    cmd.Parameters.AddWithValue("@Name", bh.Name);
                    cmd.Parameters.AddWithValue("@Address", bh.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", bh.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Rules", bh.Rules ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amenities", bh.Amenities ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath1", bh.ImagePath1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath2", bh.ImagePath2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath3", bh.ImagePath3 ?? (object)DBNull.Value);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(BoardingHouse bh)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    UPDATE BoardingHouses 
                    SET Name = @Name, 
                        Address = @Address, 
                        Description = @Description,
                        Rules = @Rules,
                        Amenities = @Amenities,
                        ImagePath1 = @ImagePath1,
                        ImagePath2 = @ImagePath2,
                        ImagePath3 = @ImagePath3
                    WHERE Id = @Id";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", bh.Id);
                    cmd.Parameters.AddWithValue("@Name", bh.Name);
                    cmd.Parameters.AddWithValue("@Address", bh.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", bh.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Rules", bh.Rules ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amenities", bh.Amenities ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath1", bh.ImagePath1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath2", bh.ImagePath2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath3", bh.ImagePath3 ?? (object)DBNull.Value);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateImages(int id, string? imagePath1, string? imagePath2, string? imagePath3)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    UPDATE BoardingHouses 
                    SET ImagePath1 = @ImagePath1,
                        ImagePath2 = @ImagePath2,
                        ImagePath3 = @ImagePath3
                    WHERE Id = @Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@ImagePath1", imagePath1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath2", imagePath2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath3", imagePath3 ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                // Soft delete
                string query = "UPDATE BoardingHouses SET IsActive = 0 WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private BoardingHouse MapToModel(MySqlDataReader reader)
        {
            var bh = new BoardingHouse
            {
                Id = Convert.ToInt32(reader["Id"]),
                OwnerId = Convert.ToInt32(reader["OwnerId"]),
                Name = reader["Name"].ToString() ?? string.Empty,
                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() ?? string.Empty : string.Empty,
                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : null,
                Rules = reader["Rules"] != DBNull.Value ? reader["Rules"].ToString() : null,
                Amenities = reader["Amenities"] != DBNull.Value ? reader["Amenities"].ToString() : null,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
            if (HasColumn(reader, "ImagePath1"))
            {
                bh.ImagePath1 = reader["ImagePath1"] != DBNull.Value ? reader["ImagePath1"].ToString() : null;
            }
            if (HasColumn(reader, "ImagePath2"))
            {
                bh.ImagePath2 = reader["ImagePath2"] != DBNull.Value ? reader["ImagePath2"].ToString() : null;
            }
            if (HasColumn(reader, "ImagePath3"))
            {
                bh.ImagePath3 = reader["ImagePath3"] != DBNull.Value ? reader["ImagePath3"].ToString() : null;
            }
            return bh;
        }

        private bool HasColumn(MySqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
