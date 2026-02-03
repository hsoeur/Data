START TRANSACTION;
CREATE TABLE `MeasurementTraceabilities` (
    `MeasurementTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SerialNumber` int NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Value` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Classification` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    `OperationNumber` int NOT NULL,
    `UnitUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `MachineUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_MeasurementTraceabilities` PRIMARY KEY (`MeasurementTraceabilityUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203134212_MeasurementTracea', '9.0.0');

COMMIT;

