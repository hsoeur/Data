START TRANSACTION;
DROP TABLE `ArticleVersions`;

CREATE TABLE `FirmwareVersions` (
    `FirmwareVersionUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ArticleUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Version` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_FirmwareVersions` PRIMARY KEY (`FirmwareVersionUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205085636_FirmwareVersion', '9.0.0');

COMMIT;

