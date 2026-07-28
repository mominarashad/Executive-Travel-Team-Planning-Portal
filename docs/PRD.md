# Product Requirements Document (PRD)

## Executive Travel & Team Planning Portal

**Version:** 1.0
**Date:** July 2026
**Author:** Momina Rashad
**Status:** In Progress
**Related:** [BRD.md](./BRD.md) · [TRD.md](./TRD.md)

---

## 1. Purpose

This document defines the product requirements for rebuilding the MGH Executive Travel & Team Planning Portal from a browser-based prototype into a production-grade full-stack web application. It translates the business needs outlined in the BRD into actionable user stories, functional requirements, and UX guidelines.

---

## 2. Personas

### P1 — Grace Hall (CEO Office Admin / EA)

- **Role:** Executive Assistant to the CEO
- **Tech comfort:** High — manages complex spreadsheets and tools daily
- **Goals:** Plan CEO trips end-to-end, schedule meetings with the right contacts per city, generate printable one-pagers before each trip, track the full team's availability
- **Frustrations:** Losing data when browser is cleared, manually exporting/importing JSON to share updates, no way to know if someone else changed the plan
- **Usage:** Daily, 2–4 hours. The primary power user who touches every feature.

### P2 — Alex Morgan (CEO)

- **Role:** CEO of Meridian Group Holdings
- **Tech comfort:** Moderate — prefers to consume information, not enter it
- **Goals:** See upcoming trips at a glance, review meeting briefs before travel, approve/reject team vacation requests
- **Frustrations:** Receiving outdated one-pagers, not knowing who is where and when
- **Usage:** Weekly, 15–30 minutes. Primarily views dashboards, one-pagers, and the calendar.

### P3 — Team Member (Jamie, Sam, Robin, Wesley, Kevin, Pierre)

- **Role:** MGH team members who travel with or support the CEO
- **Tech comfort:** Moderate to high
- **Goals:** Enter their own travel plans and vacation requests, see where the team is on the calendar, know when they're assigned to a trip or meeting
- **Frustrations:** Having to ask Grace for the latest version, can't request vacation and see approval status in one place
- **Usage:** Weekly, 10–20 minutes. Primarily manages their own entries and checks the calendar.

---

## 3. User Stories

### 3.1 Authentication

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-01 | As any user, I want to sign in with email and password so that my data is protected | Given seeded credentials, when I enter valid email/password, then I'm redirected to the dashboard. Invalid credentials show an error message. | Must |
| US-02 | As a signed-in user, I want my session to persist so that I don't have to log in every time I open the app | Session lasts until explicit logout or token expiry (24h minimum) | Must |

### 3.2 Overview Dashboard (KPIs)

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-03 | As Grace or Alex, I want to see key metrics at a glance so that I know the current state of travel planning | Dashboard shows: (1) upcoming CEO trip count, (2) next departure city + date, (3) total CEO travel days, (4) total meetings planned. All update in real-time as data changes. | Must |

### 3.3 Team Calendar

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-04 | As any user, I want to see a visual timeline of everyone's schedule so that I can spot conflicts and availability | Calendar shows June–December 2026 with color-coded bars: green=Trip, orange=Option, red=Vacation, gray=Remote. Each bar shows the city name. | Must |
| US-05 | As any user, I want to drill down from half-year to quarter, month, and week views so that I can see detail at different levels | Clicking a quarter zooms to quarter view, clicking a month zooms to month view, clicking a week zooms to week view. Breadcrumb navigation allows going back to parent level. | Must |
| US-06 | As any user, I want to filter the calendar by person so that I can focus on specific team members | Clicking a person's name shows only their row. Clicking additional names adds them. "All" resets the filter. | Must |
| US-07 | As any user, I want to see a "today" marker on the calendar so that I can orient myself in time | A vertical line marks today's date on all calendar views | Must |
| US-08 | As any user, I want to see vacation approval status on calendar bars so that I know which vacations are confirmed | Vacation bars display: ✓ Approved, ✗ Rejected, ⏳ Pending | Must |
| US-09 | As any user, I want "TBC" entries (no dates) to still appear so that tentative plans are visible | Entries without dates show as chips below the person's timeline row | Should |

