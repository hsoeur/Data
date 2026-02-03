CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___efmigrationshistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260130123058_AddManufacturingOrder') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260130123058_AddManufacturingOrder') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260130123058_AddManufacturingOrder') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260130123058_AddManufacturingOrder', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202121125_AddOFNotifications') THEN

    ALTER TABLE `ManufacturingOrders` MODIFY COLUMN `ManufacturingOrderUid` int NOT NULL AUTO_INCREMENT;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202121125_AddOFNotifications') THEN

    CREATE TABLE `ManufacturingOrderNotifications` (
        `NotificationUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `OfNumber` int NOT NULL,
        `ChangeType` longtext CHARACTER SET utf8mb4 NOT NULL,
        `OldValue` longtext CHARACTER SET utf8mb4 NOT NULL,
        `NewValue` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ProcessedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
        `CreatedDate` datetime(6) NOT NULL,
        `Processed` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ProcessedDate` datetime(6) NULL,
        CONSTRAINT `PK_ManufacturingOrderNotifications` PRIMARY KEY (`NotificationUid`)
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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202121125_AddOFNotifications') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202121125_AddOFNotifications', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202122344_ChangeProductUidToString') THEN

    ALTER TABLE `ManufacturingOrders` MODIFY COLUMN `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202122344_ChangeProductUidToString') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202122344_ChangeProductUidToString', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202122705_ChangeFirstSerialNumberType') THEN

    UPDATE `ManufacturingOrders` SET `FirstSerialNumber` = '0' WHERE `FirstSerialNumber` IS NULL OR `FirstSerialNumber` = '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202122705_ChangeFirstSerialNumberType') THEN

    ALTER TABLE `ManufacturingOrders` MODIFY COLUMN `FirstSerialNumber` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202122705_ChangeFirstSerialNumberType') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202122705_ChangeFirstSerialNumberType', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202130529_AddNotificationModelChanges') THEN

    UPDATE `ManufacturingOrderNotifications` SET `OldValue` = '0' WHERE `OldValue` = '' OR `OldValue` IS NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202130529_AddNotificationModelChanges') THEN

    UPDATE `ManufacturingOrderNotifications` SET `NewValue` = '0' WHERE `NewValue` = '' OR `NewValue` IS NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202130529_AddNotificationModelChanges') THEN

    ALTER TABLE `ManufacturingOrderNotifications` MODIFY COLUMN `OldValue` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202130529_AddNotificationModelChanges') THEN

    ALTER TABLE `ManufacturingOrderNotifications` MODIFY COLUMN `NewValue` int NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202130529_AddNotificationModelChanges') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202130529_AddNotificationModelChanges', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202144955_AddArticleTable') THEN

    CREATE TABLE `Articles` (
        `ArticleUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Designation` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Store` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Family` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Packaging` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ManageStock` longtext CHARACTER SET utf8mb4 NOT NULL,
        `SupplierReference` longtext CHARACTER SET utf8mb4 NOT NULL,
        `GpaoQuantity` int NOT NULL,
        CONSTRAINT `PK_Articles` PRIMARY KEY (`ArticleUid`)
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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202144955_AddArticleTable') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202144955_AddArticleTable', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202152026_AddBOM') THEN

    CREATE TABLE `BillOfMaterials` (
        `BillOfMaterialsUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Component` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Operation` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Quantity` longtext CHARACTER SET utf8mb4 NOT NULL,
        `QuantityDiv` longtext CHARACTER SET utf8mb4 NOT NULL,
        `CreationDate` longtext CHARACTER SET utf8mb4 NOT NULL,
        `EndDate` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_BillOfMaterials` PRIMARY KEY (`BillOfMaterialsUid`)
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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202152026_AddBOM') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202152026_AddBOM', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202155154_DateFormatBOM') THEN

    ALTER TABLE `BillOfMaterials` MODIFY COLUMN `EndDate` datetime(6) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202155154_DateFormatBOM') THEN

    ALTER TABLE `BillOfMaterials` MODIFY COLUMN `CreationDate` datetime(6) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202155154_DateFormatBOM') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202155154_DateFormatBOM', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202161648_ArticleVersions') THEN

    CREATE TABLE `ArticleVersions` (
        `ArticleVersionUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ArticleUid` longtext CHARACTER SET utf8mb4 NOT NULL,
        `FirmwareVersion` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_ArticleVersions` PRIMARY KEY (`ArticleVersionUid`)
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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260202161648_ArticleVersions') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260202161648_ArticleVersions', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260203081005_Firmwares') THEN

    CREATE TABLE `Firmwares` (
        `FirmwareUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
        `File` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Version` longtext CHARACTER SET utf8mb4 NOT NULL,
        `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
        `StartDate` datetime(6) NOT NULL,
        `EndDate` datetime(6) NOT NULL,
        `IsSuspended` tinyint(1) NOT NULL,
        CONSTRAINT `PK_Firmwares` PRIMARY KEY (`FirmwareUid`)
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
    IF NOT EXISTS(SELECT 1 FROM `__efmigrationshistory` WHERE `MigrationId` = '20260203081005_Firmwares') THEN

    INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260203081005_Firmwares', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

