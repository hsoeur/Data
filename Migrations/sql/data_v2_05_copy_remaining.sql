-- Copy every remaining table whose schema is unchanged.
-- Parent tables must already have been loaded by data_v2_01_copy_parents.sql.

SET NAMES utf8mb4;
START TRANSACTION;

INSERT INTO db_production_v2.BillOfMaterials
    (BillOfMaterialsUid, Product, Component, Operation, Quantity, QuantityDiv, CreationDate, EndDate)
SELECT s.BillOfMaterialsUid, s.Product, s.Component, s.Operation,
       s.Quantity, s.QuantityDiv, s.CreationDate, s.EndDate
FROM db_production.BillOfMaterials s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.BillOfMaterials t
    WHERE t.BillOfMaterialsUid = s.BillOfMaterialsUid
);
SELECT ROW_COUNT() AS InsertedBillOfMaterials;

INSERT INTO db_production_v2.DatabaseNotifications
    (DatabaseNotificationUid, EntityType, EntityUId, PropertyName, OldValue,
     NewValue, ModifiedAt, ModifiedBy, Processed, ProcessedDate, ProcessedBy, Comment)
SELECT s.DatabaseNotificationUid, s.EntityType, s.EntityUId, s.PropertyName,
       s.OldValue, s.NewValue, s.ModifiedAt, s.ModifiedBy, s.Processed,
       s.ProcessedDate, s.ProcessedBy, s.Comment
FROM db_production.DatabaseNotifications s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.DatabaseNotifications t
    WHERE t.DatabaseNotificationUid = s.DatabaseNotificationUid
);
SELECT ROW_COUNT() AS InsertedDatabaseNotifications;

INSERT INTO db_production_v2.DeviceIdentities
    (Id, EquipSN, MacAddressValue, CreationDate)
SELECT s.Id, s.EquipSN, s.MacAddressValue, s.CreationDate
FROM db_production.DeviceIdentities s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.DeviceIdentities t WHERE t.Id = s.Id
);
SELECT ROW_COUNT() AS InsertedDeviceIdentities;

INSERT INTO db_production_v2.FirmwareVersions
    (FirmwareVersionUid, ArticleUid, Version)
SELECT s.FirmwareVersionUid, s.ArticleUid, s.Version
FROM db_production.FirmwareVersions s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.FirmwareVersions t
    WHERE t.FirmwareVersionUid = s.FirmwareVersionUid
);
SELECT ROW_COUNT() AS InsertedFirmwareVersions;

INSERT INTO db_production_v2.KitAccTraceabilities
    (KitAccTraceabilityUid, ClientOrder, OrderLineUid, Product,
     SerialNumber, ScanFrame, Date, UserUid)
SELECT s.KitAccTraceabilityUid, s.ClientOrder, s.OrderLineUid, s.Product,
       s.SerialNumber, s.ScanFrame, s.Date, s.UserUid
FROM db_production.KitAccTraceabilities s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.KitAccTraceabilities t
    WHERE t.KitAccTraceabilityUid = s.KitAccTraceabilityUid
);
SELECT ROW_COUNT() AS InsertedKitAccTraceabilities;

INSERT INTO db_production_v2.KitTraceabilities
    (KitTraceabilityUid, ClientOrder, ManufacturingOrderUid, Product,
     HorizonComponentCode, SerialNumber, ScanFrame, Date, UserUid)
SELECT s.KitTraceabilityUid, s.ClientOrder, s.ManufacturingOrderUid, s.Product,
       s.HorizonComponentCode, s.SerialNumber, s.ScanFrame, s.Date, s.UserUid
FROM db_production.KitTraceabilities s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.KitTraceabilities t
    WHERE t.KitTraceabilityUid = s.KitTraceabilityUid
);
SELECT ROW_COUNT() AS InsertedKitTraceabilities;

INSERT INTO db_production_v2.MacAddresses
    (Id, MacAddressValue, Status, ReservedAt, UsedAt, CreatedAt)
SELECT s.Id, s.MacAddressValue, s.Status, s.ReservedAt, s.UsedAt, s.CreatedAt
FROM db_production.MacAddresses s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.MacAddresses t WHERE t.Id = s.Id
);
SELECT ROW_COUNT() AS InsertedMacAddresses;

INSERT INTO db_production_v2.ManufacturingOrderNotifications
    (NotificationUid, OfNumber, ChangeType, OldValue, NewValue,
     ProcessedBy, CreatedDate, Processed, ProcessedDate)
SELECT s.NotificationUid, s.OfNumber, s.ChangeType, s.OldValue, s.NewValue,
       s.ProcessedBy, s.CreatedDate, s.Processed, s.ProcessedDate
