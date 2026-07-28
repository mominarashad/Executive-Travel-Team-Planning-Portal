"use client";

import { useEffect, useState, useMemo, useCallback } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import { getCalendar } from "@/services/calendarService";
import { PersonCalendar } from "@/types/calendar";
import { usePolling } from "@/hooks/usePolling";
type ZoomLevel = "half-year" | "quarter" | "month" | "week";

const TYPE_COLORS: Record<string, string> = {
  Trip: "#22c55e",
  Option: "#f97316",
  Vacation: "#ef4444",
  Remote: "#9ca3af",
};

const APPROVAL_ICON: Record<string, string> = {
  Pending: "⏳",
  Approved: "✓",
  Rejected: "✗",
};

function toISODate(d: Date): string {
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, "0");
  const day = String(d.getDate()).padStart(2, "0");
  return `${y}-${m}-${day}`;
}

function parseISODate(s: string): Date {
  const [y, m, d] = s.split("-").map(Number);
  return new Date(y, m - 1, d);
}

function daysBetween(a: Date, b: Date): number {
  const ms = 1000 * 60 * 60 * 24;
  return Math.round(
    (Date.UTC(b.getFullYear(), b.getMonth(), b.getDate()) -
      Date.UTC(a.getFullYear(), a.getMonth(), a.getDate())) / ms
  );
}

function addDays(d: Date, days: number): Date {
  const copy = new Date(d);
  copy.setDate(copy.getDate() + days);
  return copy;
}

function getHalfYearRange(date: Date): [Date, Date] {
  const year = date.getFullYear();
  return date.getMonth() < 6
    ? [new Date(year, 0, 1), new Date(year, 5, 30)]
    : [new Date(year, 6, 1), new Date(year, 11, 31)];
}

function getQuarterRange(date: Date): [Date, Date] {
  const year = date.getFullYear();
  const q = Math.floor(date.getMonth() / 3);
  const startMonth = q * 3;
  return [new Date(year, startMonth, 1), new Date(year, startMonth + 3, 0)];
}

function getMonthRange(date: Date): [Date, Date] {
  const year = date.getFullYear();
  const month = date.getMonth();
  return [new Date(year, month, 1), new Date(year, month + 1, 0)];
}

function getWeekRange(date: Date): [Date, Date] {
  const day = date.getDay();
  const diffToMonday = day === 0 ? -6 : 1 - day;
  const monday = addDays(date, diffToMonday);
  return [monday, addDays(monday, 6)];
}

function getRangeForZoom(zoom: ZoomLevel, anchor: Date): [Date, Date] {
  switch (zoom) {
    case "half-year": return getHalfYearRange(anchor);
    case "quarter": return getQuarterRange(anchor);
    case "month": return getMonthRange(anchor);
    case "week": return getWeekRange(anchor);
  }
}

function formatShort(d: Date): string {
  return d.toLocaleDateString(undefined, { month: "short", day: "numeric", year: "numeric" });
}

function quarterLabel(d: Date): string {
  return `Q${Math.floor(d.getMonth() / 3) + 1} ${d.getFullYear()}`;
}

function halfLabel(d: Date): string {
  return `H${d.getMonth() < 6 ? 1 : 2} ${d.getFullYear()}`;
}

function monthLabel(d: Date): string {
  return d.toLocaleDateString(undefined, { month: "long", year: "numeric" });
}

