-- Create Database
CREATE DATABASE IF NOT EXISTS BoardingHouseDB;
USE BoardingHouseDB;

-- 1. Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL, -- 'SuperAdmin', 'Admin', 'Boarder'
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT NOW()
);

-- 2. Rooms Table
CREATE TABLE IF NOT EXISTS Rooms (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    RoomNumber VARCHAR(20) NOT NULL UNIQUE,
    Capacity INT NOT NULL,
    MonthlyRate DECIMAL(10, 2) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT NOW()
);

-- 2.1 Boarding Houses Table
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
);

-- 3. Boarders Table
CREATE TABLE IF NOT EXISTS Boarders (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NULL, -- Optional link to Users table for login
    FullName VARCHAR(100) NOT NULL,
    Address VARCHAR(255),
    Phone VARCHAR(20),
    RoomId INT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT NOW(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RoomId) REFERENCES Rooms(Id)
);

-- 4. Payments Table
CREATE TABLE IF NOT EXISTS Payments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    BoarderId INT NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentDate DATETIME DEFAULT NOW(),
    MonthPaid VARCHAR(20) NOT NULL, -- e.g., 'January'
    YearPaid INT NOT NULL,
    Status VARCHAR(20) DEFAULT 'Pending', -- 'Paid', 'Pending', 'Overdue'
    Notes VARCHAR(255),
    FOREIGN KEY (BoarderId) REFERENCES Boarders(Id)
);

-- Seed Data

-- Default Super Admin (Password: admin123)
-- Hash generated using SHA256 for 'admin123'
INSERT INTO Users (Username, PasswordHash, Role) VALUES ('superadmin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'SuperAdmin');
INSERT INTO Users (Username, PasswordHash, Role) VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin');

-- Sample Rooms
INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate) VALUES ('101', 2, 5000.00);
INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate) VALUES ('102', 4, 3500.00);
INSERT INTO Rooms (RoomNumber, Capacity, MonthlyRate) VALUES ('201', 1, 8000.00);
