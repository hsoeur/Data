START TRANSACTION;
CREATE TABLE `ProductStatuses` (
    `ProductStatusUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ProductStatuses` PRIMARY KEY (`ProductStatusUid`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProductTraceabilities` (
    `ProductTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProductSerialNumber` int NOT NULL,
    `ProductStatusUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    `ManufacturingOrderUid` int NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CycleTime` decimal(10,2) NOT NULL,
    `OperationNumber` int NOT NULL,
    `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ProductTraceabilities` PRIMARY KEY (`ProductTraceabilityUid`),
    CONSTRAINT `FK_ProductTraceabilities_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE CASCADE,
    CONSTRAINT `FK_ProductTraceabilities_ProductStatuses_ProductStatusUid` FOREIGN KEY (`ProductStatusUid`) REFERENCES `ProductStatuses` (`ProductStatusUid`) ON DELETE CASCADE,
    CONSTRAINT `FK_ProductTraceabilities_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ProductTraceabilities_MachineUid` ON `ProductTraceabilities` (`MachineUid`);

CREATE INDEX `IX_ProductTraceabilities_ProductStatusUid` ON `ProductTraceabilities` (`ProductStatusUid`);

CREATE INDEX `IX_ProductTraceabilities_UserUid` ON `ProductTraceabilities` (`UserUid`);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204080852_ProductTracea', '9.0.0');

COMMIT;