export default function CalendarPage() {
  const { user } = useAuth();
  const router = useRouter();

  const [zoomLevel, setZoomLevel] = useState<ZoomLevel>("half-year");
  const [anchor, setAnchor] = useState<Date>(new Date());
  const [people, setPeople] = useState<PersonCalendar[]>([]);
  const [selectedPersonIds, setSelectedPersonIds] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [rangeStart, rangeEnd] = useMemo(() => getRangeForZoom(zoomLevel, anchor), [zoomLevel, anchor]);
  const totalDays = daysBetween(rangeStart, rangeEnd) + 1;

  const today = useMemo(() => {
    const t = new Date();
    t.setHours(0, 0, 0, 0);
    return t;
  }, []);

  const loadCalendar = useCallback(async (silent = false) => {
    if (!silent) {
      setLoading(true);
      setError(null);
    }
    try {
      const data = await getCalendar(toISODate(rangeStart), toISODate(rangeEnd));
      setPeople(data);
    } catch {
      if (!silent) setError("Failed to load calendar data.");
    } finally {
      if (!silent) setLoading(false);
    }
  }, [rangeStart, rangeEnd]);

  useEffect(() => {
    if (!user) {
      router.push("/login");
      return;
    }
    loadCalendar();
  }, [user, router, loadCalendar]);
  usePolling(() => loadCalendar(true), 30000);

  function togglePerson(id: string) {
    setSelectedPersonIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  }

  function shiftAnchor(direction: 1 | -1) {
    let days = 0;
    if (zoomLevel === "half-year") days = 183 * direction;
    if (zoomLevel === "quarter") days = 91 * direction;
    if (zoomLevel === "month") days = 30 * direction;
    if (zoomLevel === "week") days = 7 * direction;
    setAnchor((prev) => addDays(prev, days));
  }

  function zoomTo(level: ZoomLevel, newAnchor?: Date) {
    setZoomLevel(level);
    if (newAnchor) setAnchor(newAnchor);
  }

  const visiblePeople = selectedPersonIds.length === 0
    ? people
    : people.filter((p) => selectedPersonIds.includes(p.userId));

  if (!user) return null;

  let subPeriodButtons: { label: string; onClick: () => void }[] = [];
  if (zoomLevel === "half-year") {
    const isFirstHalf = anchor.getMonth() < 6;
    const year = anchor.getFullYear();
    const q1Start = new Date(year, isFirstHalf ? 0 : 6, 1);
    const q2Start = new Date(year, isFirstHalf ? 3 : 9, 1);
    subPeriodButtons = [
      { label: quarterLabel(q1Start), onClick: () => zoomTo("quarter", q1Start) },
      { label: quarterLabel(q2Start), onClick: () => zoomTo("quarter", q2Start) },
    ];
  } else if (zoomLevel === "quarter") {
    const [qStart] = getQuarterRange(anchor);
    subPeriodButtons = [0, 1, 2].map((i) => {
      const m = new Date(qStart.getFullYear(), qStart.getMonth() + i, 1);
      return { label: monthLabel(m), onClick: () => zoomTo("month", m) };
    });
  } else if (zoomLevel === "month") {
    const [mStart, mEnd] = getMonthRange(anchor);
    const weeks: Date[] = [];
    let cursor = getWeekRange(mStart)[0];
    while (cursor <= mEnd) {
      weeks.push(cursor);
      cursor = addDays(cursor, 7);
    }
    subPeriodButtons = weeks.map((w) => ({
      label: `Week of ${formatShort(w)}`,
      onClick: () => zoomTo("week", w),
    }));
  }

  const breadcrumbs: { label: string; onClick: () => void }[] = [
    { label: String(anchor.getFullYear()), onClick: () => zoomTo("half-year") },
    { label: halfLabel(anchor), onClick: () => zoomTo("half-year") },
  ];
  if (zoomLevel !== "half-year") {
    breadcrumbs.push({ label: quarterLabel(anchor), onClick: () => zoomTo("quarter") });
  }
  if (zoomLevel === "month" || zoomLevel === "week") {
    breadcrumbs.push({ label: monthLabel(anchor), onClick: () => zoomTo("month") });
  }
  if (zoomLevel === "week") {
    const [wStart] = getWeekRange(anchor);
    breadcrumbs.push({ label: `Week of ${formatShort(wStart)}`, onClick: () => zoomTo("week") });
  }

  return (
    <DashboardLayout>
      <div className="space-y-4">
        <div className="flex justify-between items-center flex-wrap gap-2">
          <h2 className="text-2xl font-bold">Team Calendar</h2>
          <div className="flex gap-2">
            {(["half-year", "quarter", "month", "week"] as ZoomLevel[]).map((z) => (
              <button
                key={z}
                onClick={() => setZoomLevel(z)}
                className={`px-3 py-1.5 rounded-lg text-sm border ${
                  zoomLevel === z ? "bg-[#0f3c3c] text-white border-[#0f3c3c]" : "bg-white text-gray-700 border-gray-200"
                }`}
              >
                {z === "half-year" ? "Half Year" : z.charAt(0).toUpperCase() + z.slice(1)}
              </button>
            ))}
          </div>
        </div>

        <div className="text-sm text-gray-500 flex gap-1 flex-wrap items-center">
          {breadcrumbs.map((b, i) => (
            <span key={i} className="flex items-center gap-1">
              <button onClick={b.onClick} className="hover:underline hover:text-[#0f3c3c]">
                {b.label}
              </button>
              {i < breadcrumbs.length - 1 && <span>›</span>}
            </span>
          ))}
        </div>

        <div className="flex items-center gap-3">
          <button onClick={() => shiftAnchor(-1)} className="px-2 py-1 border border-gray-200 rounded text-sm bg-white">← Prev</button>
          <span className="text-sm font-medium">{formatShort(rangeStart)} – {formatShort(rangeEnd)}</span>
          <button onClick={() => shiftAnchor(1)} className="px-2 py-1 border border-gray-200 rounded text-sm bg-white">Next →</button>
        </div>

        {subPeriodButtons.length > 0 && (
          <div className="flex gap-2 flex-wrap">
            {subPeriodButtons.map((b, i) => (
              <button key={i} onClick={b.onClick} className="px-3 py-1 text-xs rounded-lg border border-gray-200 bg-white hover:bg-gray-50">
                {b.label}
              </button>
            ))}
          </div>
        )}

        <div className="flex gap-4 text-xs items-center">
          {Object.entries(TYPE_COLORS).map(([type, color]) => (
            <span key={type} className="flex items-center gap-1">
              <span style={{ backgroundColor: color, width: 12, height: 12, display: "inline-block", borderRadius: 3 }} />
              {type}
            </span>
          ))}
          <span className="flex items-center gap-1 text-blue-600">
            <span style={{ width: 2, height: 12, background: "#2563eb", display: "inline-block" }} />
            Today
          </span>
        </div>

        <div className="flex gap-2 flex-wrap items-center">
          <button
            onClick={() => setSelectedPersonIds([])}
            className={`px-3 py-1 rounded-full text-xs border ${selectedPersonIds.length === 0 ? "bg-[#0f3c3c] text-white border-[#0f3c3c]" : "bg-white border-gray-200"}`}
          >
            All
          </button>
          {people.map((p) => (
            <button
              key={p.userId}
              onClick={() => togglePerson(p.userId)}
              className={`px-3 py-1 rounded-full text-xs border ${selectedPersonIds.includes(p.userId) ? "bg-[#0f3c3c] text-white border-[#0f3c3c]" : "bg-white border-gray-200"}`}
            >
              {p.name}
            </button>
          ))}
        </div>

        {loading && <p className="text-gray-500">Loading calendar...</p>}
        {error && <p className="text-red-600">{error}</p>}

        {!loading && !error && (
          <div className="bg-white rounded-xl shadow p-4 space-y-2">
            {visiblePeople.map((p) => {
              const showToday = today >= rangeStart && today <= rangeEnd;
              const todayLeftPct = showToday ? (daysBetween(rangeStart, today) / totalDays) * 100 : null;

              return (
                <div key={p.userId} className="flex items-center gap-3">
                  <div className="w-36 shrink-0 text-sm font-medium truncate" title={p.name}>{p.name}</div>
                  <div style={{ position: "relative", height: 32, flex: 1, background: "#f3f4f6", borderRadius: 6 }}>
                    {p.entries.map((entry, idx) => {
                      const entryStart = parseISODate(entry.fromDate);
                      const entryEnd = parseISODate(entry.toDate);
                      const clippedStart = entryStart < rangeStart ? rangeStart : entryStart;
                      const clippedEnd = entryEnd > rangeEnd ? rangeEnd : entryEnd;
                      if (clippedEnd < rangeStart || clippedStart > rangeEnd) return null;

                      const leftDays = daysBetween(rangeStart, clippedStart);
                      const widthDays = daysBetween(clippedStart, clippedEnd) + 1;
                      const leftPct = (leftDays / totalDays) * 100;
                      const widthPct = Math.max((widthDays / totalDays) * 100, 1.5);
                      const color = TYPE_COLORS[entry.type] || "#9ca3af";
                      const approvalIcon = entry.type === "Vacation" && entry.approvalStatus
                        ? APPROVAL_ICON[entry.approvalStatus] ?? ""
                        : "";

                      return (
                        <div
                          key={idx}
                          title={`${entry.cityName} — ${entry.type} (${entry.fromDate} to ${entry.toDate})${entry.notes ? " — " + entry.notes : ""}`}
                          style={{
                            position: "absolute", left: `${leftPct}%`, width: `${widthPct}%`,
                            top: 3, bottom: 3, backgroundColor: color, borderRadius: 4,
                            display: "flex", alignItems: "center", paddingLeft: 4,
                            color: "white", fontSize: 10, overflow: "hidden", whiteSpace: "nowrap",
                          }}
                        >
                          {entry.cityName} {approvalIcon}
                        </div>
                      );
                    })}

                    {todayLeftPct !== null && (
                      <div
                        title="Today"
                        style={{ position: "absolute", left: `${todayLeftPct}%`, top: 0, bottom: 0, width: 2, background: "#2563eb", zIndex: 10 }}
                      />
                    )}
                  </div>
                </div>
              );
            })}
            {visiblePeople.length === 0 && <p className="text-gray-400 text-center py-8">No people to show.</p>}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}