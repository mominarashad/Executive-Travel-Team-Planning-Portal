# Test Plan — Travel Management API

**Project:** Executive Travel & Team Planning Portal — Backend (TravelManagement.API)
**Version:** 1.0
**Date:** July 2026
**Related:** PRD.md · BRD.md · TRD.md

---

## 1. Test Strategy

### 1.1 Approach

Testing was performed **incrementally, module by module**, as each vertical slice (entity → repository → service → controller) was built. Each module was verified before moving to the next, using a consistent pattern:

1. **Happy path** — confirm the core CRUD/aggregation operation works and returns the expected shape.
2. **Negative/validation cases** — confirm bad input (invalid references, malformed data, out-of-range values) is rejected with a clean `400` and a consistent error envelope, rather than an unhandled `500`.
3. **Business rule cases** — for modules with domain logic beyond CRUD (Trips, TeamPlans, Meetings), confirm the specific rule (e.g. double-booking prevention) fires correctly in both the blocking and non-blocking direction.
4. **Boundary/empty-state cases** — confirm the system behaves correctly when there's nothing to return (empty arrays, `null` fields) rather than erroring.
5. **Security cases** — once authentication was enabled globally, confirm unauthenticated requests are blocked and role-based restrictions are enforced.

### 1.2 Scope

| In scope | Out of scope |
|---|---|
| All REST API endpoints (Auth, Directory, Flights, Meetings, TeamPlans, Trips, Users, Hotels, Projects, Entities, Calendar, Dashboard, One-Pager, Export/Import) | Frontend UI (not yet built at time of this test plan) |
| Business rule validation (double-booking, whitelist fields, referential integrity) | Load/performance testing |
| Data integrity (transactional rollback on Import) | Penetration testing |
| Authentication and role-based access control | Automated CI test suite (manual testing only — see §5) |

### 1.3 Test Types Applied

- **Functional / happy path** — does the endpoint do what it's supposed to do.
- **Negative testing** — malformed GUIDs, missing references, out-of-range enums, bad JSON.
- **Business rule / domain logic testing** — double-booking conflict detection across Trips and TeamPlans.
- **Data integrity testing** — Export/Import round-trip fidelity; transactional rollback on corrupted import.
- **Security testing** — unauthenticated access rejection; role-based authorization (Admin vs. Employee).
- **Regression testing** — after each hardening pass (e.g. adding validation to `TripRepository`), previously-working endpoints were re-verified to confirm no breakage.

### 1.4 Tools & Environment

| Tool | Purpose |
|---|---|
| Swagger UI (`/swagger`) | Primary manual test interface — request construction, execution, response inspection |
| PowerShell (`Invoke-RestMethod`) | Scripted request sequences for multi-step scenarios |
| DBeaver | Schema verification, ER diagram confirmation |
| Local PostgreSQL instance | Test database, reset/reseeded as needed between test passes |

**Environment:** Local development (`http://localhost:5104`), ASP.NET Core / .NET, PostgreSQL, EF Core with `dotnet watch` / `dotnet run`.

### 1.5 Defect Classes Found During Testing

| Class | Example | Resolution |
|---|---|---|
| Missing controller actions | `TripsController` initially had no `CreateTrip`/`UpdateTrip` after a refactor | Actions restored |
| Inconsistent error envelopes | Controller-level `BadRequest()` vs. middleware's `{statusCode, message}` shape | Validation centralized in repositories; controller-level manual checks removed |
| False-positive success on malformed bulk input | `BulkCreateAsync` silently no-op'd on a wrongly-shaped request, still returned "success" | Added explicit empty-payload guard |
| Silent bad-data substitution | `BulkCreateTeamPlanDto` defaulted missing dates to "today" and missing city to `Guid.Empty` | Changed to explicit validation/rejection instead of silent substitution |
| Missing reverse-direction business rule | Trip→TeamPlan conflict check existed; TeamPlan→Trip conflict check did not | Added symmetric check in `TeamPlanRepository` |
| Seed data role misconfiguration | All seeded users assigned the Admin role, making RBAC untestable | Split seeder into Admin vs. Employee role assignments |
| Hot-reload static-field bug | `dotnet watch`'s hot-reload failed to initialize new `static readonly` fields, causing `ArgumentNullException` | Resolved via cold restart (`dotnet run` fresh) |

