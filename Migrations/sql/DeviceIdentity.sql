START TRANSACTION;
CREATE TABLE `DeviceIdentities` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `EquipSN` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `MacAddressValue` char(17) CHARACTER SET utf8mb4 NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NULL,
    CONSTRAINT `PK_DeviceIdentities` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE UNIQUE INDEX `IX_DeviceIdentities_EquipSN` ON `DeviceIdentities` (`EquipSN`);

CREATE UNIQUE INDEX `IX_DeviceIdentities_MacAddressValue` ON `DeviceIdentities` (`MacAddressValue`);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260421141915_DevideIdentity', '9.0.0');

ALTER TABLE `DeviceIdentities` DROP COLUMN `UpdatedAt`;

ALTER TABLE `DeviceIdentities` RENAME COLUMN `CreatedAt` TO `CreationDate`;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260422055213_DevideIdentity2', '9.0.0');

COMMIT;

