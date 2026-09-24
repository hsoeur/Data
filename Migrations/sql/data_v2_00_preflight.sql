-- Preflight for the Blue -> V2 data migration.
-- Read-only: this script changes no data.

SET NAMES utf8mb4;

SELECT 'Source database' AS CheckName,
       COUNT(*) AS Result
FROM information_schema.SCHEMATA
WHERE SCHEMA_NAME = 'db_production';

SELECT 'Target database' AS CheckName,
       COUNT(*) AS Result
FROM information_schema.SCHEMATA
WHERE SCHEMA_NAME = 'db_production_v2';

SELECT 'Target EF migrations' AS CheckName,
       COUNT(*) AS Result
FROM db_production_v2.__efmigrationshistory;

SELECT 'Target business rows before migration' AS CheckName,
       SUM(TableRows) AS Result
FROM (
    SELECT COUNT(*) AS TableRows FROM db_production_v2.Articles
    UNION ALL SELECT COUNT(*) FROM db_production_v2.Users
    UNION ALL SELECT COUNT(*) FROM db_production_v2.Machines
    UNION ALL SELECT COUNT(*) FROM db_production_v2.ManufacturingOrders
    UNION ALL SELECT COUNT(*) FROM db_production_v2.ProductStatuses
    UNION ALL SELECT COUNT(*) FROM db_production_v2.LabelTypes
    UNION ALL SELECT COUNT(*) FROM db_production_v2.Units
    UNION ALL SELECT COUNT(*) FROM db_production_v2.Tolerances
    UNION ALL SELECT COUNT(*) FROM db_production_v2.ProductTraceabilities
    UNION ALL SELECT COUNT(*) FROM db_production_v2.MeasurementTraceabilities
) AS TargetRows;

-- Every row must be mapped by data_v2_03_migrate_tolerances.sql.
SELECT Unit, Label, Mode, COUNT(*) AS UnmappedRows
FROM db_production.Tolerances
WHERE NOT (
       (COALESCE(TRIM(Unit), '') = '')
    OR Unit = 'Ampere'
    OR Unit IN ('Celsius', 'Degre Celsius')
    OR Unit = 'Hertz'
    OR Unit = 'Vac'
    OR Unit = 'Volt'
)
GROUP BY Unit, Label, Mode;

-- The result must be empty.
SELECT Mode, COUNT(*) AS InvalidModeRows
FROM db_production.Tolerances
WHERE Mode NOT IN ('Between', 'Equal', 'Min', 'Max', 'Record', 'Entry')
GROUP BY Mode;

-- Orphans that would violate the V2 foreign keys. Every result must be zero.
SELECT 'Tolerances -> Articles' AS RelationName, COUNT(*) AS OrphanRows
FROM db_production.Tolerances t
LEFT JOIN db_production.Articles a ON a.ArticleUid = t.ProductUid
WHERE a.ArticleUid IS NULL
UNION ALL
SELECT 'Tolerances -> Machines', COUNT(*)
FROM db_production.Tolerances t
LEFT JOIN db_production.Machines m ON m.MachineUid = t.MachineUid
WHERE m.MachineUid IS NULL
UNION ALL
SELECT 'Tolerances -> Users', COUNT(*)
FROM db_production.Tolerances t
LEFT JOIN db_production.Users u ON u.UserUid = t.UserUid
WHERE u.UserUid IS NULL
UNION ALL
SELECT 'ProductTraceabilities -> Machines', COUNT(*)
FROM db_production.ProductTraceabilities p
LEFT JOIN db_production.Machines m ON m.MachineUid = p.MachineUid
WHERE m.MachineUid IS NULL
UNION ALL
SELECT 'ProductTraceabilities -> ProductStatuses', COUNT(*)
FROM db_production.ProductTraceabilities p
LEFT JOIN db_production.ProductStatuses s ON s.ProductStatusUid = p.ProductStatusUid
WHERE s.ProductStatusUid IS NULL
UNION ALL
SELECT 'ProductTraceabilities -> Users', COUNT(*)
FROM db_production.ProductTraceabilities p
LEFT JOIN db_production.Users u ON u.UserUid = p.UserUid
WHERE u.UserUid IS NULL
UNION ALL
SELECT 'Sequences -> Articles', COUNT(*)
FROM db_production.Sequences s
LEFT JOIN db_production.Articles a ON a.ArticleUid = s.Product
WHERE a.ArticleUid IS NULL
UNION ALL
SELECT 'LabelTemplates -> Articles', COUNT(*)
FROM db_production.LabelTemplates l
LEFT JOIN db_production.Articles a ON a.ArticleUid = l.Product
WHERE a.ArticleUid IS NULL
UNION ALL
SELECT 'LabelTemplates -> LabelTypes', COUNT(*)
FROM db_production.LabelTemplates l
LEFT JOIN db_production.LabelTypes t ON t.LabelTypeUid = l.LabelTypeUid
WHERE t.LabelTypeUid IS NULL
UNION ALL
SELECT 'Firmwares -> Articles', COUNT(*)
FROM db_production.Firmwares f
LEFT JOIN db_production.Articles a ON a.ArticleUid = f.Product
WHERE a.ArticleUid IS NULL
UNION ALL
SELECT 'Firmwares -> Users', COUNT(*)
FROM db_production.Firmwares f
LEFT JOIN db_production.Users u ON u.UserUid = f.UserUid
WHERE u.UserUid IS NULL
UNION ALL
SELECT 'Labels -> Articles', COUNT(*)
FROM db_production.Labels l
LEFT JOIN db_production.Articles a ON a.ArticleUid = l.ProductUid
WHERE a.ArticleUid IS NULL
UNION ALL
SELECT 'Labels -> LabelTypes', COUNT(*)
FROM db_production.Labels l
LEFT JOIN db_production.LabelTypes t ON t.LabelTypeUid = l.LabelTypeUid
WHERE t.LabelTypeUid IS NULL
UNION ALL
SELECT 'Labels -> ManufacturingOrders', COUNT(*)
FROM db_production.Labels l
LEFT JOIN db_production.ManufacturingOrders m ON m.ManufacturingOrderUid = l.ManufacturingOrderUid
WHERE m.ManufacturingOrderUid IS NULL
UNION ALL
SELECT 'Labels -> Users', COUNT(*)
FROM db_production.Labels l
LEFT JOIN db_production.Users u ON u.UserUid = l.UserUid
WHERE u.UserUid IS NULL
UNION ALL
SELECT 'SolderingPrograms -> Articles', COUNT(*)
FROM db_production.SolderingPrograms s
LEFT JOIN db_production.Articles a ON a.ArticleUid = s.ProductUid
WHERE a.ArticleUid IS NULL
UNION ALL
SELECT 'SolderingPrograms -> Users', COUNT(*)
FROM db_production.SolderingPrograms s
LEFT JOIN db_production.Users u ON u.UserUid = s.UserUid
WHERE u.UserUid IS NULL;