---

## 2. Key Test Cases by Module

### 2.1 Hotels (`/api/hotels`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| HTL-01 | Create hotel | `POST /api/hotels` with valid `cityId` | `201`, `HotelDto` with resolved `cityName` | ✅ Pass |
| HTL-02 | Get all | `GET /api/hotels` | `200`, list includes created hotel | ✅ Pass |
| HTL-03 | Get by city | `GET /api/hotels/city/{cityId}` | `200`, only that city's hotels returned | ✅ Pass |
| HTL-04 | Update | `PUT /api/hotels/{id}` with new name | `204`, name persisted on re-GET | ✅ Pass |
| HTL-05 | Delete (soft) | `DELETE /api/hotels/{id}` | `204`; subsequent `GET` → `404`; excluded from list | ✅ Pass |
| HTL-06 | Bad city reference | `POST` with random GUID `cityId` | `400`, `"City not found."` (not `500`) | ✅ Pass |
| HTL-07 | Duplicate name per city | `POST` same `cityId` + `name` twice | `400`, `"Hotel already exists for this city."` | ✅ Pass |

### 2.2 Projects (`/api/projects`) & Entities (`/api/entities`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| PRJ-01 | Duplicate name blocked | `POST` same name twice | `400`, `"Project already exists."` | ✅ Pass |
| PRJ-02 | System-record delete blocked | `DELETE` a seeded `IsSystem: true` record | `400`, `"Cannot delete a system-defined project."` | ✅ Pass |
| PRJ-03 | Normal record delete succeeds | `DELETE` a non-system record | `204`, then `404` on re-GET | ✅ Pass |
| ENT-01 to ENT-03 | Same three cases mirrored for `/api/entities` | — | Same results | ✅ Pass |

### 2.3 Trips (`/api/trips`) — Core Business Logic

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| TRP-01 | Date sanity (bulk) | `POST /api/trips/bulk` with `endDate < startDate` | `400`, `"End date cannot be earlier than start date."` | ✅ Pass |
| TRP-02 | Bad city reference | `POST /api/trips` with random GUID `destinationCityId` | `400`, `"Destination city not found."` | ✅ Pass |
| TRP-03 | **Double-booking: confirmed vs. confirmed** | Create Trip A (Confirmed, overlapping dates, same user) then attempt Trip B (Confirmed, overlapping) | `400`, message naming the conflicting trip, city, and dates | ✅ Pass |
| TRP-04 | **Exemption: tentative vs. tentative** | Two `Option`-status trips, same user, overlapping dates, no existing confirmed commitment | Both `201 Created` — tentative trips may coexist | ✅ Pass |
| TRP-05 | **Vacation exemption (Pending)** | User has a `Vacation`/`Pending` TeamPlanEntry; attempt overlapping Confirmed trip | `201 Created` — pending vacation does not block | ✅ Pass |
| TRP-06 | **Vacation exemption (Approved)** | Same as TRP-05, but vacation approved first | `400`, `"already has a vacation entry ... that overlaps"` | ✅ Pass |
| TRP-07 | Bulk empty-payload guard | `POST /api/trips/bulk` with malformed/empty `trips` array | `400`, `"At least one trip is required."` (previously silently "succeeded" with 0 trips created) | ✅ Pass (after fix) |

### 2.4 TeamPlans (`/api/TeamPlans`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| TPL-01 | Type whitelist | `POST` with `"type": "Banana"` | `400`, `"Invalid type 'Banana'. Must be one of: Trip, Option, Vacation, Remote."` | ✅ Pass |
| TPL-02 | Date sanity | `POST` with `toDate < fromDate` | `400`, `"To date cannot be earlier than From date."` | ✅ Pass |
| TPL-03 | Bad user reference | `POST` with random GUID `userId` | `400`, `"User not found."` | ✅ Pass |
| TPL-04 | **Reverse double-booking check** | User has existing Confirmed trip; attempt `POST` a `Remote`-type TeamPlanEntry with overlapping dates | `400`, message naming the conflicting trip | ✅ Pass |

