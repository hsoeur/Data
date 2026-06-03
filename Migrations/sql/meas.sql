START TRANSACTION;
CREATE TABLE `MeasurementTraceabilities` (
    `MeasurementTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ProductTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ToleranceUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Value` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `Classification` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_MeasurementTraceabilities` PRIMARY KEY (`MeasurementTraceabilityUid`),
    CONSTRAINT `FK_MeasurementTraceabilities_ProductTraceabilities_ProductTrace~` FOREIGN KEY (`ProductTraceabilityUid`) REFERENCES `ProductTraceabilities` (`ProductTraceabilityUid`) ON DELETE RESTRICT,
    CONSTRAINT `FK_MeasurementTraceabilities_Tolerances_ToleranceUid` FOREIGN KEY (`ToleranceUid`) REFERENCES `Tolerances` (`ToleranceUid`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_MeasurementTraceabilities_ProductTraceabilityUid_Classificat~` ON `MeasurementTraceabilities` (`ProductTraceabilityUid`, `Classification`);

CREATE INDEX `IX_MeasurementTraceabilities_ProductTraceabilityUid_ToleranceUid` ON `MeasurementTraceabilities` (`ProductTraceabilityUid`, `ToleranceUid`);

CREATE INDEX `IX_MeasurementTraceabilities_ToleranceUid_Classification` ON `MeasurementTraceabilities` (`ToleranceUid`, `Classification`);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603133843_AddMeasurementTraceability', '9.0.0');

COMMIT;

