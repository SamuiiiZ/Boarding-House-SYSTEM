using System;
using MySql.Data.MySqlClient;

namespace BoardingHouseSys.Data
{
    public static class DatabaseBootstrap
    {
        private static string _serverConnection = "Server=localhost;Uid=root;Pwd=root;";
        private static string _dbConnection = "Server=localhost;Database=BoardingHouseDB;Uid=root;Pwd=root;";

        public static void Initialize()
        {
            try
            {
                // 1. Create Database if it doesn't exist
                using (var conn = new MySqlConnection(_serverConnection))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS BoardingHouseDB;", conn);
                    cmd.ExecuteNonQuery();
                }

                // 2. Create Tables
                using (var conn = new MySqlConnection(_dbConnection))
                {
                    conn.Open();
                    
                    // Users Table
                    string sqlUsers = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            Username VARCHAR(50) NOT NULL UNIQUE,
                            PasswordHash VARCHAR(255) NOT NULL,
                            Role VARCHAR(20) NOT NULL,
                            IsActive BIT DEFAULT 1,
                            CreatedAt DATETIME DEFAULT NOW()
                        );";
                    new MySqlCommand(sqlUsers, conn).ExecuteNonQuery();

                    // Rooms Table
                    string sqlRooms = @"
                        CREATE TABLE IF NOT EXISTS Rooms (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            RoomNumber VARCHAR(20) NOT NULL UNIQUE,
                            Capacity INT NOT NULL,
                            MonthlyRate DECIMAL(10, 2) NOT NULL,
                            IsActive BIT DEFAULT 1,
                            CreatedAt DATETIME DEFAULT NOW()
                        );";
                    new MySqlCommand(sqlRooms, conn).ExecuteNonQuery();

                    // Boarders Table
                    string sqlBoarders = @"
                        CREATE TABLE IF NOT EXISTS Boarders (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            UserId INT NULL,
                            FullName VARCHAR(100) NOT NULL,
                            Address VARCHAR(255),
                            Phone VARCHAR(20),
                            RoomId INT NULL,
                            IsActive BIT DEFAULT 1,
                            CreatedAt DATETIME DEFAULT NOW(),
                            FOREIGN KEY (UserId) REFERENCES Users(Id),
                            FOREIGN KEY (RoomId) REFERENCES Rooms(Id)
                        );";
                    new MySqlCommand(sqlBoarders, conn).ExecuteNonQuery();

                    // Payments Table
                    string sqlPayments = @"
                        CREATE TABLE IF NOT EXISTS Payments (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            BoarderId INT NOT NULL,
                            Amount DECIMAL(10, 2) NOT NULL,
                            PaymentDate DATETIME DEFAULT NOW(),
                            MonthPaid VARCHAR(20) NOT NULL,
                            YearPaid INT NOT NULL,
                            Status VARCHAR(20) DEFAULT 'Pending',
                            Notes VARCHAR(255),
                            FOREIGN KEY (BoarderId) REFERENCES Boarders(Id)
                        );";
                    new MySqlCommand(sqlPayments, conn).ExecuteNonQuery();

                    // ==========================================
                    // NEW: Multi-Property Features
                    // ==========================================

                    // 1. Create BoardingHouses Table
                    string sqlBH = @"
                        CREATE TABLE IF NOT EXISTS BoardingHouses (
                            Id INT PRIMARY KEY AUTO_INCREMENT,
                            OwnerId INT NOT NULL,
                            Name VARCHAR(100) NOT NULL,
                            Address VARCHAR(255),
                            Description TEXT,
                            Rules TEXT,
                            Amenities TEXT,
                            ImagePath1 VARCHAR(500) NULL,
                            ImagePath2 VARCHAR(500) NULL,
                            ImagePath3 VARCHAR(500) NULL,
                            IsActive BIT DEFAULT 1,
                            CreatedAt DATETIME DEFAULT NOW(),
                            FOREIGN KEY (OwnerId) REFERENCES Users(Id)
                        );";
                    new MySqlCommand(sqlBH, conn).ExecuteNonQuery();

                    try {
                        string alterRooms = "ALTER TABLE Rooms ADD COLUMN BoardingHouseId INT NULL;";
                        new MySqlCommand(alterRooms, conn).ExecuteNonQuery();
                        string fkRooms = "ALTER TABLE Rooms ADD FOREIGN KEY (BoardingHouseId) REFERENCES BoardingHouses(Id);";
                        new MySqlCommand(fkRooms, conn).ExecuteNonQuery();
                    } catch (Exception) { }

                    try {
                        string dropUniqueRoomNumber = "ALTER TABLE Rooms DROP INDEX RoomNumber;";
                        new MySqlCommand(dropUniqueRoomNumber, conn).ExecuteNonQuery();
                    } catch (Exception) { }

                    try {
                        string addCompositeUnique = "ALTER TABLE Rooms ADD UNIQUE KEY UX_Rooms_BH_Room (BoardingHouseId, RoomNumber);";
                        new MySqlCommand(addCompositeUnique, conn).ExecuteNonQuery();
                    } catch (Exception) { }

                    // 3. Add BoardingHouseId to Boarders
                    try {
                        string alterBoarders = "ALTER TABLE Boarders ADD COLUMN BoardingHouseId INT NULL;";
                        new MySqlCommand(alterBoarders, conn).ExecuteNonQuery();
                        string fkBoarders = "ALTER TABLE Boarders ADD FOREIGN KEY (BoardingHouseId) REFERENCES BoardingHouses(Id);";
                        new MySqlCommand(fkBoarders, conn).ExecuteNonQuery();
                    } catch (Exception) { /* Column likely exists */ }

                    // 6. Add ProfilePicturePath to Boarders
                    try {
                        string alterBoardersPic = "ALTER TABLE Boarders ADD COLUMN ProfilePicturePath VARCHAR(500) NULL;";
                        new MySqlCommand(alterBoardersPic, conn).ExecuteNonQuery();
                    } catch (Exception) { /* Column likely exists */ }

                    try {
                        string alterBhPic1 = "ALTER TABLE BoardingHouses ADD COLUMN ImagePath1 VARCHAR(500) NULL;";
                        new MySqlCommand(alterBhPic1, conn).ExecuteNonQuery();
                    } catch (Exception) { /* Column likely exists */ }
                    try {
                        string alterBhPic2 = "ALTER TABLE BoardingHouses ADD COLUMN ImagePath2 VARCHAR(500) NULL;";
                        new MySqlCommand(alterBhPic2, conn).ExecuteNonQuery();
                    } catch (Exception) { /* Column likely exists */ }
                    try {
                        string alterBhPic3 = "ALTER TABLE BoardingHouses ADD COLUMN ImagePath3 VARCHAR(500) NULL;";
                        new MySqlCommand(alterBhPic3, conn).ExecuteNonQuery();
                    } catch (Exception) { /* Column likely exists */ }

                    // 5. Seed Default Boarding House (Migration for existing data)
                    // If we have rooms but no BoardingHouses, create a default one and link them.
                    var checkBH = new MySqlCommand("SELECT COUNT(*) FROM BoardingHouses", conn);
                    long bhCount = (long)checkBH.ExecuteScalar();

                    if (bhCount == 0)
                    {
                        // Get the first Admin ID
                        var cmdGetAdmin = new MySqlCommand("SELECT Id FROM Users WHERE Role IN ('Admin', 'SuperAdmin') LIMIT 1", conn);
                        object? adminIdObj = cmdGetAdmin.ExecuteScalar();
                        
                        if (adminIdObj != null)
                        {
                            int adminId = Convert.ToInt32(adminIdObj);
                            
                            // Insert Default House
                            string seedBH = @"
                                INSERT INTO BoardingHouses (OwnerId, Name, Address, Description) 
                                VALUES (@OwnerId, 'My Main Boarding House', 'Default Address', 'Main Property');
                                SELECT LAST_INSERT_ID();";
                            
                            var cmdSeed = new MySqlCommand(seedBH, conn);
                            cmdSeed.Parameters.AddWithValue("@OwnerId", adminId);
                            int newBhId = Convert.ToInt32(cmdSeed.ExecuteScalar());

                            // Update existing records to belong to this house
                            new MySqlCommand($"UPDATE Rooms SET BoardingHouseId = {newBhId} WHERE BoardingHouseId IS NULL", conn).ExecuteNonQuery();
                            new MySqlCommand($"UPDATE Boarders SET BoardingHouseId = {newBhId} WHERE BoardingHouseId IS NULL", conn).ExecuteNonQuery();
                        }
                    }

                    // 3. Seed Data (Only if empty)
                    var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM Users", conn);
                    long userCount = (long)checkCmd.ExecuteScalar();

                    if (userCount == 0)
                    {
                        string seedUsers = @"
                            INSERT INTO Users (Username, PasswordHash, Role) VALUES ('superadmin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'SuperAdmin');
                            INSERT INTO Users (Username, PasswordHash, Role) VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin');";
                        new MySqlCommand(seedUsers, conn).ExecuteNonQuery();

                        string seedRooms = @"
                            INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate) VALUES ('101', 2, 5000.00);
                            INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate) VALUES ('102', 4, 3500.00);
                            INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate) VALUES ('201', 1, 8000.00);";
                        new MySqlCommand(seedRooms, conn).ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Re-throw to be caught in Program.cs and shown to user
                throw new Exception("Database Initialization Failed: " + ex.Message);
            }
        }
    }
}
