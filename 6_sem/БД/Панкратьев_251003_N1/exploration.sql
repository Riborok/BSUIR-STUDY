-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: exploration
-- ------------------------------------------------------
-- Server version	8.3.0

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
-- Table structure for table `cities`
--

DROP TABLE IF EXISTS `cities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cities` (
  `ct_id` int NOT NULL AUTO_INCREMENT,
  `ct_name` varchar(50) NOT NULL,
  PRIMARY KEY (`ct_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cities`
--

LOCK TABLES `cities` WRITE;
/*!40000 ALTER TABLE `cities` DISABLE KEYS */;
INSERT INTO `cities` VALUES (1,'Лондон'),(2,'Париж'),(3,'Мадрид'),(4,'Токио'),(5,'Москва'),(6,'Киев'),(7,'Минск'),(8,'Рига'),(9,'Варшава'),(10,'Берлин');
/*!40000 ALTER TABLE `cities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `computers`
--

DROP TABLE IF EXISTS `computers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `computers` (
  `c_id` int NOT NULL AUTO_INCREMENT,
  `c_room` int DEFAULT NULL,
  `c_name` varchar(50) NOT NULL,
  PRIMARY KEY (`c_id`),
  KEY `FK_computers_rooms` (`c_room`),
  CONSTRAINT `FK_computers_rooms` FOREIGN KEY (`c_room`) REFERENCES `rooms` (`r_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `computers`
--

LOCK TABLES `computers` WRITE;
/*!40000 ALTER TABLE `computers` DISABLE KEYS */;
INSERT INTO `computers` VALUES (1,1,'Компьютер A в комнате 1'),(2,1,'Компьютер B в комнате 1'),(3,2,'Компьютер A в комнате 2'),(4,2,'Компьютер B в комнате 2'),(5,2,'Компьютер C в комнате 2'),(6,NULL,'Свободный компьютер A'),(7,NULL,'Свободный компьютер B'),(8,NULL,'Свободный компьютер C');
/*!40000 ALTER TABLE `computers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `connections`
--

DROP TABLE IF EXISTS `connections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `connections` (
  `cn_from` int NOT NULL,
  `cn_to` int NOT NULL,
  `cn_cost` double(10,2) DEFAULT NULL,
  `cn_bidir` enum('N','Y') NOT NULL,
  PRIMARY KEY (`cn_from`,`cn_to`),
  KEY `FK_connections_cities2` (`cn_to`),
  CONSTRAINT `FK_connections_cities1` FOREIGN KEY (`cn_from`) REFERENCES `cities` (`ct_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_connections_cities2` FOREIGN KEY (`cn_to`) REFERENCES `cities` (`ct_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `connections`
--

LOCK TABLES `connections` WRITE;
/*!40000 ALTER TABLE `connections` DISABLE KEYS */;
INSERT INTO `connections` VALUES (1,5,10.00,'Y'),(1,7,20.00,'N'),(2,6,50.00,'N'),(3,6,5.00,'N'),(4,8,35.00,'N'),(6,8,40.00,'Y'),(7,1,25.00,'N'),(7,2,15.00,'Y'),(7,3,5.00,'N'),(8,4,30.00,'N'),(8,9,15.00,'Y'),(9,1,20.00,'N');
/*!40000 ALTER TABLE `connections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dates`
--

DROP TABLE IF EXISTS `dates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dates` (
  `d` date DEFAULT NULL,
  KEY `idx_d` (`d`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dates`
--

LOCK TABLES `dates` WRITE;
/*!40000 ALTER TABLE `dates` DISABLE KEYS */;
/*!40000 ALTER TABLE `dates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `library_in_json`
--

DROP TABLE IF EXISTS `library_in_json`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `library_in_json` (
  `lij_id` int unsigned NOT NULL AUTO_INCREMENT,
  `lij_book` varchar(150) NOT NULL,
  `lij_author` json NOT NULL,
  `lij_genre` json NOT NULL,
  PRIMARY KEY (`lij_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `library_in_json`
--

LOCK TABLES `library_in_json` WRITE;
/*!40000 ALTER TABLE `library_in_json` DISABLE KEYS */;
INSERT INTO `library_in_json` VALUES (1,'Евгений Онегин','[{\"id\": 7, \"name\": \"А.С. Пушкин\"}]','[{\"id\": 1, \"name\": \"Поэзия\"}, {\"id\": 5, \"name\": \"Классика\"}]'),(2,'Искусство программирования','[{\"id\": 1, \"name\": \"Д. Кнут\"}]','[{\"id\": 2, \"name\": \"Программирование\"}, {\"id\": 5, \"name\": \"Классика\"}]'),(3,'Курс теоретической физики','[{\"id\": 4, \"name\": \"Л.Д. Ландау\"}, {\"id\": 5, \"name\": \"Е.М. Лифшиц\"}]','[{\"id\": 5, \"name\": \"Классика\"}]'),(4,'Основание и империя','[{\"id\": 2, \"name\": \"А. Азимов\"}]','[{\"id\": 6, \"name\": \"Фантастика\"}]'),(5,'Психология программирования','[{\"id\": 3, \"name\": \"Д. Карнеги\"}, {\"id\": 6, \"name\": \"Б. Страуструп\"}]','[{\"id\": 2, \"name\": \"Программирование\"}, {\"id\": 3, \"name\": \"Психология\"}]'),(6,'Сказка о рыбаке и рыбке','[{\"id\": 7, \"name\": \"А.С. Пушкин\"}]','[{\"id\": 1, \"name\": \"Поэзия\"}, {\"id\": 5, \"name\": \"Классика\"}]'),(7,'Язык программирования С++','[{\"id\": 6, \"name\": \"Б. Страуструп\"}]','[{\"id\": 2, \"name\": \"Программирование\"}]');
/*!40000 ALTER TABLE `library_in_json` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `overflow`
--

DROP TABLE IF EXISTS `overflow`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `overflow` (
  `x` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `overflow`
--

LOCK TABLES `overflow` WRITE;
/*!40000 ALTER TABLE `overflow` DISABLE KEYS */;
/*!40000 ALTER TABLE `overflow` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rooms` (
  `r_id` int NOT NULL AUTO_INCREMENT,
  `r_name` varchar(50) NOT NULL,
  `r_space` tinyint NOT NULL,
  PRIMARY KEY (`r_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rooms`
--

LOCK TABLES `rooms` WRITE;
/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
INSERT INTO `rooms` VALUES (1,'Комната с двумя компьютерами',5),(2,'Комната с тремя компьютерами',5),(3,'Пустая комната 1',2),(4,'Пустая комната 2',2),(5,'Пустая комната 3',2);
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopping`
--

DROP TABLE IF EXISTS `shopping`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopping` (
  `sh_id` int unsigned NOT NULL AUTO_INCREMENT,
  `sh_transaction` int unsigned NOT NULL,
  `sh_category` varchar(150) NOT NULL,
  PRIMARY KEY (`sh_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopping`
--

LOCK TABLES `shopping` WRITE;
/*!40000 ALTER TABLE `shopping` DISABLE KEYS */;
INSERT INTO `shopping` VALUES (1,1,'Сумка'),(2,1,'Платье'),(3,1,'Сумка'),(4,2,'Сумка'),(5,2,'Юбка'),(6,3,'Платье'),(7,3,'Юбка'),(8,3,'Туфли'),(9,3,'Шляпка'),(10,4,'Сумка');
/*!40000 ALTER TABLE `shopping` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `site_pages`
--

DROP TABLE IF EXISTS `site_pages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `site_pages` (
  `sp_id` int unsigned NOT NULL AUTO_INCREMENT,
  `sp_parent` int unsigned DEFAULT NULL,
  `sp_name` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`sp_id`),
  KEY `FK_site_pages_site_pages` (`sp_parent`),
  CONSTRAINT `FK_site_pages_site_pages` FOREIGN KEY (`sp_parent`) REFERENCES `site_pages` (`sp_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `site_pages`
--

LOCK TABLES `site_pages` WRITE;
/*!40000 ALTER TABLE `site_pages` DISABLE KEYS */;
INSERT INTO `site_pages` VALUES (1,NULL,'Главная'),(2,1,'Читателям'),(3,1,'Спонсорам'),(4,1,'Рекламодателям'),(5,2,'Новости'),(6,2,'Статистика'),(7,3,'Предложения'),(8,3,'Истории успеха'),(9,4,'Акции'),(10,1,'Контакты'),(11,3,'Документы'),(12,6,'Текущая'),(13,6,'Архивная'),(14,6,'Неофициальная');
/*!40000 ALTER TABLE `site_pages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `table_with_nulls`
--

DROP TABLE IF EXISTS `table_with_nulls`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `table_with_nulls` (
  `x` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `table_with_nulls`
--

LOCK TABLES `table_with_nulls` WRITE;
/*!40000 ALTER TABLE `table_with_nulls` DISABLE KEYS */;
/*!40000 ALTER TABLE `table_with_nulls` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `test_counts`
--

DROP TABLE IF EXISTS `test_counts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `test_counts` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fni` int DEFAULT NULL,
  `fwi` int DEFAULT NULL,
  `fni_nn` int NOT NULL,
  `fwi_nn` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_fwi` (`fwi`),
  KEY `idx_fwi_nn` (`fwi_nn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `test_counts`
--

LOCK TABLES `test_counts` WRITE;
/*!40000 ALTER TABLE `test_counts` DISABLE KEYS */;
/*!40000 ALTER TABLE `test_counts` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-01-31 18:30:35
