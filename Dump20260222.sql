CREATE DATABASE  IF NOT EXISTS `boardinghousedb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `boardinghousedb`;
-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: localhost    Database: boardinghousedb
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `boarders`
--

DROP TABLE IF EXISTS `boarders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boarders` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int DEFAULT NULL,
  `FullName` varchar(100) NOT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `RoomId` int DEFAULT NULL,
  `IsActive` bit(1) DEFAULT b'1',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `BoardingHouseId` int DEFAULT NULL,
  `ProfilePicturePath` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `UserId` (`UserId`),
  KEY `RoomId` (`RoomId`),
  KEY `BoardingHouseId` (`BoardingHouseId`),
  CONSTRAINT `boarders_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`),
  CONSTRAINT `boarders_ibfk_2` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`),
  CONSTRAINT `boarders_ibfk_3` FOREIGN KEY (`BoardingHouseId`) REFERENCES `boardinghouses` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `boarders`
--

LOCK TABLES `boarders` WRITE;
/*!40000 ALTER TABLE `boarders` DISABLE KEYS */;
INSERT INTO `boarders` VALUES (1,4,'Keith Fuentes','Magsaysay Barayong','123456789101',6,_binary '','2026-02-16 12:03:16',2,'ProfilePictures/9091c970-b62a-4b30-ab99-b05b18808fd0.jpg'),(2,5,'rodz','Malalag','123456789101',7,_binary '','2026-02-18 11:17:34',2,'ProfilePictures/380fb5db-2665-42b8-8398-ee876afb8306.jpg');
/*!40000 ALTER TABLE `boarders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `boardinghouses`
--

DROP TABLE IF EXISTS `boardinghouses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boardinghouses` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OwnerId` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `Description` text,
  `Rules` text,
  `Amenities` text,
  `IsActive` bit(1) DEFAULT b'1',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `ImagePath1` varchar(500) DEFAULT NULL,
  `ImagePath2` varchar(500) DEFAULT NULL,
  `ImagePath3` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `OwnerId` (`OwnerId`),
  CONSTRAINT `boardinghouses_ibfk_1` FOREIGN KEY (`OwnerId`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `boardinghouses`
--

LOCK TABLES `boardinghouses` WRITE;
/*!40000 ALTER TABLE `boardinghouses` DISABLE KEYS */;
INSERT INTO `boardinghouses` VALUES (1,1,'My Main Boarding House','Default Address','Main Property',NULL,NULL,_binary '\0','2026-02-16 11:45:20',NULL,NULL,NULL),(2,3,'Alisoso BH','HBV TABLIZO ST, MATI DIGOS CITY','Boarding House at affordable price','9 pm curfew, no pets allowed(bois)','Fast internet, cozy, friendly environment',_binary '','2026-02-16 11:47:16','BoardingHousePictures/01b5c124-7371-4d7d-82cc-96fc93922331.jpg','BoardingHousePictures/277c2857-74a6-413d-9308-b99136b9861e.jpg','BoardingHousePictures/6474cb4b-d7e3-476c-b9d5-d4676e11a3e5.jpg'),(3,3,'Alisoso BH','HBV TABLIZO ST, MATI DIGOS CITY','Boarding House at affordable price','9 pm curfew, no pets allowed(bois)','Fast internet, cozy, friendly environment',_binary '\0','2026-02-22 17:54:27',NULL,NULL,NULL),(4,3,'Alisoso BH','HBV TABLIZO ST, MATI DIGOS CITY','Boarding House at affordable price','9 pm curfew, no pets allowed(bois)','Fast internet, cozy, friendly environment',_binary '\0','2026-02-22 19:00:45',NULL,NULL,NULL);
/*!40000 ALTER TABLE `boardinghouses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `expenses`
--

DROP TABLE IF EXISTS `expenses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `expenses` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) NOT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `ExpenseDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `Category` varchar(50) DEFAULT 'General',
  `BoardingHouseId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `BoardingHouseId` (`BoardingHouseId`),
  CONSTRAINT `expenses_ibfk_1` FOREIGN KEY (`BoardingHouseId`) REFERENCES `boardinghouses` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `expenses`
--

LOCK TABLES `expenses` WRITE;
/*!40000 ALTER TABLE `expenses` DISABLE KEYS */;
/*!40000 ALTER TABLE `expenses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `BoarderId` int NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `PaymentDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `MonthPaid` varchar(20) NOT NULL,
  `YearPaid` int NOT NULL,
  `Status` varchar(20) DEFAULT 'Pending',
  `Notes` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `BoarderId` (`BoarderId`),
  CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`BoarderId`) REFERENCES `boarders` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,1,1000.00,'2026-02-17 11:43:50','February',2026,'Pending','bayari napud'),(2,1,1000.00,'2026-02-17 11:51:19','February',2026,'Pending','hoy ting bayad na'),(3,2,1000.00,'2026-02-18 11:17:42','February',2025,'Pending','bayad nimo ron');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rooms` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoomNumber` varchar(20) NOT NULL,
  `Capacity` int NOT NULL,
  `MonthlyRate` decimal(10,2) NOT NULL,
  `IsActive` bit(1) DEFAULT b'1',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `BoardingHouseId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Rooms_BH_Room` (`BoardingHouseId`,`RoomNumber`),
  KEY `BoardingHouseId` (`BoardingHouseId`),
  CONSTRAINT `rooms_ibfk_1` FOREIGN KEY (`BoardingHouseId`) REFERENCES `boardinghouses` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rooms`
--

LOCK TABLES `rooms` WRITE;
/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
INSERT INTO `rooms` VALUES (1,'101',2,5000.00,_binary '','2026-02-16 11:42:24',1),(2,'102',4,3500.00,_binary '','2026-02-16 11:42:24',1),(3,'201',1,8000.00,_binary '','2026-02-16 11:42:24',1),(6,'501',4,1000.00,_binary '','2026-02-16 12:08:04',2),(7,'101',4,1000.00,_binary '','2026-02-16 12:13:36',2),(8,'102',4,1000.00,_binary '','2026-02-19 21:30:19',2);
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` varchar(20) NOT NULL,
  `IsActive` bit(1) DEFAULT b'1',
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'superadmin','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9','SuperAdmin',_binary '','2026-02-16 11:42:24'),(2,'admin','240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9','Admin',_binary '','2026-02-16 11:42:24'),(3,'alisoso','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Admin',_binary '','2026-02-16 11:45:52'),(4,'keith','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Boarder',_binary '','2026-02-16 12:03:16'),(5,'rodz','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Boarder',_binary '','2026-02-18 11:17:34');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-02-22 23:08:08
