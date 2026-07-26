# Business Requirements Document (BRD)

## Executive Travel & Team Planning Portal

**Version:** 1.0
**Date:** July 2026
**Author:** Momina Rashad
**Status:** Approved

---

## 1. Business Context

Meridian Group Holdings (MGH) is a diversified holding company operating across energy, logistics, healthcare, real estate, digital, and financial services. The CEO and a core team of 7–8 people travel extensively for international deal-making, investor roadshows, joint venture negotiations, and partner meetings across Europe, Southeast Asia, the Middle East, and the United States.

The CEO's office currently coordinates all travel planning, team scheduling, meeting preparation, and itinerary generation through an internal browser-based prototype. This prototype has served the team well as a proof of concept but has reached its limits as the organization's travel activity grows.

## 2. Problem Statement

The current prototype suffers from several fundamental limitations that hinder the CEO office's ability to plan effectively at scale:

- **No persistent backend:** All data lives in the browser's localStorage. If a user clears their browser data or switches devices, all planning information is lost.
- **No true multi-user support:** Two team members cannot work on the same plan simultaneously. Sharing requires manually exporting and importing JSON files, creating version conflicts and data loss risks.
- **No access control:** Anyone with the file can see and edit everything. There is no concept of roles (e.g., CEO office admin vs. team member who can only view/edit their own entries).
- **No data integrity guarantees:** There is no validation, no audit trail, and no protection against accidental data corruption.
- **Fragile architecture:** A single HTML file with 1,000+ lines of inline JavaScript is unmaintainable and untestable.

These issues result in the CEO office spending unnecessary time on data reconciliation, manual sharing workflows, and recovering from data loss incidents — time that should be spent on high-value executive coordination.

## 3. Stakeholders

| Stakeholder | Role | Interest |
|---|---|---|
| Alex Morgan (CEO) | Primary end user | Needs accurate, up-to-date itineraries and meeting briefs before every trip |
| Grace Hall (CEO Office / EA) | Power user / admin | Manages all trip planning, contact directories, team scheduling, and generates one-pagers |
| Team members (Jamie, Sam, Robin, Wesley, Kevin, Pierre) | Contributors | Enter their own travel plans, vacation requests, and availability |
| MGH Board / Compliance | Oversight | May need audit trail of travel and meeting history |

## 4. Business Objectives

| # | Objective | Measurable Outcome |
|---|---|---|
| BO-1 | Eliminate data loss risk | Zero incidents of lost planning data per quarter |
| BO-2 | Enable real-time multi-user collaboration | Two or more users can edit the plan simultaneously without conflicts |
| BO-3 | Reduce trip planning time | One-pager generation takes < 30 seconds (currently involves manual collation) |
| BO-4 | Provide a single source of truth | All team members see the same data at all times — no version conflicts |
| BO-5 | Modernize the user experience | A responsive, intuitive interface that works on desktop and tablet |

## 5. Scope

### 5.1 In Scope

- Full-stack web application replacing the current HTML prototype
- All existing prototype capabilities preserved (see Feature Inventory for parity checklist)
- PostgreSQL database as the single source of truth
- User authentication with seeded demo accounts
- Containerized deployment via Docker Compose (one-command startup)
- JSON export/import for data portability
- Modern, responsive UI redesigned with Next.js

**Capabilities carried forward from the prototype:**

1. Overview KPI dashboard (upcoming trips, next departure, total travel days, meetings planned)
2. Team calendar with drill-down (half-year → quarter → month → week), color-coded by entry type, person filtering, today marker
3. CEO trip planner with destination, dates, project, entity, status, hotel, transport, flight selection, team assignment, and meeting scheduling
4. Meeting management with order, priority, status, time, team, agenda, and materials with owners
5. Bulk trip entry (multiple legs at once) and bulk team plan entry (apply to multiple people)
6. Flights-on-file table with inline editing and Google Flights deep linking
7. Team plan with per-person entries, vacation approval workflow (Pending / Approved / Rejected)
8. Contact directory organized by city, feeding the trip planner's meeting picker
9. Printable individual one-pagers with itinerary, days-by-country, meetings, agendas, and materials
10. Data export (JSON) and import

### 5.2 Out of Scope

- Mobile native applications (responsive web is sufficient)
- Integration with external calendar systems (Google Calendar, Outlook)
- Expense tracking or budget management
- Automated flight booking or hotel reservation systems
- Localization / multi-language support
- GDPR-specific data retention and deletion workflows (can be added later)

### 5.3 Bonus / Stretch Goals (if time permits)

- Email integration using a containerized mail catcher (e.g., Mailpit) for offline demo: email one-pagers, notify on vacation approval
- Real-time multi-user sync via WebSocket / SignalR
- Role-based access control (CEO office admin vs. team member)
- Automated tests (unit, integration, e2e)
- Audit trail / change history

## 6. Assumptions

1. The application will be used by a small team (< 15 concurrent users). Enterprise-scale performance optimization is not required.
2. The CEO is "Alex Morgan" and is the fixed reference point for trip planning. The system does not need to support multiple CEO profiles.
3. The planning horizon is H2 2026 (June–December) as defined by the prototype, though the data model should not hard-code this.
4. Hotel and flight information is entered manually (no integration with booking APIs).
5. The directory of contacts and organizations is maintained manually by the CEO office.
6. Internet access is not guaranteed during demos — the entire stack must run offline via Docker.
7. The existing prototype's seed data (trips, flights, team entries, directory contacts) will be used as demo data in the rebuilt product.

## 7. Success Criteria

| # | Criterion | Verification Method |
|---|---|---|
| SC-1 | Application starts with a single `docker compose up` command | Manual test on a clean machine |
| SC-2 | All 10 capabilities from Section 5.1 are functional and demonstrable | Live demo walkthrough |
| SC-3 | Two browser windows show the same data without manual sync | Open two tabs, create a trip in one, verify it appears in the other (via polling at minimum) |
| SC-4 | One-pager prints correctly with all meeting details | Print to PDF from the browser |
| SC-5 | Data survives container restart | `docker compose down && docker compose up`, verify data persists |
| SC-6 | Seed data loads automatically on first run | Fresh `docker compose up` shows pre-populated demo data |

## 8. Constraints

- **Technology stack is mandated:** Next.js (TypeScript preferred) + .NET 9 Web API + PostgreSQL
- **Deployment:** Fully Dockerized; `docker compose up` must start everything
- **Portability:** Must run fully offline on a laptop (no cloud dependencies at demo time)
- **Timeline:** Target completion by 27 July 2026 (bonus), hard deadline 29 July 2026
- **Confidentiality:** The prototype, assignment materials, and solution must remain private at all times

## 9. Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Calendar visualization complexity exceeds timeline | Medium | High | Prioritize functional correctness over visual polish; use a library (e.g., custom CSS grid) rather than building from scratch |
| Docker environment differences between dev and demo laptop | Low | High | Test `docker compose up` on a clean machine before presentation |
| Data model changes mid-build as edge cases emerge | Medium | Medium | Design schema with flexibility (nullable fields, JSONB for extensible data); document deviations in PRD |
| Google Flights deep linking may change URL format | Low | Low | Treat as a nice-to-have; the link is a convenience, not a core dependency |

---

*This document will be updated as design and development progress. Any changes to scope or assumptions will be noted here with dates.*
