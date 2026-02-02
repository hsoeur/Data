CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260130123058_AddManufacturingOrder') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260130123058_AddManufacturingOrder') THEN

    CREATE TABLE `ManufacturingOrders` (
        `ManufacturingOrderUid` char(36) COLLATE ascii_general_ci NOT NULL,
        `ProductUid` char(36) COLLATE ascii_general_ci NOT NULL,
        `Quantity` int NOT NULL,
        `FirstSerialNumber` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `InternalOrderNumber` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `ExternalOrderNumber` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `Status` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ManufacturingOrders` PRIMARY KEY (`ManufacturingOrderUid`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260130123058_AddManufacturingOrder') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260130123058_AddManufacturingOrder', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

