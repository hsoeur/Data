START TRANSACTION;
CREATE TABLE `MacAddresses` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `MacAddressValue` char(17) CHARACTER SET utf8mb4 NOT NULL,
    `Status` enum('AVAILABLE','RESERVED','USED') CHARACTER SET utf8mb4 NOT NULL,
    `ReservedAt` datetime(6) NULL,
    `UsedAt` datetime(6) NULL,
    `CreatedAt` datetime(6) NOT NULL,
    CONSTRAINT `PK_MacAddresses` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260318103900_MacAddresses', '9.0.0');

COMMIT;