### 3.4 CEO Trip Planner

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-10 | As Grace, I want to create a CEO trip by entering destination, dates, project, entity, and status so that I can plan each trip | Form requires destination (with autocomplete from 800+ cities). Dates are optional but To >= From when both are set. Status options: Confirmed / Option / Tentative. | Must |
| US-11 | As Grace, I want to assign hotel and transportation details to a trip so that logistics are tracked | Hotel field offers a per-city dropdown with ability to add custom hotels. Transport is free text. | Must |
| US-12 | As Grace, I want to select team members who will accompany the CEO on a trip so that attendance is tracked | Checkbox list of all team members. Selected members appear on the trip card and in the calendar. | Must |
| US-13 | As Grace, I want to pick people to meet from the city's contact directory so that meetings are planned | When a city is selected, the contact directory for that city is shown as checkboxes. Checking a contact creates a meeting entry. | Must |
| US-14 | As Grace, I want to set meeting details (order, priority, status, time, project, entity, team, agenda, materials) so that each meeting is fully briefed | Each meeting has: order (number), priority (High/Medium/Low), status (Proposed/Requested/Confirmed/Tentative/Declined/Completed), time, project (can differ from trip), entity (can differ from trip), attending team checkboxes, agenda text, and repeatable material rows (description + owner). | Must |
| US-15 | As Grace, I want to add multiple trip legs at once so that I can quickly enter multi-city itineraries | A bulk-add table allows entering multiple rows (project, entity, city, from, to, status) and committing them all at once. These trips are created without meetings (can be edited later). | Should |
| US-16 | As Grace, I want to filter and search trips by city, project, person, or text so that I can find specific trips quickly | Filter bar with: free text search, person dropdown, project dropdown, clear button. Results grouped as "Upcoming" and "Past". | Should |
| US-17 | As Grace, I want to delete a trip so that I can remove cancelled plans | Delete button on each trip card with confirmation dialog | Must |

### 3.5 Google Flights Integration

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-18 | As Grace, I want to search Google Flights for a trip's route so that I can find flight options | A button opens Google Flights in a new tab with origin, destination, and date pre-filled in the URL query string | Should |
| US-19 | As Grace, I want to record a flight I found into the system so that flight details are captured | An inline form captures: traveller, from, to, date, flight number, departure, arrival, aircraft. Saving adds it to the flights table and selects it for the current trip. | Should |

### 3.6 Flights on File

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-20 | As any user, I want to see all recorded flights in an editable table so that flight information is centralized | Table columns: Traveller, Route, Date, Flight No., Depart, Arrive, Aircraft. All fields are inline-editable. | Must |
| US-21 | As any user, I want a Google Flights link per flight row so that I can verify live schedules | Each row has a link that opens Google Flights with the route and date pre-filled | Should |
| US-22 | As Grace, I want to select an existing flight for a trip I'm planning so that I don't re-enter details | "Use for trip" button on each flight row sets it as the selected flight in the trip planner | Should |

### 3.7 Team Plan

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-23 | As a team member, I want to add entries to my schedule (dates, city, type, notes) so that my availability is tracked | Each person has a table of entries. Types: Trip / Option / Vacation / Remote. Add row button creates a new empty entry. | Must |
| US-24 | As a team member, I want to set my title and function so that one-pagers show my role | Two text fields per person: Title (e.g., "Group CFO") and Function (e.g., "Finance") | Must |
| US-25 | As Grace/Alex, I want to approve or reject vacation requests so that team availability is clear | Entries with type=Vacation have an approval dropdown: — / Pending / Approved / Rejected. Status appears on calendar bars. | Must |
| US-26 | As Grace, I want to add a plan entry to multiple people at once so that shared travel is entered quickly | Bulk-add form: From, To, City, Type, Notes + person checkboxes. Submitting creates an identical entry for each selected person. | Should |
| US-27 | As any user, I want to see days-by-country totals per person so that travel volume is summarized | Below each person's entry table, chips show: "Prague, Czechia: 3d", "New York, USA: 27d", "Total: 30d" | Should |

### 3.8 Directory

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-28 | As Grace, I want to manage contacts organized by city so that the meeting picker has the right people | Each city shows its contacts as chips. Add/remove contacts per city. Add/remove entire cities. | Must |
| US-29 | As Grace, I want to add a new city to the directory so that new destinations are supported | Text input with autocomplete from the global city list. Creates an empty city group ready for contacts. | Must |

### 3.9 One-Pagers

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-30 | As Grace, I want to generate a printable one-pager per person so that the CEO has a briefing document before each trip | One-pager contains: name, title/function, generation date, itinerary table (all entries sorted by date), days-by-country summary, meetings (with order, person, project, entity, status, priority, time, team, agenda, materials). | Must |
| US-31 | As Grace, I want to print a one-pager or save it as PDF so that I can hand it to the CEO | "Print / PDF" button triggers the browser's print dialog. Print CSS hides all UI chrome and formats for A4. | Must |
| US-32 | As Grace, I want to generate a one-pager scoped to a single trip so that trip-specific briefs are available | Per-trip one-pager shows only that trip's details, meetings, and materials | Should |

