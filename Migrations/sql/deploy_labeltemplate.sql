START TRANSACTION;
CREATE TABLE `SolderingPrograms` (
    `SolderingProgramUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Program` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StartDate` datetime(6) NOT NULL,
    `EndDate` datetime(6) NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Side` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_SolderingPrograms` PRIMARY KEY (`SolderingProgramUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204133303_SolderingProgram', '9.0.0');

COMMIT;

