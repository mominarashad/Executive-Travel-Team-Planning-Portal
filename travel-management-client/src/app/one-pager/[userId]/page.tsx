"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { getOnePager, sendOnePagerEmail } from "@/services/onePagerService";
import { OnePager } from "@/types/onePager";

function formatDateTime(iso: string): { date: string; time: string } {
  const d = new Date(iso);
  return {
    date: d.toLocaleDateString(undefined, { year: "numeric", month: "short", day: "numeric" }),
    time: d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" }),
  };
}


export default function OnePagerViewPage() {
  const { user } = useAuth();
  const router = useRouter();
  const params = useParams<{ userId: string }>();

  const [data, setData] = useState<OnePager | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showEmailBox, setShowEmailBox] = useState(false);
  const [emailAddress, setEmailAddress] = useState("");
  const [emailSending, setEmailSending] = useState(false);
  const [emailMessage, setEmailMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      router.push("/login");
      return;
    }
    getOnePager(params.userId)
      .then(setData)
      .catch(() => setError("Failed to load one-pager."))
      .finally(() => setLoading(false));
  }, [user, router, params.userId]);

  if (!user) return null;

  async function handleSendEmail() {
    if (!emailAddress.trim()) return;
    setEmailSending(true);
    setEmailMessage(null);
    try {
      const result = await sendOnePagerEmail(params.userId, emailAddress.trim());
      setEmailMessage(result.message);
      setShowEmailBox(false);
    } catch {
      setEmailMessage("Failed to send email.");
    } finally {
      setEmailSending(false);
    }
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <div className="no-print flex justify-between items-center bg-white shadow px-6 py-3 sticky top-0 z-10">
        <Link href="/one-pager" className="text-sm text-[#0f3c3c] hover:underline">
          ← Back to list
        </Link>
        {data && (
          <div>
            <button
              onClick={() => window.print()}
              className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
            >
              Print / Save PDF
            </button>
            <button
              onClick={() => setShowEmailBox((s) => !s)}
              className="ml-3 bg-white border border-[#0f3c3c] text-[#0f3c3c] px-4 py-2 rounded-lg text-sm"
            >
              Email
            </button>
          </div>
        )}
      </div>

      {showEmailBox && (
        <div className="no-print max-w-3xl mx-auto mt-3 flex gap-2">
          <input
            type="email"
            placeholder="traveller@example.com"
            value={emailAddress}
            onChange={(e) => setEmailAddress(e.target.value)}
            className="border border-gray-200 rounded-lg p-2 text-sm flex-1"
          />
          <button onClick={handleSendEmail} disabled={emailSending} className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60">
            {emailSending ? "Sending..." : "Send"}
          </button>
        </div>
      )}
      {emailMessage && <p className="no-print text-center text-sm text-green-600 mt-2">{emailMessage}</p>}

      {loading && <p className="text-center text-gray-500 py-10">Loading...</p>}
      {error && <p className="text-center text-red-600 py-10">{error}</p>}

      {data && (
        <div className="onepager-page max-w-4xl mx-auto bg-white shadow my-6 p-10 text-sm">
          {/* ---- Header ---- */}
          <div className="border-b pb-4 mb-6 flex justify-between items-start">
            <div>
              <h1 className="text-2xl font-bold">{data.name}</h1>
              <p className="text-gray-600">{data.title} · {data.function}</p>
            </div>
            <p className="text-xs text-gray-400">
              Generated {new Date(data.generatedAt).toLocaleString()}
            </p>
          </div>

          {/* ---- Itinerary ---- */}
          <section className="mb-8 page-break-avoid">
            <h2 className="text-base font-semibold mb-2 uppercase tracking-wide text-gray-700">Itinerary</h2>
            {data.itinerary.length === 0 ? (
              <p className="text-gray-400 text-sm">No trips or plan entries.</p>
            ) : (
              <table>
                <thead>
                  <tr className="bg-gray-50 text-left text-gray-600">
                    <th className="p-2">City</th>
                    <th className="p-2">Country</th>
                    <th className="p-2">From</th>
                    <th className="p-2">To</th>
                    <th className="p-2">Type</th>
                    <th className="p-2">Notes</th>
                  </tr>
                </thead>
                <tbody>
                  {data.itinerary.map((entry, i) => (
                    <tr key={i}>
                      <td className="p-2 font-medium">{entry.cityName}</td>
                      <td className="p-2">{entry.country || "—"}</td>
                      <td className="p-2">{entry.fromDate}</td>
                      <td className="p-2">{entry.toDate}</td>
                      <td className="p-2">{entry.type}</td>
                      <td className="p-2 text-gray-500">{entry.notes || "—"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </section>

          {/* ---- Days by Country ---- */}
          <section className="mb-8 page-break-avoid">
            <h2 className="text-base font-semibold mb-2 uppercase tracking-wide text-gray-700">Days by Country</h2>
            {data.daysByCountry.length === 0 ? (
              <p className="text-gray-400 text-sm">No travel days recorded.</p>
            ) : (
              <table className="max-w-md">
                <thead>
                  <tr className="bg-gray-50 text-left text-gray-600">
                    <th className="p-2">Country</th>
                    <th className="p-2">Days</th>
                  </tr>
                </thead>
                <tbody>
                  {data.daysByCountry.map((d, i) => (
                    <tr key={i}>
                      <td className="p-2">{d.country}</td>
                      <td className="p-2">{d.days}</td>
                    </tr>
                  ))}
                  <tr className="bg-gray-50 font-semibold">
                    <td className="p-2">Total</td>
                    <td className="p-2">{data.totalDays}</td>
                  </tr>
                </tbody>
              </table>
            )}
          </section>

          {/* ---- Flights ---- */}
          <section className="mb-8 page-break-avoid">
            <h2 className="text-base font-semibold mb-2 uppercase tracking-wide text-gray-700">Flights</h2>
            {data.flights.length === 0 ? (
              <p className="text-gray-400 text-sm">No flights on file for this person.</p>
            ) : (
              <table>
                <thead>
                  <tr className="bg-gray-50 text-left text-gray-600">
                    <th className="p-2">Trip</th>
                    <th className="p-2">Airline</th>
                    <th className="p-2">Flight No.</th>
                    <th className="p-2">Route</th>
                    <th className="p-2">Depart</th>
                    <th className="p-2">Arrive</th>
                    <th className="p-2">Aircraft</th>
                    <th className="p-2">Booking Ref.</th>
                  </tr>
                </thead>
                <tbody>
                  {data.flights.map((f, i) => {
                    const dep = formatDateTime(f.departureTime);
                    const arr = formatDateTime(f.arrivalTime);
                    return (
                      <tr key={i}>
                        <td className="p-2">{f.tripCity}</td>
                        <td className="p-2">{f.airline}</td>
                        <td className="p-2">{f.flightNumber}</td>
                        <td className="p-2">{f.departureAirport} → {f.arrivalAirport}</td>
                        <td className="p-2">{dep.date} {dep.time}</td>
                        <td className="p-2">{arr.date} {arr.time}</td>
                        <td className="p-2">{f.aircraft || "—"}</td>
                        <td className="p-2">{f.bookingReference || "—"}</td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            )}
          </section>

          {/* ---- Meetings ---- */}
<section>
  <h2 className="text-base font-semibold mb-3 uppercase tracking-wide text-gray-700">Meetings</h2>
  {data.meetings.length === 0 ? (
    <p className="text-gray-400 text-sm">No meetings scheduled.</p>
  ) : (
    <div className="space-y-6">
      {(() => {
        const orderCounters: Record<string, number> = {};
        return data.meetings.map((m, i) => {
          orderCounters[m.tripId] = (orderCounters[m.tripId] || 0) + 1;
          const displayNumber = orderCounters[m.tripId];
          return (
            <div key={i} className="border rounded-lg overflow-hidden page-break-avoid">
              <table>
                <tbody>
                  <tr className="bg-gray-50">
                    <td className="p-2 font-semibold w-1/4">Meeting #{displayNumber}</td>
                    <td className="p-2" colSpan={3}>
                      {m.contactName} — {m.tripCity} ({m.tripStartDate} → {m.tripEndDate})
                    </td>
                  </tr>
                  <tr>
                    <td className="p-2 text-gray-500">Time</td>
                    <td className="p-2">{m.scheduledTime || "—"}</td>
                    <td className="p-2 text-gray-500">Priority</td>
                    <td className="p-2">{m.priority}</td>
                  </tr>
                  <tr>
                    <td className="p-2 text-gray-500">Status</td>
                    <td className="p-2">{m.status}</td>
                    <td className="p-2 text-gray-500">Project / Entity</td>
                    <td className="p-2">
                      {m.projectName || "—"}{m.projectName && m.businessEntityName && " / "}{m.businessEntityName || (!m.projectName ? "—" : "")}
                    </td>
                  </tr>
                  <tr>
                    <td className="p-2 text-gray-500 align-top">Agenda</td>
                    <td className="p-2" colSpan={3}>{m.agenda || "—"}</td>
                  </tr>
                  <tr>
                    <td className="p-2 text-gray-500 align-top">Team</td>
                    <td className="p-2" colSpan={3}>{m.team.length > 0 ? m.team.join(", ") : "—"}</td>
                  </tr>
                </tbody>
              </table>

              <div className="p-2">
                <p className="text-xs font-semibold text-gray-600 uppercase mb-1 mt-1">Materials to Prepare</p>
                {m.materials.length === 0 ? (
                  <p className="text-gray-400 text-xs pb-1">None required.</p>
                ) : (
                  <table>
                    <thead>
                      <tr className="bg-gray-50 text-left text-gray-600">
                        <th className="p-2 w-8">✓</th>
                        <th className="p-2">Description</th>
                        <th className="p-2">Owner</th>
                      </tr>
                    </thead>
                    <tbody>
                      {m.materials.map((mat, j) => (
                        <tr key={j}>
                          <td className="p-2 text-center">☐</td>
                          <td className="p-2">{mat.description}</td>
                          <td className="p-2">{mat.ownerName || "—"}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          );
        });
      })()}
    </div>
  )}
</section>
</div>
)}
</div>
  );
}
