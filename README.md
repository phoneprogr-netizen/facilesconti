# FacileSconti

Base enterprise ASP.NET Core MVC per portale coupon multi-ruolo (Admin, Customer, EndUser), con SQL Server e script DB manuali.

## 1) Analisi architetturale
- **Architettura a layer**: separazione Domain/Application/Infrastructure/Web.
- **Principio**: business rules nel dominio + servizi applicativi orchestrati, UI MVC snella.
- **Sicurezza**: Identity + ruoli + anti-forgery + aree protette.
- **Evolutività**: predisposizione per coupon free/paid, boost, rinnovi, pagamenti gateway.

## 2) Struttura solution
- `FacileSconti.Web`: MVC + Razor + Bootstrap 5
- `FacileSconti.Application`: DTO, interfacce servizi, result pattern
- `FacileSconti.Domain`: entity, enum, basi dominio
- `FacileSconti.Infrastructure`: EF Core DbContext/config, Identity, servizi
- `FacileSconti.Database`: script SQL manuali e documentazione

## 3) Modello dati principale
Entità chiave: `ApplicationUser`, `CustomerBusiness`, `SubscriptionPlan`, `CustomerContract`, `Coupon`, `CouponCategory`, `CouponDownload`, `PaymentRecord`, `AuditLog` (+ entità estese per immagini, boost, CMS, newsletter, pagamenti futuri).

## 4) Setup locale
1. Installare .NET 8 LTS SDK.
2. Configurare connection string in `FacileSconti.Web/appsettings*.json`.
3. Eseguire script SQL in ordine da `FacileSconti.Database/docs/EXECUTION_ORDER.md`.
4. Build e run:
   ```bash
   dotnet restore
   dotnet build FacileSconti.sln
   dotnet run --project FacileSconti.Web
   ```

## 5) Ruoli e aree
- **Admin**: dashboard e gestione completa (clienti, contratti, piani, coupon, categorie, richieste business, pagamenti, CMS, audit).
- **Customer**: dashboard, profilo azienda, contratto/rinnovo, gestione coupon, statistiche, boost.
- **EndUser**: dashboard personale, coupon scaricati, preferenze newsletter.

## 6) No migrations policy
Questo progetto **non usa EF migrations**. Lo schema è governato solo dagli script SQL versionati nella cartella `FacileSconti.Database`.

## 7) Roadmap TODO
- Implementare pipeline upload immagini sicura (whitelist MIME + antivirus hook).
- Aggiungere paginazione completa e ricerca full-text coupon.
- Implementare QR code reale (libreria dedicata) e validazione redemption endpoint.
- Estendere audit log con middleware centralizzato.
- Integrare payment gateway (Stripe/Nexi/PayPal) su `CouponPaymentConfig`.
- Aggiungere test unitari/integrati (services + authorization policy tests).
