START TRANSACTION;
ALTER TABLE `MeasurementTraceabilities` ADD `ManufacturingOrderUid` int NULL;

CREATE INDEX `IX_MeasurementTraceabilities_ManufacturingOrderUid` ON `MeasurementTraceabilities` (`ManufacturingOrderUid`);

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_ManufacturingOrders_ManufacturingO~` FOREIGN KEY (`ManufacturingOrderUid`) REFERENCES `ManufacturingOrders` (`ManufacturingOrderUid`);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260318100723_ManufacturingOrderMeasTracea', '9.0.0');

COMMIT;

