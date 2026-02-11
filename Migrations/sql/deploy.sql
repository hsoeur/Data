START TRANSACTION;
ALTER TABLE `Sequences` MODIFY COLUMN `Product` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

CREATE INDEX `IX_Sequences_Product` ON `Sequences` (`Product`);

ALTER TABLE `Sequences` ADD CONSTRAINT `FK_Sequences_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE CASCADE;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211090049_FKSequence', '9.0.0');

COMMIT;