### 3.10 Data Management

| ID | Story | Acceptance Criteria | Priority |
|---|---|---|---|
| US-33 | As Grace, I want to export all data as a JSON file so that I have a backup | "Export" action downloads a JSON file containing the complete application state | Must |
| US-34 | As Grace, I want to import a JSON file so that I can restore data or migrate from the old prototype | "Import" action accepts a JSON file, validates its structure, and replaces current data | Must |
| US-35 | As any user, I want data to persist across sessions and browser changes so that nothing is lost | All data stored in PostgreSQL. Refreshing or reopening the browser shows the same data. | Must |
| US-36 | As two users working simultaneously, I want to see each other's changes so that we don't diverge | Polling at a reasonable interval (e.g., 30 seconds) keeps data in sync across tabs/browsers. Real-time push via WebSocket is a bonus. | Must |

---

## 4. Functional Requirements

### FR-01: Authentication
- The system shall support email/password authentication
- The system shall provide at least 3 seeded user accounts for demo purposes (e.g., Grace Hall admin, Alex Morgan CEO, Wesley Stone team member)
- The system shall issue JWT tokens or sessions with a minimum 24-hour expiry
- The system shall redirect unauthenticated users to the login page

### FR-02: Data Persistence
- The system shall store all data in PostgreSQL
- The system shall apply database migrations automatically on startup
- The system shall seed demo data on first run (trips, flights, team entries, directory contacts, hotels)
- Data shall survive container restarts (Docker volume)

### FR-03: API Design
- The backend shall expose a RESTful API with JSON request/response bodies
- All mutating endpoints shall require authentication
- The API shall return appropriate HTTP status codes (200, 201, 400, 401, 404, 500)
- The API shall validate input data and return clear error messages

### FR-04: Date Handling
- All dates shall be stored in ISO 8601 format (YYYY-MM-DD)
- The system shall enforce To date >= From date when both are provided
- Dates are optional (nullable) — entries with no dates are "TBC"
- Day count calculation shall be inclusive: daysBetween(from, to) = (to - from) + 1

### FR-05: Calendar Data Aggregation
- The calendar shall merge two data sources per person: their team plan entries AND their participation in CEO trips (as a travelling team member or meeting attendee)
- Duplicate entries (same person, same dates, same city from both sources) shall not be double-counted

### FR-06: Meeting Ordering
- Meetings within a trip shall be ordered by the `display_order` field (ascending numeric)
- New meetings shall default to the next available order number

### FR-07: Print / One-Pager
- One-pagers shall render as a full-screen overlay with print-optimized CSS
- The print view shall hide all navigation, toolbars, and interactive elements
- Page breaks shall separate major sections for clean multi-page printing

---

## 5. Non-Functional Requirements

| ID | Requirement | Target |
|---|---|---|
| NFR-01 | Startup time | `docker compose up` to fully loaded app in < 2 minutes |
| NFR-02 | Page load time | Initial page load < 3 seconds after startup |
| NFR-03 | API response time | 95th percentile < 500ms for all endpoints |
| NFR-04 | Browser support | Latest Chrome and Firefox (demo environment) |
| NFR-05 | Responsive design | Usable on desktop (1280px+) and tablet (768px+) viewports |
| NFR-06 | Data integrity | No data loss on concurrent edits (last-write-wins acceptable for MVP) |
| NFR-07 | Offline capability | Entire stack runs without internet via Docker |
| NFR-08 | Code quality | Consistent naming, separation of concerns, no hardcoded secrets |

---

## 6. MoSCoW Prioritization

### Must Have (MVP — required for functional parity)
- User authentication with seeded accounts
- KPI dashboard (4 cards)
- Team calendar with drill-down (half-year → quarter → month → week)
- Calendar person filtering and today marker
- CEO trip CRUD (create, read, update, delete)
- Meeting management with all fields (order, priority, status, time, team, agenda, materials)
- Flights table with inline editing
- Team plan with per-person entries and vacation approval
- Contact directory by city (CRUD)
- Individual one-pagers with print support
- JSON export/import
- PostgreSQL persistence
- Docker Compose one-command startup
- Seed data on first run
- Multi-user data consistency (polling)

### Should Have (important but not blocking)
- Bulk trip add (multi-leg)
- Bulk team plan entry (multi-person)
- Trip filtering and search
- Google Flights deep linking
- Inline flight add from trip planner
- Trip-scoped one-pagers (per trip segment)
- Days-by-country summary per person
- "TBC" chips for undated calendar entries
- Hotel per-city management with custom entries

