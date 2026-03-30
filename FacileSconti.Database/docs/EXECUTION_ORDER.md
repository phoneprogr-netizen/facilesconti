# Ordine esecuzione SQL (senza EF migrations)

1. `01_tables/001_create_schema.sql`
2. `01_tables/002_create_additional_tables.sql`
3. `01_tables/003_create_identity_tables.sql`
4. `02_constraints/001_foreign_keys.sql`
5. `03_indexes/001_indexes.sql`
6. `05_views/001_dashboard_views.sql`
7. `04_seed/001_seed_core.sql`
8. `04_seed/002_seed_demo_data.sql`

## Strategia evolutiva
- Ogni modifica DB va in `06_updates` con script incrementale.
- Aggiornare **contestualmente**: entity C#, Fluent API e script SQL.
- Non usare `Add-Migration` né `Update-Database`.
