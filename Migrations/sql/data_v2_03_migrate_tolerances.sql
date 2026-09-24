-- Migrate Blue tolerances to the normalized V2 Units model.
-- Approved mapping:
--   blank + Battery_Level_50                  -> %
--   other blank units                         -> Char
--   Ampere                                    -> Adc
--   Celsius / Degre Celsius                   -> °C
--   Hertz                                     -> Hz
--   Volt + Frequence_Sonnerie                 -> Hz
--   Volt + Amplitude_1000Hz/440Hz/Sonnerie    -> Vac
--   Vac                                       -> Vac
--   other Volt                                -> Vdc

SET NAMES utf8mb4;
START TRANSACTION;

INSERT INTO db_production_v2.Tolerances
    (ToleranceUid, ProductUid, Label, Val1, Val2, StartDate, EndDate,
     OperationNumber, UserUid, MachineUid, Mode, UnitUid)
SELECT
    s.ToleranceUid,
    s.ProductUid,
    s.Label,
    s.Val1,
    s.Val2,
    s.StartDate,
    s.EndDate,
    s.OperationNumber,
    s.UserUid,
    s.MachineUid,
    s.Mode,
    CASE
        WHEN COALESCE(TRIM(s.Unit), '') = '' AND s.Label = 'Battery_Level_50' THEN '%'
        WHEN COALESCE(TRIM(s.Unit), '') = '' THEN 'Char'
        WHEN s.Unit = 'Ampere' THEN 'Adc'
        WHEN s.Unit IN ('Celsius', 'Degre Celsius') THEN '°C'
        WHEN s.Unit = 'Hertz' THEN 'Hz'
        WHEN s.Unit = 'Vac' THEN 'Vac'
        WHEN s.Unit = 'Volt' AND s.Label = 'Frequence_Sonnerie' THEN 'Hz'
        WHEN s.Unit = 'Volt' AND s.Label IN (
            'Amplitude_1000Hz',
            'Amplitude_440Hz',
            'Amplitude_Sonnerie'
        ) THEN 'Vac'
        WHEN s.Unit = 'Volt' THEN 'Vdc'
        ELSE '__UNMAPPED__'
    END AS UnitUid
FROM db_production.Tolerances s
WHERE NOT EXISTS (
    SELECT 1 FROM db_production_v2.Tolerances t WHERE t.ToleranceUid = s.ToleranceUid
);

SELECT ROW_COUNT() AS InsertedTolerances;
COMMIT;

SELECT UnitUid, COUNT(*) AS NumberOfTolerances
FROM db_production_v2.Tolerances
GROUP BY UnitUid
ORDER BY UnitUid;

