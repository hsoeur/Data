START TRANSACTION;
CREATE INDEX IX_ProductTraceabilities_ProductUid_OperationNumber ON ProductTraceabilities (ProductUid(255), OperationNumber);

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260603140700_AddProductTraceabilityLookupPrefixIndex', '9.0.0');

COMMIT;

