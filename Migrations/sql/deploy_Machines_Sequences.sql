START TRANSACTION;
CREATE TABLE `Operations` (
    `OperationUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OperationNumber` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Machine` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PrepTime` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProdTime` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreationDate` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Operations` PRIMARY KEY (`OperationUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203120926_Operations', '9.0.0');

ALTER TABLE `Operations` MODIFY COLUMN `ProdTime` decimal(10,2) NOT NULL;

ALTER TABLE `Operations` MODIFY COLUMN `PrepTime` decimal(10,2) NOT NULL;

ALTER TABLE `Operations` MODIFY COLUMN `OperationNumber` int NOT NULL;

ALTER TABLE `Operations` MODIFY COLUMN `CreationDate` datetime(6) NOT NULL;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203122636_ChangeTypeOperations', '9.0.0');

ALTER TABLE `Operations` MODIFY COLUMN `ProdTime` decimal(10,2) NOT NULL;

ALTER TABLE `Operations` MODIFY COLUMN `PrepTime` decimal(10,2) NOT NULL;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203123214_ChangeTypeOperations2', '9.0.0');

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203123730_ChangeTypeOperations3', '9.0.0');

CREATE TABLE `Sequences` (
    `SequenceUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OperationNumber` int NOT NULL,
    `File` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    CONSTRAINT `PK_Sequences` PRIMARY KEY (`SequenceUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203130546_Sequence', '9.0.0');

COMMIT;

