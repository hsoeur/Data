START TRANSACTION;
ALTER TABLE `Firmwares` MODIFY COLUMN `UserUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL;


SET @exists := (SELECT COUNT(1) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Firmwares' AND INDEX_NAME = 'IX_Firmwares_UserUid');
SET @stmt := IF(@exists >0, 'DROP INDEX `IX_Firmwares_UserUid` ON `Firmwares`', 'SELECT 1');
PREPARE s FROM @stmt; EXECUTE s; DEALLOCATE PREPARE s;



DELETE FROM `Firmwares`
WHERE `UserUid` IS NULL
 OR `UserUid` NOT IN (SELECT `UserUid` FROM `Users`);


CREATE INDEX `IX_Firmwares_UserUid` ON `Firmwares` (`UserUid`);

ALTER TABLE `Firmwares` ADD CONSTRAINT `FK_Firmwares_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE CASCADE;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205134138_AddFirmwareForeignKeyUser', '9.0.0');

COMMIT;

