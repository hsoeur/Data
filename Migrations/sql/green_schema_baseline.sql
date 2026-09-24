-- API Data - Green schema baseline
-- Generated from the current EF Core model for MySQL 8.0.36.
-- Apply only to a new, empty Green database selected by the connection.
-- This script creates no business data. It must never be run on Blue.

ALTER DATABASE CHARACTER SET utf8mb4;


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


CREATE TABLE `BillOfMaterials` (
    `BillOfMaterialsUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Component` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Operation` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Quantity` longtext CHARACTER SET utf8mb4 NOT NULL,
    `QuantityDiv` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_BillOfMaterials` PRIMARY KEY (`BillOfMaterialsUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `DatabaseNotifications` (
    `DatabaseNotificationUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `EntityType` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EntityUId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PropertyName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OldValue` longtext CHARACTER SET utf8mb4 NOT NULL,
    `NewValue` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ModifiedAt` datetime(6) NOT NULL,
    `ModifiedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Processed` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProcessedDate` datetime(6) NOT NULL,
    `ProcessedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Comment` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_DatabaseNotifications` PRIMARY KEY (`DatabaseNotificationUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `DeviceIdentities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `EquipSN` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `MacAddressValue` char(17) CHARACTER SET utf8mb4 NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_DeviceIdentities` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `FirmwareVersions` (
    `FirmwareVersionUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ArticleUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Version` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_FirmwareVersions` PRIMARY KEY (`FirmwareVersionUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `KitAccTraceabilities` (
    `KitAccTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClientOrder` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OrderLineUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SerialNumber` int NOT NULL,
    `ScanFrame` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Date` datetime(6) NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_KitAccTraceabilities` PRIMARY KEY (`KitAccTraceabilityUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `KitTraceabilities` (
    `KitTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClientOrder` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ManufacturingOrderUid` int NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `HorizonComponentCode` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SerialNumber` int NOT NULL,
    `ScanFrame` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Date` datetime(6) NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_KitTraceabilities` PRIMARY KEY (`KitTraceabilityUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `LabelTypes` (
    `LabelTypeUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_LabelTypes` PRIMARY KEY (`LabelTypeUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `MacAddresses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `MacAddressValue` char(17) CHARACTER SET utf8mb4 NOT NULL,
    `Status` enum('AVAILABLE','RESERVED','USED') CHARACTER SET utf8mb4 NOT NULL,
    `ReservedAt` datetime(6) NULL,
    `UsedAt` datetime(6) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_MacAddresses` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Machines` (
    `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Machines` PRIMARY KEY (`MachineUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ManufacturingOrderNotifications` (
    `NotificationUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `OfNumber` int NOT NULL,
    `ChangeType` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OldValue` int NOT NULL,
    `NewValue` int NOT NULL,
    `ProcessedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreatedDate` datetime(6) NOT NULL,
    `Processed` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProcessedDate` datetime(6) NULL,
    CONSTRAINT `PK_ManufacturingOrderNotifications` PRIMARY KEY (`NotificationUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ManufacturingOrders` (
    `ManufacturingOrderUid` int NOT NULL AUTO_INCREMENT,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Quantity` int NOT NULL,
    `FirstSerialNumber` int NOT NULL,
    `InternalOrderNumber` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `ExternalOrderNumber` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `Status` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ManufacturingOrders` PRIMARY KEY (`ManufacturingOrderUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Operations` (
    `OperationUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OperationNumber` int NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Machine` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PrepTime` decimal(10,2) NOT NULL,
    `ProdTime` decimal(10,2) NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_Operations` PRIMARY KEY (`OperationUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `OrderLines` (
    `UidOrderLine` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Quantity` int NOT NULL,
    CONSTRAINT `PK_OrderLines` PRIMARY KEY (`UidOrderLine`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `PackagingLocationPhotos` (
    `PackagingLocationPhotoUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ArticleUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Photo` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UseCase` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_PackagingLocationPhotos` PRIMARY KEY (`PackagingLocationPhotoUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ProductStatuses` (
    `ProductStatusUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ProductStatuses` PRIMARY KEY (`ProductStatusUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `SimCards` (
    `SimCardUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `PhoneNumber` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    `Provider` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
    `SimCardNumber` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_SimCards` PRIMARY KEY (`SimCardUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Units` (
    `UnitUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Units` PRIMARY KEY (`UnitUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Users` (
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LastName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FirstName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EntryDate` datetime(6) NOT NULL,
    `ExitDate` datetime(6) NOT NULL,
    `Department` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`UserUid`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Sequences` (
    `SequenceUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `OperationNumber` int NOT NULL,
    `File` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_Sequences` PRIMARY KEY (`SequenceUid`),
    CONSTRAINT `FK_Sequences_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `LabelTemplates` (
    `LabelTemplateUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LabelTypeUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `TemplateFile` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Photo` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_LabelTemplates` PRIMARY KEY (`LabelTemplateUid`),
    CONSTRAINT `FK_LabelTemplates_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_LabelTemplates_LabelTypes_LabelTypeUid` FOREIGN KEY (`LabelTypeUid`) REFERENCES `LabelTypes` (`LabelTypeUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `Firmwares` (
    `FirmwareUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `File` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Version` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `IsSuspended` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Firmwares` PRIMARY KEY (`FirmwareUid`),
    CONSTRAINT `FK_Firmwares_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Firmwares_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `Labels` (
    `LabelUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AuthentiUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ManufacturingOrderUid` int NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    `CurrentSerialNumber` int NOT NULL,
    `ProductVersion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `LabelTypeUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Template` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Labels` PRIMARY KEY (`LabelUid`),
    CONSTRAINT `FK_Labels_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Labels_LabelTypes_LabelTypeUid` FOREIGN KEY (`LabelTypeUid`) REFERENCES `LabelTypes` (`LabelTypeUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Labels_ManufacturingOrders_ManufacturingOrderUid` FOREIGN KEY (`ManufacturingOrderUid`) REFERENCES `ManufacturingOrders` (`ManufacturingOrderUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Labels_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `ProductTraceabilities` (
    `ProductTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProductSerialNumber` int NOT NULL,
    `ProductStatusUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `FailureComment` longtext CHARACTER SET utf8mb4 NULL,
    `CreationDate` datetime(6) NOT NULL,
    `ManufacturingOrderUid` int NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CycleTime` decimal(10,2) NOT NULL,
    `OperationNumber` int NOT NULL,
    `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ProductTraceabilities` PRIMARY KEY (`ProductTraceabilityUid`),
    CONSTRAINT `FK_ProductTraceabilities_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ProductTraceabilities_ProductStatuses_ProductStatusUid` FOREIGN KEY (`ProductStatusUid`) REFERENCES `ProductStatuses` (`ProductStatusUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ProductTraceabilities_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `SolderingPrograms` (
    `SolderingProgramUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Program` int NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Side` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_SolderingPrograms` PRIMARY KEY (`SolderingProgramUid`),
    CONSTRAINT `FK_SolderingPrograms_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_SolderingPrograms_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `Tolerances` (
    `ToleranceUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Val1` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Val2` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `OperationNumber` int NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Mode` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UnitUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Tolerances` PRIMARY KEY (`ToleranceUid`),
    CONSTRAINT `FK_Tolerances_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Tolerances_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Tolerances_Units_UnitUid` FOREIGN KEY (`UnitUid`) REFERENCES `Units` (`UnitUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Tolerances_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `MeasurementTraceabilities` (
    `MeasurementTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ToleranceUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Classification` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_MeasurementTraceabilities` PRIMARY KEY (`MeasurementTraceabilityUid`),
    CONSTRAINT `FK_MeasurementTraceabilities_ProductTraceabilities_ProductTrace~` FOREIGN KEY (`ProductTraceabilityUid`) REFERENCES `ProductTraceabilities` (`ProductTraceabilityUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_MeasurementTraceabilities_Tolerances_ToleranceUid` FOREIGN KEY (`ToleranceUid`) REFERENCES `Tolerances` (`ToleranceUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE UNIQUE INDEX `IX_DeviceIdentities_EquipSN` ON `DeviceIdentities` (`EquipSN`);


CREATE UNIQUE INDEX `IX_DeviceIdentities_MacAddressValue` ON `DeviceIdentities` (`MacAddressValue`);


CREATE INDEX `IX_Firmwares_Product` ON `Firmwares` (`Product`);


CREATE INDEX `IX_Firmwares_UserUid` ON `Firmwares` (`UserUid`);


CREATE INDEX `IX_Labels_LabelTypeUid` ON `Labels` (`LabelTypeUid`);


CREATE INDEX `IX_Labels_ManufacturingOrderUid` ON `Labels` (`ManufacturingOrderUid`);


CREATE INDEX `IX_Labels_ProductUid` ON `Labels` (`ProductUid`);


CREATE INDEX `IX_Labels_UserUid` ON `Labels` (`UserUid`);


CREATE INDEX `IX_LabelTemplates_LabelTypeUid` ON `LabelTemplates` (`LabelTypeUid`);


CREATE INDEX `IX_LabelTemplates_Product` ON `LabelTemplates` (`Product`);


CREATE INDEX `IX_MeasurementTraceabilities_ProductTraceabilityUid_Classificat~` ON `MeasurementTraceabilities` (`ProductTraceabilityUid`, `Classification`);


CREATE INDEX `IX_MeasurementTraceabilities_ProductTraceabilityUid_ToleranceUid` ON `MeasurementTraceabilities` (`ProductTraceabilityUid`, `ToleranceUid`);


CREATE INDEX `IX_MeasurementTraceabilities_ToleranceUid_Classification` ON `MeasurementTraceabilities` (`ToleranceUid`, `Classification`);


CREATE INDEX `IX_ProductTraceabilities_MachineUid` ON `ProductTraceabilities` (`MachineUid`);


CREATE INDEX `IX_ProductTraceabilities_ProductStatusUid` ON `ProductTraceabilities` (`ProductStatusUid`);


CREATE INDEX `IX_ProductTraceabilities_UserUid` ON `ProductTraceabilities` (`UserUid`);


CREATE INDEX `IX_Sequences_Product` ON `Sequences` (`Product`);


CREATE INDEX `IX_SolderingPrograms_ProductUid` ON `SolderingPrograms` (`ProductUid`);


CREATE INDEX `IX_SolderingPrograms_UserUid` ON `SolderingPrograms` (`UserUid`);


CREATE INDEX `IX_Tolerances_MachineUid` ON `Tolerances` (`MachineUid`);


CREATE INDEX `IX_Tolerances_ProductUid` ON `Tolerances` (`ProductUid`);


CREATE INDEX `IX_Tolerances_UnitUid` ON `Tolerances` (`UnitUid`);


CREATE INDEX `IX_Tolerances_UserUid` ON `Tolerances` (`UserUid`);



CREATE INDEX `IX_ProductTraceabilities_ProductUid_OperationNumber` ON `ProductTraceabilities` (`ProductUid`(255), `OperationNumber`);

CREATE TABLE `__efmigrationshistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___efmigrationshistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260130123058_AddManufacturingOrder', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202121125_AddOFNotifications', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202122344_ChangeProductUidToString', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202122705_ChangeFirstSerialNumberType', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202130529_AddNotificationModelChanges', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202144955_AddArticleTable', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202152026_AddBOM', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202155154_DateFormatBOM', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260202161648_ArticleVersions', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203081005_Firmwares', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203095341_Users', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203102124_Machines', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203120926_Operations', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203122636_ChangeTypeOperations', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203123214_ChangeTypeOperations2', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203123730_ChangeTypeOperations3', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203130546_Sequence', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203134212_MeasurementTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204080852_ProductTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204092151_LabelType', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204101919_Tolerance', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204104511_LabelTemplate', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204133303_SolderingProgram', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204140229_SimCard', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204142312_PackLocPhoto', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204145040_OrderLine', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204151014_Label', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204154905_DatabaseNotif', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205080057_KitTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205083117_KitAccTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205085636_FirmwareVersion', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205131912_AddFirmwareForeingKeyProduct', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205134138_AddFirmwareForeignKeyUser', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206083537_LabelTypeChangeLabel', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206084513_FKArticleLabel', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206085042_FKArticleLabelUserAndLabelType', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206085600_FKOfLabel', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206094535_FKLabelTemplate', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206095428_FKLabelTemplate2', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211084906_FKMeasurementTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211085247_UnitEnumMeasTra', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211090049_FKSequence', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211092225_FKSoldering', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211092642_FKTolerance', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211144238_UnitChange', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211145313_ModeChange', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260212081918_ProgChangeSoldering', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260212101050_NoDeleteCascade', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260310132041_SerialNumberKitTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260318100723_ManufacturingOrderMeasTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260318103900_MacAddresses', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260421141915_DevideIdentity', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260422055213_DevideIdentity2', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603075926_RemoveMeasTracea', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603102622_UnitAndDeleteMeas', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603133843_AddMeasurementTraceability', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603140700_AddProductTraceabilityLookupPrefixIndex', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260604110000_AddProductTraceabilityFailureComment', '9.0.0');

