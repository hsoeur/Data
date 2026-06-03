START TRANSACTION;
ALTER TABLE `Tolerances` DROP COLUMN `Unit`;

ALTER TABLE `Tolerances` ADD `UnitUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

CREATE TABLE `Units` (
    `UnitUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Units` PRIMARY KEY (`UnitUid`)
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Tolerances_UnitUid` ON `Tolerances` (`UnitUid`);

ALTER TABLE `Tolerances` ADD CONSTRAINT `FK_Tolerances_Units_UnitUid` FOREIGN KEY (`UnitUid`) REFERENCES `Units` (`UnitUid`) ON DELETE RESTRICT;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603102622_UnitAndDeleteMeas', '9.0.0');

COMMIT;

