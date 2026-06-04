START TRANSACTION;
ALTER TABLE `ProductTraceabilities` ADD `FailureComment` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260604110000_AddProductTraceabilityFailureComment', '9.0.0');

COMMIT;

