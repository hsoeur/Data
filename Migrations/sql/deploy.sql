START TRANSACTION;
CREATE TABLE `Machines` (
    `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Machines` PRIMARY KEY (`MachineUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260203102124_Machines', '9.0.0');

COMMIT;

