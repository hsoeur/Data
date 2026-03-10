START TRANSACTION;
ALTER TABLE `KitTraceabilities` ADD `SerialNumber` int NOT NULL DEFAULT 0;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260310132041_SerialNumberKitTracea', '9.0.0');

COMMIT;

