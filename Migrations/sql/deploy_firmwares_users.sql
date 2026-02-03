START TRANSACTION;
CREATE TABLE `Users` (
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `LastName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FirstName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EntryDate` datetime(6) NOT NULL,
    `ExitDate` datetime(6) NOT NULL,
    `Department` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`UserUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203095341_Users', '9.0.0');

COMMIT;

