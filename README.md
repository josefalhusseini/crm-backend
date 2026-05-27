# CRM API – Azure Functions & Cosmos DB

CRM-lösning med ASP.NET Minimal Web API och Azure Function mot Cosmos DB.

---

## Köra API lokalt

```bash
cd backend/CrmApi
dotnet run
```

Swagger öppnas på: http://localhost:5000/swagger

---

## Köra Azure Function lokalt

```bash
cd backend/CrmFunction
func start
```

Funktionen lyssnar på Cosmos DB change feed och skickar mail till ansvarig säljare när en kund skapas eller uppdateras.

---

## Environment Variables / Konfiguration

### API (`appsettings.Development.json`)
| Nyckel | Beskrivning |
|--------|-------------|
| `CosmosDb:ConnectionString` | Connection string till Cosmos DB |
| `CosmosDb:DatabaseName` | Databasnamn (default: `CrmDb`) |
| `CosmosDb:ContainerName` | Containernamn (default: `Customers`) |

### Azure Function (`local.settings.json`)
| Nyckel | Beskrivning |
|--------|-------------|
| `AzureWebJobsStorage` | Azure Storage connection string |
| `CosmosDbConnectionString` | Connection string till Cosmos DB |
| `Email:SmtpHost` | SMTP-server (t.ex. `sandbox.smtp.mailtrap.io`) |
| `Email:SmtpPort` | SMTP-port (default: 587) |
| `Email:Username` | SMTP-användarnamn |
| `Email:Password` | SMTP-lösenord |
| `Email:From` | Avsändaradress |

> Om Email-konfiguration saknas loggas mailet till konsolen istället.

---

## API Endpoints

| Metod | URL | Beskrivning |
|-------|-----|-------------|
| GET | `/api/customers` | Hämta alla kunder |
| GET | `/api/customers/{id}` | Hämta kund med ID |
| POST | `/api/customers` | Skapa ny kund |
| PUT | `/api/customers/{id}` | Uppdatera kund |
| DELETE | `/api/customers/{id}` | Ta bort kund |
| GET | `/api/customers/search?customerName=Anna` | Sök på kundnamn |
| GET | `/api/customers/search?sellerName=Erik` | Sök på säljarnamn |

---

## Exempel-JSON för POST /api/customers

```json
{
  "name": "Anna Karlsson",
  "title": "VD",
  "phone": "070-123 45 67",
  "email": "anna.karlsson@foretag.se",
  "address": "Storgatan 1, 411 38 Göteborg",
  "responsibleSeller": {
    "name": "Erik Svensson",
    "phone": "073-987 65 43",
    "email": "erik.svensson@salja.se"
  }
}
```

## Exempel-JSON för PUT /api/customers/{id}

```json
{
  "phone": "070-999 88 77",
  "responsibleSeller": {
    "name": "Lisa Johansson",
    "phone": "073-111 22 33",
    "email": "lisa.johansson@salja.se"
  }
}
```

---

## curl-kommandon

```bash
# Skapa kund
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d @example-customer.json

# Hämta alla kunder
curl http://localhost:5000/api/customers

# Sök på kundnamn
curl "http://localhost:5000/api/customers/search?customerName=Anna"

# Sök på säljarnamn
curl "http://localhost:5000/api/customers/search?sellerName=Erik"

# Uppdatera kund (ersätt {id} med faktiskt ID)
curl -X PUT http://localhost:5000/api/customers/{id} \
  -H "Content-Type: application/json" \
  -d '{"phone": "070-999 88 77"}'

# Ta bort kund
curl -X DELETE http://localhost:5000/api/customers/{id}
```

---

## Redovisningschecklista

- [ ] API startar utan fel (`dotnet run`)
- [ ] Swagger öppnas och visar alla endpoints
- [ ] POST kund skapar post i Cosmos DB
- [ ] Cosmos DB Data Explorer visar kunden
- [ ] Azure Function startar (`func start`)
- [ ] Funktionen triggas av Cosmos DB change feed
- [ ] Mail/logg skickas till säljarens e-post
- [ ] PUT kund → nytt mail/logg skickas
- [ ] GET search fungerar på kundnamn och säljarnamn

---

## 1-minutsförklaring till läraren

> "Jag har byggt en CRM-lösning med tre delar. Först ett ASP.NET Minimal Web API med full CRUD och sökning mot Cosmos DB – det ser du i Swagger här. Cosmos DB är på Azure och heter CrmDb med containern Customers. Varje kund har en ansvarig säljare lagrad direkt i dokumentet. Sedan finns en Azure Function med Cosmos DB change feed-trigger – den lyssnar på ändringar i containern och skickar automatiskt ett e-postmeddelande till säljarens mailadress när en kund skapas eller uppdateras. Jag ska nu skapa en kund live och visa att funktionen triggas."

---

## Azure-resurser

| Resurs | Namn |
|--------|------|
| Cosmos DB | `crm-cosmos-josef` |
| Databas | `CrmDb` |
| Container | `Customers` |
| Leases | `leases` |
| Function App | `crm-functions-josef` |
| Storage | `crmstoragejosef` |
| Resource Group | `RG-Josef-Al-Husseini-e0aa81-DotNetCloudDeveloper-VT-Mars-Goteborg` |
