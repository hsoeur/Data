START TRANSACTION;
DROP TABLE `MeasurementTraceabilities`;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603075926_RemoveMeasTracea', '9.0.0');

COMMIT;