### Could Have (bonus if time permits)
- Email integration via Mailpit (email one-pagers, vacation approval notifications)
- Real-time sync via WebSocket / SignalR
- Role-based access control
- Audit trail / change history
- Dark mode

### Won't Have (this version)
- Mobile native app
- External calendar integration (Google Calendar / Outlook)
- Expense tracking
- Automated flight/hotel booking
- Multi-language support

---

## 7. UX Notes & Design Decisions

### 7.1 Navigation Redesign

**Prototype:** Single long page with anchor links. All 7 sections stacked vertically.

**New design:** Sidebar or top-tab navigation with distinct pages/views. Proposed structure:

| Route | Content |
|---|---|
| `/dashboard` | KPI cards + Team Calendar (the two most-viewed components together) |
| `/trips` | Trip list + Trip planner form (create/edit) |
| `/trips/:id` | Trip detail view with meetings |
| `/flights` | Flights table |
| `/team` | Team plan entries for all people |
| `/directory` | Contact directory by city |
| `/one-pager/:name` | Full-screen printable one-pager |

**Rationale:** Separating into pages reduces cognitive load. The prototype's single-page approach worked for one user but becomes overwhelming when multiple sections are open. The dashboard landing page gives immediate situational awareness.

### 7.2 Calendar Interaction

The calendar remains the most visually complex component. Key design decisions:

- Retain the 4-level drill-down (half → quarter → month → week) as it matches the prototype's behavior
- Use CSS Grid for bar positioning rather than absolute pixel math
- Add hover tooltips on bars showing full details (city, dates, type, notes)
- Keep the person filter chips above the calendar

### 7.3 Trip Planner Flow

**Prototype:** One large form with everything visible at once (destination, dates, hotel, transport, meetings, materials).

**New design:** Multi-step or accordion approach:
1. Trip basics (destination, dates, project, entity, status)
2. Logistics (hotel, transport, flight)
3. Team (who is going)
4. Meetings (picker → meeting detail cards)

**Rationale:** The prototype's form is ~800px tall. Breaking it into steps reduces errors and makes it mobile-friendly. Each step validates before proceeding.

### 7.4 Visual Modernization

- Clean, minimal UI using a consistent design system
- Replace inline color hacks with a proper theme
- Use card-based layouts for trip cards and meeting cards
- Improve table styling with alternating rows and hover effects
- Status badges with consistent colors across the application

---

## 8. Deliberate Deviations from the Prototype

