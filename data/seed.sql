
DROP TABLE IF EXISTS `CheckoutItems`;

CREATE TABLE `CheckoutItems` (
  `SessionId` varchar(36) NOT NULL,
  `ProductId` varchar(50) NOT NULL,
  `Quantity` int NOT NULL,
  PRIMARY KEY (`SessionId`,`ProductId`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `CheckoutItems_ibfk_1` FOREIGN KEY (`SessionId`) REFERENCES `CheckoutSessions` (`SessionId`),
  CONSTRAINT `CheckoutItems_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Table structure for table `CheckoutSessions`
--

DROP TABLE IF EXISTS `CheckoutSessions`;

CREATE TABLE `CheckoutSessions` (
  `SessionId` varchar(36) NOT NULL,
  `Status` varchar(20) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `EndedAt` datetime DEFAULT NULL,
  `TotalAmount` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`SessionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Table structure for table `PricingRules`
--

DROP TABLE IF EXISTS `PricingRules`;

CREATE TABLE `PricingRules` (
  `ProductId` varchar(255) NOT NULL,
  `UnitPrice` decimal(18,2) NOT NULL,
  `SpecialQuantity` int DEFAULT NULL,
  `SpecialPrice` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`ProductId`),
  CONSTRAINT `PricingRules_ibfk_1` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `PricingRules`
--

LOCK TABLES `PricingRules` WRITE;
ALTER TABLE `PricingRules` DISABLE KEYS 
('SKU001', 0.50, 10, 4.50),
('SKU002', 0.30, 5, 1.20),
('SKU004', 1.20, 3, 3.00),
('SKU006', 1.50, 2, 2.50),
('SKU008', 2.50, 12, 25.00),
('SKU010', 7.00, 2, 12.00),
('SKU012', 8.00, 3, 20.00),
('SKU015', 0.40, 10, 3.50),
('SKU018', 0.10, 20, 1.50),
('SKU020', 0.80, 5, 3.50),
('SKU022', 0.50, 6, 2.50),
('SKU025', 0.50, 10, 4.00),
('SKU027', 1.50, 3, 4.00),
('SKU030', 1.00, 4, 3.00),
('SKU033', 2.00, 5, 9.00),
('SKU035', 1.00, 6, 5.00),
('SKU038', 1.50, 4, 5.00),
('SKU041', 1.50, 3, 4.00),
('SKU044', 1.50, 5, 6.00),
('SKU047', 2.50, 2, 4.00);
ALTER TABLE `PricingRules` ENABLE KEYS
UNLOCK TABLES;

--
-- Table structure for table `Products`
--

DROP TABLE IF EXISTS `Products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Products` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `UnitPrice` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Products`
--

LOCK TABLES `Products` WRITE;
ALTER TABLE `PricingRules` DISABLE KEYS
INSERT INTO `Products` (`Id`, `Name`, `UnitPrice`) VALUES
('SKU001', 'Apple', 0.50),
('SKU002', 'Banana', 0.30),
('SKU003', 'Orange', 0.60),
('SKU004', 'Milk', 1.20),
('SKU005', 'Bread', 1.00),
('SKU006', 'Butter', 1.50),
('SKU007', 'Cheese', 2.00),
('SKU008', 'Eggs', 2.50),
('SKU009', 'Chicken', 5.00),
('SKU010', 'Beef', 7.00),
('SKU011', 'Pork', 6.00),
('SKU012', 'Fish', 8.00),
('SKU013', 'Rice', 1.00),
('SKU014', 'Pasta', 1.20),
('SKU015', 'Tomato', 0.40),
('SKU016', 'Potato', 0.30),
('SKU017', 'Onion', 0.20),
('SKU018', 'Garlic', 0.10),
('SKU019', 'Carrot', 0.25),
('SKU020', 'Broccoli', 0.80),
('SKU021', 'Lettuce', 0.70),
('SKU022', 'Cucumber', 0.50),
('SKU023', 'Pepper', 0.60),
('SKU024', 'Salt', 0.20),
('SKU025', 'Sugar', 0.50),
('SKU026', 'Flour', 0.70),
('SKU027', 'Oil', 1.50),
('SKU028', 'Vinegar', 1.00),
('SKU029', 'Ketchup', 1.20),
('SKU030', 'Mustard', 1.00),
('SKU031', 'Mayonnaise', 1.50),
('SKU032', 'Yogurt', 1.00),
('SKU033', 'Juice', 2.00),
('SKU034', 'Water', 0.50),
('SKU035', 'Soda', 1.00),
('SKU036', 'Coffee', 3.00),
('SKU037', 'Tea', 2.50),
('SKU038', 'Chocolate', 1.50),
('SKU039', 'Candy', 0.80),
('SKU040', 'Chips', 1.20),
('SKU041', 'Cookies', 1.50),
('SKU042', 'Ice Cream', 2.00),
('SKU043', 'Cereal', 2.50),
('SKU044', 'Oatmeal', 1.50),
('SKU045', 'Honey', 3.00),
('SKU046', 'Jam', 2.00),
('SKU047', 'Peanut Butter', 2.50),
('SKU048', 'Soup', 1.00),
('SKU049', 'Sauce', 1.50),
('SKU050', 'Spices', 0.70);
ALTER TABLE `PricingRules` ENABLE KEYS
UNLOCK TABLES;

--
-- Table structure for table `schemaversions`
--

DROP TABLE IF EXISTS `schemaversions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schemaversions` (
  `schemaversionid` int NOT NULL AUTO_INCREMENT,
  `scriptname` varchar(255) NOT NULL,
  `applied` timestamp NOT NULL,
  PRIMARY KEY (`schemaversionid`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `schemaversions`
--

LOCK TABLES `schemaversions` WRITE;
INSERT INTO `schemaversions` VALUES (1,'CheckoutPricing.Api.Data.Migrations.001_CreateTables.sql','2024-08-13 11:14:32');
UNLOCK TABLES;
