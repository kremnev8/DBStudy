-- MySQL dump 10.13  Distrib 5.6.26, for Win64 (x86_64)
--
-- Host: localhost    Database: vending_machine_business
-- ------------------------------------------------------
-- Server version	5.6.26-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES cp1251 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `delivery`
--

DROP TABLE IF EXISTS `delivery`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `delivery` (
  `DeliveryId` int(11) NOT NULL AUTO_INCREMENT,
  `MachineId` int(11) NOT NULL,
  `EmployeeId` int(11) NOT NULL,
  `WithdrawnMoney` decimal(10,2) NOT NULL,
  `DeliveryDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`DeliveryId`),
  KEY `Emp -D` (`EmployeeId`),
  KEY `VM - D` (`MachineId`),
  CONSTRAINT `Emp -D` FOREIGN KEY (`EmployeeId`) REFERENCES `employee` (`EmployeeId`) ON UPDATE CASCADE,
  CONSTRAINT `VM - D` FOREIGN KEY (`MachineId`) REFERENCES `vendingmachine` (`MachineId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `delivery`
--

/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = cp1251 */ ;
/*!50003 SET character_set_results = cp1251 */ ;
/*!50003 SET collation_connection  = cp1251_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER OnDeliveryDone

	AFTER INSERT

	ON delivery

	FOR EACH ROW

BEGIN



  UPDATE vending_machine_business.vendingmachine vm

  SET vm.CashStored = vm.CashStored - NEW.WithdrawnMoney

  WHERE vm.MachineId = NEW.MachineId;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `deliverycontents`
--

DROP TABLE IF EXISTS `deliverycontents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `deliverycontents` (
  `ItemId` int(11) NOT NULL AUTO_INCREMENT,
  `DeliveryId` int(11) NOT NULL,
  `SlotPosition` int(11) NOT NULL,
  `GoodCount` int(11) NOT NULL,
  `GoodId` int(11) NOT NULL,
  PRIMARY KEY (`ItemId`),
  KEY `D - DC` (`DeliveryId`),
  KEY `DC - GD` (`GoodId`),
  CONSTRAINT `D - DC` FOREIGN KEY (`DeliveryId`) REFERENCES `delivery` (`DeliveryId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `DC - GD` FOREIGN KEY (`GoodId`) REFERENCES `good` (`GoodId`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `deliverycontents`
--

/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = cp1251 */ ;
/*!50003 SET character_set_results = cp1251 */ ;
/*!50003 SET collation_connection  = cp1251_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `On Item Delivered`

	AFTER INSERT

	ON deliverycontents

	FOR EACH ROW

BEGIN



  DECLARE machineID int;



  SELECT d.MachineId INTO machineID FROM delivery d 

  WHERE d.DeliveryId = NEW.DeliveryId;



  IF NOT EXISTS (

    SELECT 1 FROM vending_machine_business.vendingmachineslot vms

    WHERE machineID = vms.MachineId

    AND vms.SlotPosition = NEW.SlotPosition

    AND vms.GoodId = NEW.GoodId) THEN



      INSERT INTO vending_machine_business.vendingmachineslot 

      (MachineId, SlotPosition, GoodId, GoodCount)

      VALUES (machineID, NEW.SlotPosition, NEW.GoodId, NEW.GoodCount);



  ELSE



    UPDATE vending_machine_business.vendingmachineslot vms

    SET vms.GoodCount = vms.GoodCount + NEW.GoodCount

    WHERE machineID = vms.MachineId

    AND vms.SlotPosition = NEW.SlotPosition

    AND vms.GoodId = NEW.GoodId;



  END IF;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `employee` (
  `EmployeeId` int(11) NOT NULL AUTO_INCREMENT,
  `FullName` varchar(255) NOT NULL DEFAULT '',
  `Salary` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Email` varchar(50) NOT NULL DEFAULT '',
  `Password` varchar(50) NOT NULL DEFAULT '',
  `PermissionId` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`EmployeeId`),
  UNIQUE KEY `UK_employee_Email` (`Email`),
  KEY `FK_employee` (`PermissionId`),
  CONSTRAINT `FK_employee` FOREIGN KEY (`PermissionId`) REFERENCES `employeepermission` (`PermissionId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

--
-- Table structure for table `employeepermission`
--

DROP TABLE IF EXISTS `employeepermission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `employeepermission` (
  `PermissionId` int(11) NOT NULL AUTO_INCREMENT,
  `PermissionName` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`PermissionId`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employeepermission`
--

--
-- Table structure for table `good`
--

DROP TABLE IF EXISTS `good`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `good` (
  `GoodId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL DEFAULT '',
  `PurchaseCost` decimal(10,2) NOT NULL,
  `SaleCost` decimal(10,2) NOT NULL,
  `SupplierId` int(11) NOT NULL,
  `IconPath` varchar(50) CHARACTER SET cp1251 DEFAULT '',
  PRIMARY KEY (`GoodId`),
  KEY `Good By Name` (`Name`),
  KEY `GD - SP` (`SupplierId`),
  CONSTRAINT `GD - SP` FOREIGN KEY (`SupplierId`) REFERENCES `supplier` (`SupplierId`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `good`
--

--
-- Table structure for table `purchase`
--

DROP TABLE IF EXISTS `purchase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `purchase` (
  `PurchaseId` int(11) NOT NULL AUTO_INCREMENT,
  `MachineId` int(11) NOT NULL,
  `GoodId` int(11) NOT NULL,
  `SlotPosition` int(11) NOT NULL,
  `GoodCount` int(11) NOT NULL,
  `PurchaseTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`PurchaseId`),
  KEY `PR - GD` (`GoodId`),
  KEY `VM - P` (`MachineId`),
  CONSTRAINT `PR - GD` FOREIGN KEY (`GoodId`) REFERENCES `good` (`GoodId`) ON UPDATE CASCADE,
  CONSTRAINT `VM - P` FOREIGN KEY (`MachineId`) REFERENCES `vendingmachine` (`MachineId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=cp1251 AVG_ROW_LENGTH=1820;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `purchase`
--

/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = cp1251 */ ;
/*!50003 SET character_set_results = cp1251 */ ;
/*!50003 SET collation_connection  = cp1251_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `Purchase Change Count`

	AFTER INSERT

	ON purchase

	FOR EACH ROW

BEGIN

 

  DECLARE goodPrice decimal(10,2);



  UPDATE vending_machine_business.vendingmachineslot vms

  SET vms.GoodCount = vms.GoodCount - NEW.GoodCount

  WHERE vms.MachineId = NEW.MachineId AND vms.SlotPosition = NEW.SlotPosition

  AND vms.GoodId = NEW.GoodId;





  SELECT SaleCost INTO goodPrice FROM good WHERE GoodId = NEW.GoodId;



  UPDATE vending_machine_business.vendingmachine vm

  SET vm.CashStored = vm.CashStored + goodPrice

  WHERE vm.MachineId = NEW.MachineId;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `supplier`
--

DROP TABLE IF EXISTS `supplier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `supplier` (
  `SupplierId` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`SupplierId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier`
--

--
-- Table structure for table `vendingmachine`
--

DROP TABLE IF EXISTS `vendingmachine`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendingmachine` (
  `MachineId` int(11) NOT NULL AUTO_INCREMENT,
  `EmployeeId` int(11) DEFAULT NULL,
  `Address` varchar(255) NOT NULL DEFAULT '',
  `AccessToken` varchar(64) NOT NULL DEFAULT '',
  `CashStored` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`MachineId`),
  KEY `FK_vending_machine` (`EmployeeId`),
  KEY `IDX_vendingmachine_AccessToken` (`AccessToken`),
  CONSTRAINT `FK_vending_machine` FOREIGN KEY (`EmployeeId`) REFERENCES `employee` (`EmployeeId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendingmachine`
--
--
-- Table structure for table `vendingmachineslot`
--

DROP TABLE IF EXISTS `vendingmachineslot`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendingmachineslot` (
  `SlotId` int(11) NOT NULL AUTO_INCREMENT,
  `MachineId` int(11) NOT NULL,
  `SlotPosition` int(11) NOT NULL,
  `GoodId` int(11) DEFAULT NULL,
  `GoodCount` int(11) NOT NULL,
  PRIMARY KEY (`SlotId`),
  KEY `VM - VMS` (`MachineId`),
  KEY `VMS - GD` (`GoodId`),
  CONSTRAINT `VM - VMS` FOREIGN KEY (`MachineId`) REFERENCES `vendingmachine` (`MachineId`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `VMS - GD` FOREIGN KEY (`GoodId`) REFERENCES `good` (`GoodId`) ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendingmachineslot`
--

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-12-15  9:06:16