FROM db_production.ManufacturingOrderNotifications s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.ManufacturingOrderNotifications t
    WHERE t.NotificationUid = s.NotificationUid
);
SELECT ROW_COUNT() AS InsertedManufacturingOrderNotifications;

INSERT INTO db_production_v2.Operations
    (OperationUid, ProductUid, OperationNumber, Label, Machine,
     PrepTime, ProdTime, CreationDate)
SELECT s.OperationUid, s.ProductUid, s.OperationNumber, s.Label, s.Machine,
       s.PrepTime, s.ProdTime, s.CreationDate
FROM db_production.Operations s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Operations t WHERE t.OperationUid = s.OperationUid
);
SELECT ROW_COUNT() AS InsertedOperations;

INSERT INTO db_production_v2.OrderLines (UidOrderLine, Product, Quantity)
SELECT s.UidOrderLine, s.Product, s.Quantity
FROM db_production.OrderLines s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.OrderLines t WHERE t.UidOrderLine = s.UidOrderLine
);
SELECT ROW_COUNT() AS InsertedOrderLines;

INSERT INTO db_production_v2.PackagingLocationPhotos
    (PackagingLocationPhotoUid, ArticleUid, Photo, UseCase)
SELECT s.PackagingLocationPhotoUid, s.ArticleUid, s.Photo, s.UseCase
FROM db_production.PackagingLocationPhotos s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.PackagingLocationPhotos t
    WHERE t.PackagingLocationPhotoUid = s.PackagingLocationPhotoUid
);
SELECT ROW_COUNT() AS InsertedPackagingLocationPhotos;

INSERT INTO db_production_v2.SimCards
    (SimCardUid, PhoneNumber, Provider, SimCardNumber)
SELECT s.SimCardUid, s.PhoneNumber, s.Provider, s.SimCardNumber
FROM db_production.SimCards s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.SimCards t WHERE t.SimCardUid = s.SimCardUid
);
SELECT ROW_COUNT() AS InsertedSimCards;

-- Tables with V2 foreign keys are copied after their parents.
INSERT INTO db_production_v2.Sequences
    (SequenceUid, Product, OperationNumber, File, StartDate, EndDate)
SELECT s.SequenceUid, s.Product, s.OperationNumber, s.File, s.StartDate, s.EndDate
FROM db_production.Sequences s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Sequences t WHERE t.SequenceUid = s.SequenceUid
);
SELECT ROW_COUNT() AS InsertedSequences;

INSERT INTO db_production_v2.LabelTemplates
    (LabelTemplateUid, Product, LabelTypeUid, TemplateFile, Photo,
     StartDate, EndDate, UserUid)
SELECT s.LabelTemplateUid, s.Product, s.LabelTypeUid, s.TemplateFile, s.Photo,
       s.StartDate, s.EndDate, s.UserUid
FROM db_production.LabelTemplates s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.LabelTemplates t
    WHERE t.LabelTemplateUid = s.LabelTemplateUid
);
SELECT ROW_COUNT() AS InsertedLabelTemplates;

INSERT INTO db_production_v2.Firmwares
    (FirmwareUid, Product, Label, File, Version, UserUid, StartDate, EndDate, IsSuspended)
SELECT s.FirmwareUid, s.Product, s.Label, s.File, s.Version, s.UserUid,
       s.StartDate, s.EndDate, s.IsSuspended
FROM db_production.Firmwares s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Firmwares t WHERE t.FirmwareUid = s.FirmwareUid
);
SELECT ROW_COUNT() AS InsertedFirmwares;

INSERT INTO db_production_v2.Labels
    (LabelUid, AuthentiUid, UserUid, ProductUid, ManufacturingOrderUid,
     CreationDate, CurrentSerialNumber, ProductVersion, LabelTypeUid, Template)
SELECT s.LabelUid, s.AuthentiUid, s.UserUid, s.ProductUid, s.ManufacturingOrderUid,
       s.CreationDate, s.CurrentSerialNumber, s.ProductVersion, s.LabelTypeUid, s.Template
FROM db_production.Labels s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Labels t WHERE t.LabelUid = s.LabelUid
);
SELECT ROW_COUNT() AS InsertedLabels;

INSERT INTO db_production_v2.SolderingPrograms
    (SolderingProgramUid, ProductUid, Program, StartDate, EndDate, UserUid, Side)
SELECT s.SolderingProgramUid, s.ProductUid, s.Program, s.StartDate,
       s.EndDate, s.UserUid, s.Side
FROM db_production.SolderingPrograms s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.SolderingPrograms t
    WHERE t.SolderingProgramUid = s.SolderingProgramUid
);
SELECT ROW_COUNT() AS InsertedSolderingPrograms;

COMMIT;

