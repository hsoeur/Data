START TRANSACTION;
CREATE TABLE `PackagingLocationPhotos` (
    `PackagingLocationPhotoUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ArticleUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Photo` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UseCase` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_PackagingLocationPhotos` PRIMARY KEY (`PackagingLocationPhotoUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204142312_PackLocPhoto', '9.0.0');

COMMIT;

