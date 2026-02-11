START TRANSACTION;
ALTER TABLE `MeasurementTraceabilities` MODIFY COLUMN `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `MeasurementTraceabilities` MODIFY COLUMN `ProductUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `MeasurementTraceabilities` MODIFY COLUMN `MachineUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

ALTER TABLE `MeasurementTraceabilities` ADD `Unit` int NULL;

CREATE INDEX `IX_MeasurementTraceabilities_MachineUid` ON `MeasurementTraceabilities` (`MachineUid`);

CREATE INDEX `IX_MeasurementTraceabilities_ProductUid` ON `MeasurementTraceabilities` (`ProductUid`);

CREATE INDEX `IX_MeasurementTraceabilities_UserUid` ON `MeasurementTraceabilities` (`UserUid`);

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE CASCADE;

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE CASCADE;

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE CASCADE;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211084906_FKMeasurementTracea', '9.0.0');

ALTER TABLE `MeasurementTraceabilities` DROP COLUMN `UnitUid`;

ALTER TABLE `MeasurementTraceabilities` MODIFY COLUMN `Unit` int NOT NULL DEFAULT 0;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211085247_UnitEnumMeasTra', '9.0.0');

COMMIT;

