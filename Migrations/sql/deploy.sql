START TRANSACTION;
ALTER TABLE `LabelTemplates` MODIFY COLUMN `LabelTypeUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

CREATE INDEX `IX_LabelTemplates_LabelTypeUid` ON `LabelTemplates` (`LabelTypeUid`);

ALTER TABLE `LabelTemplates` ADD CONSTRAINT `FK_LabelTemplates_LabelTypes_LabelTypeUid` FOREIGN KEY (`LabelTypeUid`) REFERENCES `LabelTypes` (`LabelTypeUid`) ON DELETE CASCADE;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206095428_FKLabelTemplate2', '9.0.0');

COMMIT;

