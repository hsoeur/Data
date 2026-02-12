START TRANSACTION;
ALTER TABLE `Firmwares` DROP FOREIGN KEY `FK_Firmwares_Articles_Product`;

ALTER TABLE `Firmwares` DROP FOREIGN KEY `FK_Firmwares_Users_UserUid`;

ALTER TABLE `Labels` DROP FOREIGN KEY `FK_Labels_Articles_ProductUid`;

ALTER TABLE `Labels` DROP FOREIGN KEY `FK_Labels_LabelTypes_LabelTypeUid`;

ALTER TABLE `Labels` DROP FOREIGN KEY `FK_Labels_ManufacturingOrders_ManufacturingOrderUid`;

ALTER TABLE `Labels` DROP FOREIGN KEY `FK_Labels_Users_UserUid`;

ALTER TABLE `LabelTemplates` DROP FOREIGN KEY `FK_LabelTemplates_Articles_Product`;

ALTER TABLE `LabelTemplates` DROP FOREIGN KEY `FK_LabelTemplates_LabelTypes_LabelTypeUid`;

ALTER TABLE `MeasurementTraceabilities` DROP FOREIGN KEY `FK_MeasurementTraceabilities_Articles_ProductUid`;

ALTER TABLE `MeasurementTraceabilities` DROP FOREIGN KEY `FK_MeasurementTraceabilities_Machines_MachineUid`;

ALTER TABLE `MeasurementTraceabilities` DROP FOREIGN KEY `FK_MeasurementTraceabilities_Users_UserUid`;

ALTER TABLE `ProductTraceabilities` DROP FOREIGN KEY `FK_ProductTraceabilities_Machines_MachineUid`;

ALTER TABLE `ProductTraceabilities` DROP FOREIGN KEY `FK_ProductTraceabilities_ProductStatuses_ProductStatusUid`;

ALTER TABLE `ProductTraceabilities` DROP FOREIGN KEY `FK_ProductTraceabilities_Users_UserUid`;

ALTER TABLE `Sequences` DROP FOREIGN KEY `FK_Sequences_Articles_Product`;

ALTER TABLE `SolderingPrograms` DROP FOREIGN KEY `FK_SolderingPrograms_Articles_ProductUid`;

ALTER TABLE `SolderingPrograms` DROP FOREIGN KEY `FK_SolderingPrograms_Users_UserUid`;

ALTER TABLE `Tolerances` DROP FOREIGN KEY `FK_Tolerances_Articles_ProductUid`;

ALTER TABLE `Tolerances` DROP FOREIGN KEY `FK_Tolerances_Machines_MachineUid`;

ALTER TABLE `Tolerances` DROP FOREIGN KEY `FK_Tolerances_Users_UserUid`;

ALTER TABLE `Firmwares` ADD CONSTRAINT `FK_Firmwares_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `Firmwares` ADD CONSTRAINT `FK_Firmwares_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT;

ALTER TABLE `Labels` ADD CONSTRAINT `FK_Labels_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `Labels` ADD CONSTRAINT `FK_Labels_LabelTypes_LabelTypeUid` FOREIGN KEY (`LabelTypeUid`) REFERENCES `LabelTypes` (`LabelTypeUid`) ON DELETE RESTRICT;

ALTER TABLE `Labels` ADD CONSTRAINT `FK_Labels_ManufacturingOrders_ManufacturingOrderUid` FOREIGN KEY (`ManufacturingOrderUid`) REFERENCES `ManufacturingOrders` (`ManufacturingOrderUid`) ON DELETE RESTRICT;

ALTER TABLE `Labels` ADD CONSTRAINT `FK_Labels_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT;

ALTER TABLE `LabelTemplates` ADD CONSTRAINT `FK_LabelTemplates_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `LabelTemplates` ADD CONSTRAINT `FK_LabelTemplates_LabelTypes_LabelTypeUid` FOREIGN KEY (`LabelTypeUid`) REFERENCES `LabelTypes` (`LabelTypeUid`) ON DELETE RESTRICT;

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE RESTRICT;

ALTER TABLE `MeasurementTraceabilities` ADD CONSTRAINT `FK_MeasurementTraceabilities_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT;

ALTER TABLE `ProductTraceabilities` ADD CONSTRAINT `FK_ProductTraceabilities_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE RESTRICT;

ALTER TABLE `ProductTraceabilities` ADD CONSTRAINT `FK_ProductTraceabilities_ProductStatuses_ProductStatusUid` FOREIGN KEY (`ProductStatusUid`) REFERENCES `ProductStatuses` (`ProductStatusUid`) ON DELETE RESTRICT;

ALTER TABLE `ProductTraceabilities` ADD CONSTRAINT `FK_ProductTraceabilities_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT;

ALTER TABLE `Sequences` ADD CONSTRAINT `FK_Sequences_Articles_Product` FOREIGN KEY (`Product`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `SolderingPrograms` ADD CONSTRAINT `FK_SolderingPrograms_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `SolderingPrograms` ADD CONSTRAINT `FK_SolderingPrograms_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT;

ALTER TABLE `Tolerances` ADD CONSTRAINT `FK_Tolerances_Articles_ProductUid` FOREIGN KEY (`ProductUid`) REFERENCES `Articles` (`ArticleUid`) ON DELETE RESTRICT;

ALTER TABLE `Tolerances` ADD CONSTRAINT `FK_Tolerances_Machines_MachineUid` FOREIGN KEY (`MachineUid`) REFERENCES `Machines` (`MachineUid`) ON DELETE RESTRICT;

ALTER TABLE `Tolerances` ADD CONSTRAINT `FK_Tolerances_Users_UserUid` FOREIGN KEY (`UserUid`) REFERENCES `Users` (`UserUid`) ON DELETE RESTRICT;

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260212101050_NoDeleteCascade', '9.0.0');

COMMIT;

