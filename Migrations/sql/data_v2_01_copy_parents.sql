-- Copy the V2 parent/reference tables without transforming their data.
-- Re-runnable: existing primary keys are skipped.

SET NAMES utf8mb4;
START TRANSACTION;

INSERT INTO db_production_v2.Articles
    (ArticleUid, Designation, Store, Family, Packaging, ManageStock, SupplierReference, GpaoQuantity)
SELECT s.ArticleUid, s.Designation, s.Store, s.Family, s.Packaging, s.ManageStock, s.SupplierReference, s.GpaoQuantity
FROM db_production.Articles s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Articles t WHERE t.ArticleUid = s.ArticleUid
);
SELECT ROW_COUNT() AS InsertedArticles;

INSERT INTO db_production_v2.LabelTypes (LabelTypeUid)
SELECT s.LabelTypeUid
FROM db_production.LabelTypes s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.LabelTypes t WHERE t.LabelTypeUid = s.LabelTypeUid
);
SELECT ROW_COUNT() AS InsertedLabelTypes;

INSERT INTO db_production_v2.Machines (MachineUid, Label)
SELECT s.MachineUid, s.Label
FROM db_production.Machines s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Machines t WHERE t.MachineUid = s.MachineUid
);
SELECT ROW_COUNT() AS InsertedMachines;

INSERT INTO db_production_v2.ManufacturingOrders
    (ManufacturingOrderUid, ProductUid, Quantity, FirstSerialNumber, InternalOrderNumber, ExternalOrderNumber, Status)
SELECT s.ManufacturingOrderUid, s.ProductUid, s.Quantity, s.FirstSerialNumber,
       s.InternalOrderNumber, s.ExternalOrderNumber, s.Status
FROM db_production.ManufacturingOrders s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.ManufacturingOrders t
    WHERE t.ManufacturingOrderUid = s.ManufacturingOrderUid
);
SELECT ROW_COUNT() AS InsertedManufacturingOrders;

INSERT INTO db_production_v2.ProductStatuses (ProductStatusUid)
SELECT s.ProductStatusUid
FROM db_production.ProductStatuses s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.ProductStatuses t WHERE t.ProductStatusUid = s.ProductStatusUid
);
SELECT ROW_COUNT() AS InsertedProductStatuses;

INSERT INTO db_production_v2.Users
    (UserUid, LastName, FirstName, EntryDate, ExitDate, Department)
SELECT s.UserUid, s.LastName, s.FirstName, s.EntryDate, s.ExitDate, s.Department
FROM db_production.Users s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Users t WHERE t.UserUid = s.UserUid
);
SELECT ROW_COUNT() AS InsertedUsers;

COMMIT;