### 2.5 Meetings (`/api/meetings`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| MTG-01 | Priority whitelist | `POST` with `"priority": "Urgent"` | `400`, `"Invalid priority 'Urgent'. Must be one of: High, Medium, Low."` | ✅ Pass |
| MTG-02 | Status whitelist | `POST` with `"status": "InProgress"` | `400`, `"Invalid status 'InProgress'. Must be one of: Proposed, Requested, Confirmed, Tentative, Declined, Completed."` | ✅ Pass |
| MTG-03 | Bad contact reference | `POST` with random GUID `contactId` | `400`, `"Contact not found."` | ✅ Pass |
| MTG-04 | Valid create | `POST` with valid trip/contact/attendees | `201`, full `MeetingDto` returned | ✅ Pass |
| MTG-05 | **Display order collision** | `POST` second meeting on same trip with same `displayOrder` | `400`, `"Display order 1 is already used by another meeting on this trip."` | ✅ Pass |

### 2.6 Calendar Aggregation (`/api/calendar`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| CAL-01 | Multi-source merge | `GET /api/calendar?from=2026-06-01&to=2026-12-31` | Per-person entries from both Trip participation and TeamPlan records | ✅ Pass |
| CAL-02 | Null-city (TBC) handling | Entry with no `cityId` | `cityName: "TBC"` rendered correctly | ✅ Pass |
| CAL-03 | Vacation approval status surfaced | Vacation-type entry | `approvalStatus` field populated (`Pending`/`Approved`/`Rejected`) | ✅ Pass |
| CAL-04 | Person filter | `&personIds={userId}` | Only specified person(s) returned | ✅ Pass |

### 2.7 Dashboard KPIs (`/api/dashboard`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| DSH-01 | Full aggregation | `GET /api/dashboard` | Returns `upcomingTripsCount`, `nextDeparture` (enriched with `daysUntil`), `totalTravelDaysThisYear`, `upcomingMeetingsCount`, `travelersThisWeekCount`, `tripsNeedingAttentionCount` | ✅ Pass |
| DSH-02 | Empty state | No upcoming trips | `nextDeparture: null` (not an error, not a placeholder string) | Verified by design; boundary confirmed |
| DSH-03 | Attention-window boundary | Trip 15 days out (window = 14 days) | Correctly excluded from `tripsNeedingAttentionCount` | ✅ Pass |

### 2.8 One-Pager (`/api/onepager/{userId}`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| ONE-01 | Full aggregation | `GET /api/onepager/{userId}` for a user with trips + meetings | Itinerary (deduped, sorted), `daysByCountry` (arithmetically verified), `totalDays`, full meeting detail with team/materials | ✅ Pass |
| ONE-02 | Empty-state user | New user, zero attachments | `200`, all arrays empty (`itinerary: []`, `meetings: []`), `totalDays: 0` — not an error | ✅ Pass |
| ONE-03 | Nonexistent user | Random GUID | `404 Not Found` | ✅ Pass |

### 2.9 Export / Import (`/api/data/export`, `/api/data/import`)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| EXP-01 | Full export | `GET /api/data/export` | JSON file with all 14 tables; `Users` present but `PasswordHash` excluded | ✅ Pass |
| EXP-02 | Round-trip import | `POST /api/data/import` with the exported JSON | `200`, `"Data imported successfully. User accounts were not modified."` | ✅ Pass |
| EXP-03 | Round-trip data fidelity | Re-run `GET /api/trips`, `GET /api/onepager/{userId}` after import | Data identical to pre-import state | ✅ Pass |
| EXP-04 | **Transactional rollback on corrupted import** | Corrupt a `DestinationCityId` reference in the export JSON, re-import | `400`, `"Import failed and was rolled back."`; follow-up `GET /api/trips` shows original data fully intact, not wiped or partial | ✅ Pass |

### 2.10 Security — Authentication & Authorization

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| SEC-01 | Login (anonymous access) | `POST /api/auth/login` with no token | `200` with JWT | ✅ Pass |
| SEC-02 | Unauthenticated request blocked | `GET /api/trips` with no Authorization header | `401 Unauthorized` | ✅ Pass |
| SEC-03 | Authenticated request succeeds | `GET /api/trips` with valid JWT | `200` with data | ✅ Pass |
| SEC-04 | Role restriction enforced (negative) | `POST /api/users` with a non-Admin (Employee-role) token | `403 Forbidden` | ✅ Pass |
| SEC-05 | Role restriction enforced (positive) | `POST /api/users` with Admin-role token | `201 Created` | ✅ Pass |