| # | Prototype Behavior | New Behavior | Rationale |
|---|---|---|---|
| D-1 | Single-page with anchor navigation | Multi-page with route-based navigation | Better UX for a multi-section application; enables deep linking and back button |
| D-2 | All data in one localStorage blob | Separate API endpoints per entity | Proper data model; enables granular updates and better performance |
| D-3 | Firebase Auth with Google sign-in | Simple JWT auth with seeded users | Meets assignment requirement; runs fully offline without Firebase dependency |
| D-4 | Real-time Firebase sync | Polling-based sync (WebSocket as bonus) | Simpler to implement reliably; real-time is a bonus feature |
| D-5 | Trip form shows everything at once | Multi-step or accordion form | Better UX; reduces cognitive load and enables validation per step |
| D-6 | Hotels hardcoded in JavaScript | Hotels stored in database, manageable via API | Proper data persistence; new hotels survive across sessions |
| D-7 | Projects/entities are static arrays | Projects/entities stored in database | Allows CRUD; new custom entries are persisted and shared across users |
| D-8 | Calendar hardcoded to H2 2026 | Calendar date range driven by data | More flexible; system can adapt to any date range present in the data |
| D-9 | Print uses window.print() on the whole page | Dedicated one-pager route with print-optimized CSS | Cleaner print output; avoids printing unrelated sections |
| D-10 | Dashboard shows 4 raw counts with no context | Enriched with days-until-departure, active traveler count, and at-risk trip flagging (missing hotel/transport within 14 days) | Raw counts without context don't support decision-making; a KPI dashboard should help catch problems, not just report numbers |
| D-11 | No conflict detection between overlapping trips/plan entries | Double-booking prevention added: a person cannot be committed to two confirmed trips, or a confirmed trip and an approved vacation, with overlapping dates. Tentative/Option-status commitments are exempt, since real planning requires holding multiple possibilities before one is confirmed | Not requested in the original brief, but identified as a real data-integrity gap during build — a system managing executive travel should not silently allow physically impossible schedules |
| D-12 | Import replaces all data unconditionally | Import restores business data (trips, meetings, flights, directory, team plans, hotels, projects, entities) but never overwrites Users or Roles | A literal full-data-replace would overwrite password hashes with stripped/absent values from export, locking out every account; accounts are treated as environment-specific, not portable business data |
| D-13 | Meeting order set manually with no collision handling | Meeting display order is validated for uniqueness per trip at both the application layer and a DB-level filtered unique index; a colliding order is rejected with a clear error rather than silently overwritten | Prevents two meetings on the same trip from silently sharing an order number, which would make meeting sequencing ambiguous |
| D-14 | Trip destination can be changed freely at any time | Once a trip has one or more meetings, its destination city becomes locked (read-only) in the edit form | Meetings are tied to city-scoped contacts and the trip's hotel is city-scoped; changing destination after meetings exist would silently orphan those references, producing a trip that shows meetings/hotel for a city it's no longer going to |
| D-15 | No concept of a "primary" traveler on a trip | The CEO is pre-selected by default in the team-members picker when creating a new trip, but this is a soft default only — never enforced, and can be unchecked | Reflects the BRD's premise that trips are fundamentally CEO travel, while still permitting legitimate delegation-only trips (CEO unavailable, lower-stakes visit) without requiring a workaround |
| D-16 | Role separation not modeled | Two roles implemented: Admin (can manage user accounts) and Employee (cannot). All other data (trips, meetings, directory, etc.) remains visible/editable by any authenticated user, matching OQ-1's MVP resolution ("everyone sees everything") | Full RBAC (e.g., restricting edits to one's own entries) was explicitly scoped as bonus in both the PRD (OQ-1) and the assignment brief; a working Admin/Employee distinction was implemented as a genuine, tested proof of the mechanism rather than left completely unbuilt |
| D-17 | Data export/import assumed a stable authentication model | Global authentication enforced on every endpoint except login (fallback policy), rather than endpoint-by-endpoint opt-in | Fail-secure default: a forgotten `[Authorize]` annotation on a future endpoint silently defaults to protected rather than silently defaulting to public |
| D-18 | Google Flights deep-link only (prototype) | Origin/destination/date pre-filled search launches from the trip itself; a linked inline capture form saves the chosen flight directly against that trip and traveler(s), with multi-traveler support and overlap validation | Goes beyond the bonus requirement's literal ask by adding conflict detection and multi-person capture, not just a faster manual-entry flow |

---

## 9. Seed Data Requirements

The following data must load automatically on first `docker compose up`:

| Entity | Count | Source |
|---|---|---|
| Users | 8 | Alex Morgan (CEO), Grace Hall (Admin), + 6 team members |
| CEO Trips | 1 | Prague, Czechia — Northwind (23–25 Jun) with 4 meetings |
| Flights | 8 | Default flights from prototype (Vienna↔Prague, Singapore↔Prague, Amsterdam↔NY) |
| Team Plan Entries | ~12 | Default entries for all 8 people from prototype |
| Directory Cities | 12+ | Prague, Vienna, Amsterdam, US, Lisbon, Bangkok, Jakarta, KL, Singapore, HK, Warsaw, Zurich, Seoul, Other |
| Directory Contacts | 100+ | All contacts from prototype's DEFAULT_DIR |
| Hotels | 10 | Default hotels per city from prototype |
| Projects | 15 | All project names from prototype |
| Entities | 9 | All MGH entity names from prototype |

---

## 10. Open Questions

| # | Question | Proposed Resolution |
|---|---|---|
| OQ-1 | Should team members only see/edit their own entries, or can everyone see everything? | MVP: everyone sees everything (matches prototype). Bonus: RBAC limits editing to own entries. |
| OQ-2 | Should the calendar support date ranges beyond H2 2026? | Yes — the data model will use actual dates, not hardcode months. The UI may default to showing the current half-year but adapt to the data. |
| OQ-3 | What happens when a directory city is deleted that has meetings linked to it? | Soft-delete or warn the user. Meetings should retain their contact name even if the city/contact is removed. |

---

### Known Limitations

- **US-09 (TBC/undated entries)** is not supported. `TeamPlanEntry.FromDate`/`ToDate` are non-nullable in the current schema, so a plan entry with no dates cannot be represented. This is a Should-have per the PRD's own MoSCoW table, not a Must-have, and was consciously deprioritized given the project timeline.

---

*This document will evolve during the build phase. Deviations discovered during implementation will be documented here with dates and reasoning.*
