-- Seed the complete V2 unit reference list approved by the business.

SET NAMES utf8mb4;
START TRANSACTION;

INSERT INTO db_production_v2.Units (UnitUid, Label)
VALUES
    ('%',    'Pourcentage'),
    ('°C',   'Température en degrés Celsius'),
    ('°F',   'Température en degrés Fahrenheit'),
    ('Aac',  'Courant alternatif'),
    ('Adc',  'Courant continu'),
    ('Char', 'Chaîne de caractères'),
    ('Hz',   'Fréquence'),
    ('Ohms', 'Résistance'),
    ('s',    'Temps en secondes'),
    ('Vac',  'Tension alternative'),
    ('Vdc',  'Tension continue')
AS SourceUnits
ON DUPLICATE KEY UPDATE Label = SourceUnits.Label;

COMMIT;

SELECT UnitUid, Label
FROM db_production_v2.Units
ORDER BY UnitUid;

