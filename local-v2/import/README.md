# Local V2 database import

Place the production V2 logical dump in this directory with this name:

```text
01_db_production_v2.sql
```

The MySQL image imports files from this directory only when the named Docker
volume is empty. Replacing the dump does not update an already initialized
volume.

Production dumps can contain personal or confidential data. Keep them outside
Git, transfer them securely, and remove them when they are no longer needed.
