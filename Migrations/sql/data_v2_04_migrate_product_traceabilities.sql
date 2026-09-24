-- Copy product traceabilities. FailureComment did not exist in Blue and starts as NULL.

SET NAMES utf8mb4;
START TRANSACTION;

INSERT INTO db_production_v2.ProductTraceabilities
    (ProductTraceabilityUid, ProductUid, ProductSerialNumber, ProductStatusUid,
     FailureComment, CreationDate, ManufacturingOrderUid, UserUid,
     CycleTime, OperationNumber, MachineUid)
SELECT
    s.ProductTraceabilityUid,
    s.ProductUid,
    s.ProductSerialNumber,
    s.ProductStatusUid,
    NULL,
    s.CreationDate,
    s.ManufacturingOrderUid,
    s.UserUid,
    s.CycleTime,
    s.OperationNumber,
    s.MachineUid
FROM db_production.ProductTraceabilities s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.ProductTraceabilities t
    WHERE t.ProductTraceabilityUid = s.ProductTraceabilityUid
);

SELECT ROW_COUNT() AS InsertedProductTraceabilities;
COMMIT;

