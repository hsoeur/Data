START TRANSACTION;
CREATE TABLE `DatabaseNotifications` (
    `DatabaseNotificationUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `EntityType` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EntityUId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PropertyName` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OldValue` longtext CHARACTER SET utf8mb4 NOT NULL,
    `NewValue` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ModifiedAt` datetime(6) NOT NULL,
    `ModifiedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Processed` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProcessedDate` datetime(6) NOT NULL,
    `ProcessedBy` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Comment` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_DatabaseNotifications` PRIMARY KEY (`DatabaseNotificationUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204154905_DatabaseNotif', '9.0.0');

COMMIT;

