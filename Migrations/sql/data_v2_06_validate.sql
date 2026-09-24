-- Validate the Blue -> V2 copy. Read-only.

SET NAMES utf8mb4;

SELECT 'Articles' AS TableName,
       (SELECT COUNT(*) FROM db_production.Articles) AS BlueRows,
       (SELECT COUNT(*) FROM db_production_v2.Articles) AS V2Rows
UNION ALL SELECT 'BillOfMaterials',
       (SELECT COUNT(*) FROM db_production.BillOfMaterials),
       (SELECT COUNT(*) FROM db_production_v2.BillOfMaterials)
UNION ALL SELECT 'DatabaseNotifications',
       (SELECT COUNT(*) FROM db_production.DatabaseNotifications),
       (SELECT COUNT(*) FROM db_production_v2.DatabaseNotifications)
UNION ALL SELECT 'DeviceIdentities',
       (SELECT COUNT(*) FROM db_production.DeviceIdentities),
       (SELECT COUNT(*) FROM db_production_v2.DeviceIdentities)
UNION ALL SELECT 'FirmwareVersions',
       (SELECT COUNT(*) FROM db_production.FirmwareVersions),
       (SELECT COUNT(*) FROM db_production_v2.FirmwareVersions)
UNION ALL SELECT 'Firmwares',
       (SELECT COUNT(*) FROM db_production.Firmwares),
       (SELECT COUNT(*) FROM db_production_v2.Firmwares)
UNION ALL SELECT 'KitAccTraceabilities',
       (SELECT COUNT(*) FROM db_production.KitAccTraceabilities),
       (SELECT COUNT(*) FROM db_production_v2.KitAccTraceabilities)
UNION ALL SELECT 'KitTraceabilities',
       (SELECT COUNT(*) FROM db_production.KitTraceabilities),
       (SELECT COUNT(*) FROM db_production_v2.KitTraceabilities)
UNION ALL SELECT 'Labels',
       (SELECT COUNT(*) FROM db_production.Labels),
       (SELECT COUNT(*) FROM db_production_v2.Labels)
UNION ALL SELECT 'LabelTemplates',
       (SELECT COUNT(*) FROM db_production.LabelTemplates),
       (SELECT COUNT(*) FROM db_production_v2.LabelTemplates)
UNION ALL SELECT 'LabelTypes',
       (SELECT COUNT(*) FROM db_production.LabelTypes),
       (SELECT COUNT(*) FROM db_production_v2.LabelTypes)
UNION ALL SELECT 'MacAddresses',
       (SELECT COUNT(*) FROM db_production.MacAddresses),
       (SELECT COUNT(*) FROM db_production_v2.MacAddresses)
UNION ALL SELECT 'Machines',
       (SELECT COUNT(*) FROM db_production.Machines),
       (SELECT COUNT(*) FROM db_production_v2.Machines)
UNION ALL SELECT 'ManufacturingOrderNotifications',
       (SELECT COUNT(*) FROM db_production.ManufacturingOrderNotifications),
       (SELECT COUNT(*) FROM db_production_v2.ManufacturingOrderNotifications)
UNION ALL SELECT 'ManufacturingOrders',
       (SELECT COUNT(*) FROM db_production.ManufacturingOrders),
       (SELECT COUNT(*) FROM db_production_v2.ManufacturingOrders)
UNION ALL SELECT 'Operations',
       (SELECT COUNT(*) FROM db_production.Operations),
       (SELECT COUNT(*) FROM db_production_v2.Operations)
UNION ALL SELECT 'OrderLines',
       (SELECT COUNT(*) FROM db_production.OrderLines),
       (SELECT COUNT(*) FROM db_production_v2.OrderLines)
UNION ALL SELECT 'PackagingLocationPhotos',
       (SELECT COUNT(*) FROM db_production.PackagingLocationPhotos),
       (SELECT COUNT(*) FROM db_production_v2.PackagingLocationPhotos)
UNION ALL SELECT 'ProductStatuses',
       (SELECT COUNT(*) FROM db_production.ProductStatuses),
       (SELECT COUNT(*) FROM db_production_v2.ProductStatuses)
UNION ALL SELECT 'ProductTraceabilities',
       (SELECT COUNT(*) FROM db_production.ProductTraceabilities),
       (SELECT COUNT(*) FROM db_production_v2.ProductTraceabilities)
UNION ALL SELECT 'Sequences',
       (SELECT COUNT(*) FROM db_production.Sequences),
       (SELECT COUNT(*) FROM db_production_v2.Sequences)
UNION ALL SELECT 'SimCards',
       (SELECT COUNT(*) FROM db_production.SimCards),
       (SELECT COUNT(*) FROM db_production_v2.SimCards)
UNION ALL SELECT 'SolderingPrograms',
       (SELECT COUNT(*) FROM db_production.SolderingPrograms),
       (SELECT COUNT(*) FROM db_production_v2.SolderingPrograms)
UNION ALL SELECT 'Tolerances',
       (SELECT COUNT(*) FROM db_production.Tolerances),
       (SELECT COUNT(*) FROM db_production_v2.Tolerances)
UNION ALL SELECT 'Users',
       (SELECT COUNT(*) FROM db_production.Users),
       (SELECT COUNT(*) FROM db_production_v2.Users)
ORDER BY TableName;

-- Expected unit distribution for the current Blue snapshot:
-- %=2, °C=6, Adc=22, Char=76, Hz=11, Vac=12, Vdc=70.
SELECT UnitUid, COUNT(*) AS NumberOfTolerances
FROM db_production_v2.Tolerances
GROUP BY UnitUid
ORDER BY UnitUid;

SELECT COUNT(*) AS InvalidUnitReferences
FROM db_production_v2.Tolerances t
LEFT JOIN db_production_v2.Units u ON u.UnitUid = t.UnitUid
WHERE u.UnitUid IS NULL;

SELECT COUNT(*) AS InvalidModes
FROM db_production_v2.Tolerances
WHERE Mode NOT IN ('Between', 'Equal', 'Min', 'Max', 'Record', 'Entry');

SELECT COUNT(*) AS UnexpectedMeasurementRows
FROM db_production_v2.MeasurementTraceabilities;

SELECT COUNT(*) AS NonNullMigratedFailureComments
FROM db_production_v2.ProductTraceabilities
WHERE FailureComment IS NOT NULL;

SELECT COUNT(*) AS MigrationHistoryRows
FROM db_production_v2.__efmigrationshistory;

