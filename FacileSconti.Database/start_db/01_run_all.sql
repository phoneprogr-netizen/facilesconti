/*
Eseguire in SQL Server Management Studio con "SQLCMD Mode" attivo.
Questo script richiama tutti gli script necessari in ordine.
*/

:r ..\01_tables\001_create_schema.sql
:r ..\01_tables\002_create_additional_tables.sql
:r ..\02_constraints\001_foreign_keys.sql
:r ..\03_indexes\001_indexes.sql
:r ..\05_views\001_dashboard_views.sql
:r ..\04_seed\001_seed_core.sql