---

## 3. Evidence of Execution

All test cases above were executed manually against the running API via Swagger UI during development, with responses captured and reviewed turn-by-turn as each module was built. Representative evidence:

- **TRP-03/TRP-06 (double-booking):** confirmed via a live sequence — creating a Confirmed trip, attempting an overlapping Confirmed trip (blocked with exact conflict message), then repeating with a Pending vacation (allowed) vs. an Approved vacation (blocked), isolating the exact code path by removing confounding test trips between attempts.
- **EXP-04 (rollback):** confirmed by deliberately corrupting a foreign key in a saved export file, observing the clean `400` rollback response, then immediately re-querying `GET /api/trips` and confirming all 6 original trips were present and unmodified — proving the transaction did not partially commit.
- **SEC-04/SEC-05 (RBAC):** confirmed after discovering and fixing a seed-data defect (all users had been assigned the Admin role, making the negative case untestable); re-seeded with Employee-role non-admin users, then confirmed `403` for a non-admin token and `201` for the admin token.
- **ONE-02 (empty state):** confirmed by creating a throwaway user via `POST /api/users` with zero trip/meeting/plan attachments and verifying the one-pager endpoint returned a valid, empty-but-well-formed object rather than an error.

Raw request/response pairs for each case above are available in the development session transcript; this document summarizes the test design and outcomes rather than reproducing every raw payload.

---

## 4. Known Gaps / Deferred Items

| Item | Status | Reason |
|---|---|---|
| Flights (`/api/flights`) validation audit | Not performed | Same CRUD risk profile as Hotels; deferred due to time constraints, no double-booking-style risk present |
| TBC entries (US-09, no-date plan entries) | Not supported | `TeamPlanEntry.FromDate`/`ToDate` are non-nullable in the current schema; documented as a known deviation rather than blocking MVP (US-09 is a Should-have) |
| Automated test suite / CI pipeline | **Implemented** — see §6 | Converted the highest-value manual cases (double-booking, RBAC, rollback) into automated tests, now part of the delivered submission |

---

## 5. Flight & Email Validation (Added Post-Initial Test Plan)

| ID | Case | Steps | Expected | Result |
|---|---|---|---|---|
| FLT-01 | Overlap detection | Book a flight for a traveller, then attempt a second overlapping flight for the same traveller | `400`, `"Traveller already has a flight (...) that overlaps this time window."` | ✅ Pass |
| FLT-02 | Time sanity | `POST` with `arrivalTime <= departureTime` | `400`, `"Arrival time must be after departure time."` | ✅ Pass |
| EML-01 | One-pager email delivery | Trigger `POST /api/onepager/{userId}/send`, check Mailpit inbox | Email received with formatted itinerary/meetings/flights tables | ✅ Pass (manually verified via Mailpit UI) |
| EML-02 | Vacation approval notification | Change a Vacation entry's `ApprovalStatus` from Pending to Approved | Email received addressed to that user, subject reflects decision | ✅ Pass (manually verified via Mailpit UI) |

## 6. Automated Test Suite & CI Pipeline

An xUnit test project (`TravelManagement.API.Tests`) was added using `WebApplicationFactory<Program>` and EF Core's in-memory provider, converting the highest-value manual test cases into automated, repeatable tests:

| Test | Covers |
|---|---|
| `ConfirmedTripOverlap_IsRejected` | TRP-03 — confirmed vs. confirmed double-booking rejection |
| `TentativeTripOverlap_IsAllowed` | TRP-04 — tentative vs. tentative exemption |
| `UnauthenticatedRequest_Returns401` | SEC-02 |
| `NonAdminUser_CreatingUser_Returns403` | SEC-04 |
| `AdminUser_CreatingUser_Returns201` | SEC-05 |
| `CorruptedImport_RollsBackWithoutLosingExistingData` | EXP-04 — transactional rollback |

**Result:** 6/6 passing locally and in CI.

**CI Pipeline:** GitHub Actions workflow (`.github/workflows/ci.yml`) runs `dotnet restore` → `dotnet build` → `dotnet test` on every push/PR to `main`. Verified green — see the repository's Actions tab for live run history.
