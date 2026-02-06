START TRANSACTION;
CREATE INDEX `IX_Labels_ManufacturingOrderUid` ON `Labels` (`ManufacturingOrderUid`);

ALTER TABLE `Labels` ADD CONSTRAINT `FK_Labels_ManufacturingOrders_ManufacturingOrderUid` FOREIGN KEY (`ManufacturingOrderUid`) REFERENCES `ManufacturingOrders` (`ManufacturingOrderUid`) ON DELETE CASCADE;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260206085600_FKOfLabel', '9.0.0');

COMMIT;

