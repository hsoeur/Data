START TRANSACTION;
CREATE TABLE `OrderLines` (
    `UidOrderLine` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Product` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Quantity` int NOT NULL,
    CONSTRAINT `PK_OrderLines` PRIMARY KEY (`UidOrderLine`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260204145040_OrderLine', '9.0.0');

COMMIT;

