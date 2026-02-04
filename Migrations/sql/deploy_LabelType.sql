START TRANSACTION;
CREATE TABLE `LabelTypes` (
    `UsageUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_LabelTypes` PRIMARY KEY (`UsageUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204092151_LabelType', '9.0.0');

COMMIT;

