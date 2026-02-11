START TRANSACTION;
ALTER TABLE `Tolerances` MODIFY COLUMN `ProductUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

CREATE INDEX `IX_Tolerances_ProductUid` ON `Tolerances` (`ProductUid`);

ALTER TABLE `Tolerances` ADD CONSTRAINT `FK_Tolerances_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE CASCADE;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211092642_FKTolerance', '9.0.0');

COMMIT;

