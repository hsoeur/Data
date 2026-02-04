START TRANSACTION;
CREATE TABLE `Labels` (
    `LabelUid` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `AuthentiUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UserUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ProductUid` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ManufacturingOrderUid` int NOT NULL,
    `CreationDate` datetime(6) NOT NULL,
    `CurrentSerialNumber` int NOT NULL,
    `ProductVersion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Usage` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Template` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Labels` PRIMARY KEY (`LabelUid`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204151014_Label', '9.0.0');

COMMIT;

