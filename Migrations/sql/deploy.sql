START TRANSACTION;
CREATE TABLE `KitAccTraceabilities` (
    `KitAccTraceabilityUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ClientOrder` longtext CHARACTER SET utf8mb4 NOT NULL,
    `OrderLineUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SerialNumber` int NOT NULL,
    `ScanFrame` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Date` datetime(6) NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_KitAccTraceabilities` PRIMARY KEY (`KitAccTraceabilityUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260205083117_KitAccTracea', '9.0.0');

COMMIT;

