START TRANSACTION;
CREATE TABLE `KitTraceabilities` (
    `KitTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClientOrder` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ManufacturingOrderUid` int NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `HorizonComponentCode` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ScanFrame` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Date` datetime(6) NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_KitTraceabilities` PRIMARY KEY (`KitTraceabilityUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205080057_KitTracea', '9.0.0');

COMMIT;

