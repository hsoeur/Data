START TRANSACTION;
ALTER TABLE `SolderingPrograms` MODIFY COLUMN `Program` int NOT NULL;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260212081918_ProgChangeSoldering', '9.0.0');

COMMIT;

