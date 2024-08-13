CREATE TABLE `Products` (
    `Id` VARCHAR(255) NOT NULL,
    `Name` VARCHAR(255) NOT NULL,
    `UnitPrice` DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `PricingRules` (
    `Item` VARCHAR(255),
    `UnitPrice` DECIMAL(18, 2) NOT NULL,
    `SpecialQuantity` INT,
    `SpecialPrice` DECIMAL(18, 2),
    PRIMARY KEY (`Item`)
);
