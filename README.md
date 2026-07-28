# Executive Travel & Team Planning Portal

A full-stack rebuild of Meridian Group Holdings' (MGH) internal CEO travel and team planning tool — originally a single-file browser prototype, now a production-grade, containerized, multi-user web application.

---

## Overview

The CEO's office plans international executive travel, schedules meetings from a city-organized contact directory, tracks the whole team's calendar (trips, options, vacations, remote work), and generates printable per-person briefing one-pagers before every trip. This project replaces the original prototype's browser-only `localStorage` architecture with a real PostgreSQL-backed API, a modern Next.js frontend, and full Docker Compose deployment.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Next.js 16 (TypeScript, App Router), Tailwind CSS |
| Backend | ASP.NET Core (.NET 9) Web API, Clean Architecture-style feature folders |
| Database | PostgreSQL 17, Entity Framework Core |
| Auth | JWT bearer tokens, BCrypt password hashing |
| Email | Mailpit (containerized SMTP catcher — fully offline demo) |
| Testing | xUnit, `WebApplicationFactory`, EF Core InMemory provider |
| CI | GitHub Actions |
| Deployment | Docker Compose (Postgres + API + Web + Mailpit) |

---

## Quick Start

Requires Docker Desktop installed and running.

```bash
docker compose up --build
```

First run builds all images and can take a few minutes; subsequent runs are faster. Once it's up:

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API / Swagger | http://localhost:5104/swagger |
| Mailpit (caught emails) | http://localhost:8025 |
| PostgreSQL | `localhost:5432` (db: `travel_management`, user/pass: `postgres`/`postgres`) |

To stop (preserving data):
```bash
docker compose down
```
To stop and wipe all data:
```bash
docker compose down -v
```

### Demo Accounts

| Email | Password | Role |
|---|---|---|
| `admin@travelmanagement.com` | `Admin@123` | Admin |
| `alex@travelmanagement.com` | `Password123` | Employee (CEO) |
| `david@travelmanagement.com` | `Password123` | Employee |
| `sarah@travelmanagement.com` | `Password123` | Employee |
| `john@travelmanagement.com` | `Password123` | Employee |
| `maria@travelmanagement.com` | `Password123` | Employee |

---

## Core Features

- **Authentication** — JWT-based login, session persistence, Admin/Employee role separation
- **Dashboard** — CEO-scoped KPIs: upcoming trips, next departure, total travel days, meetings planned, travelers this week, at-risk trips (missing hotel/transport)
- **Team Calendar** — color-coded timeline (Trip/Option/Vacation/Remote), drill-down from half-year → quarter → month → week, person filtering, today marker
- **Trip Planner** — full trip CRUD with destination autocomplete, project/entity/hotel/transport, team assignment, bulk multi-leg entry, double-booking prevention
- **Meetings** — per-trip meeting management with order, priority, status, time, attending team, agenda, and repeatable materials with owners
- **Flights** — editable flights-on-file table, Google Flights deep-link search, inline capture-back form, multi-traveler booking, overlap validation
- **Team Plan** — per-person schedule entries with vacation approval workflow (Pending/Approved/Rejected), bulk multi-person entry, email notification on approval decision
- **Directory** — contacts organized by city, feeds the meeting picker, city/contact CRUD
- **One-Pagers** — printable per-person brief (itinerary, days-by-country, flights, meetings with materials checklist), emailable via Mailpit
- **Data Management** — full JSON export/import with transactional rollback on corrupted import; business data only (accounts are never overwritten)

---

## Repository Structure

```
├── TravelManagement.API/          # .NET 9 Web API (feature-folder architecture)
│   ├── Features/                  # One folder per domain (Trips, Meetings, Flights, ...)
│   ├── Infrastructure/             # Persistence, seeding, identity
│   └── Common/                    # Shared middleware, extensions, validation
├── TravelManagement.API.Tests/     # xUnit automated test suite
├── travel-management-client/       # Next.js frontend
├── docs/                           # BRD, PRD, TRD, Test Plan
├── .github/workflows/ci.yml        # CI pipeline
└── docker-compose.yml
```

---

## Running Tests

```bash
cd TravelManagement.API.Tests
dotnet test
```

Covers: trip double-booking rejection/exemption, authentication/authorization (401/403/201), and Export/Import transactional rollback. Runs automatically on every push via GitHub Actions — see the **Actions** tab for live results.

---

## Local Development (without Docker)

**Backend:**
```bash
cd TravelManagement.API
dotnet restore
dotnet run
```

**Frontend:**
```bash
cd travel-management-client
npm install
npm run dev
```

Requires a local PostgreSQL instance matching the connection string in `appsettings.Development.json`.

---

## Documentation

Full requirements and design documentation is in [`docs/`](./docs):
- `BRD.md` — Business Requirements Document
- `PRD.md` — Product Requirements Document (user stories, deliberate deviations from the prototype)
- `TRD.md` — Technical Requirements Document (architecture, API contract, security)
- `Test_Plan.md` — Test strategy, manual test case log, and automated test suite documentation

---

## Notes

- Runs fully offline once containers are built — no external API dependencies for core functionality (the Google Flights search button is the one deliberate exception, since it opens Google's actual website).
- Some deliberate deviations from the original prototype/planning documents are documented in `docs/PRD.md`, Section 8 (e.g., seed data uses different demo names, real-time sync uses polling instead of WebSockets, RBAC is implemented as Admin/Employee rather than full per-entry permissions).

---

*This project was built as part of a Product Engineer assignment. All materials are confidential.*
