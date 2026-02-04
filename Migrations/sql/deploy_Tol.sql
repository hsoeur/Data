START TRANSACTION;
CREATE TABLE `Tolerances` (
    `ToleranceUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Label` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Val1` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Val2` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `OperationNumber` int NOT NULL,
    `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Mode` int NOT NULL,
    `Unit` int NOT NULL,
    CONSTRAINT `PK_Tolerances` PRIMARY KEY (`ToleranceUid`),
    CONSTRAINT `FK_Tolerances_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tolerances_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Tolerances_MachineUid` ON `Tolerances` (`MachineUid`);

CREATE INDEX `IX_Tolerances_UserUid` ON `Tolerances` (`UserUid`);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204101919_Tolerance', '9.0.0');

COMMIT;

