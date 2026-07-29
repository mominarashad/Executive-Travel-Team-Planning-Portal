"use client";

import { useEffect, useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import { getFlights, createFlight, updateFlight, deleteFlight } from "@/services/flightService";
import { getTrips } from "@/services/tripService";
import { getUsers } from "@/services/userService";
import { Flight } from "@/types/flight";
import { Trip } from "@/types/trip";
import { AppUser } from "@/types/user";

function googleFlightsUrl(origin: string, destination: string, date: string): string {
    const dateOnly = date.split("T")[0];
    const query = `Flights from ${origin} to ${destination} on ${dateOnly}`;
    return `https://www.google.com/travel/flights?q=${encodeURIComponent(query)}`;
}

export default function FlightsPage() {
    const { user } = useAuth();
    const router = useRouter();

    const [flights, setFlights] = useState<Flight[]>([]);
    const [trips, setTrips] = useState<Trip[]>([]);
    const [users, setUsers] = useState<AppUser[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showForm, setShowForm] = useState(false);
    const [tripId, setTripId] = useState("");
    const [userId, setUserId] = useState("");
    const [airline, setAirline] = useState("");
    const [flightNumber, setFlightNumber] = useState("");
    const [departureTime, setDepartureTime] = useState("");
    const [arrivalTime, setArrivalTime] = useState("");
    const [departureAirport, setDepartureAirport] = useState("");
    const [arrivalAirport, setArrivalAirport] = useState("");
    const [aircraft, setAircraft] = useState("");
    const [bookingReference, setBookingReference] = useState("");
    const [formError, setFormError] = useState<string | null>(null);
    const [submitting, setSubmitting] = useState(false);

    const [editingId, setEditingId] = useState<string | null>(null);
    const [editValues, setEditValues] = useState<Partial<Flight>>({});

    useEffect(() => {
        if (!user) {
            router.push("/login");
            return;
        }
        loadAll();
    }, [user, router]);

    async function loadAll() {
        setLoading(true);
        setError(null);
        try {
            const [flightsData, tripsData, usersData] = await Promise.all([
                getFlights(),
                getTrips(),
                getUsers(),
            ]);
            setFlights(flightsData);
            setTrips(tripsData);
            setUsers(usersData);
        } catch {
            setError("Failed to load flights.");
        } finally {
            setLoading(false);
        }
    }

    function resetForm() {
        setTripId(""); setUserId(""); setAirline(""); setFlightNumber("");
        setDepartureTime(""); setArrivalTime(""); setDepartureAirport("");
        setArrivalAirport(""); setAircraft(""); setBookingReference("");
        setFormError(null);
    }
    function handleTripChange(newTripId: string) {
        setTripId(newTripId);
        const selectedTrip = trips.find((t) => t.id === newTripId);
        if (selectedTrip && selectedTrip.teamMemberIds.length > 0 && !selectedTrip.teamMemberIds.includes(userId)) {
            setUserId(""); // clear traveller if they're not on the newly selected trip
        }
    }

    function getAvailableTravellers(): AppUser[] {
        const selectedTrip = trips.find((t) => t.id === tripId);
        if (selectedTrip && selectedTrip.teamMemberIds.length > 0) {
            return users.filter((u) => selectedTrip.teamMemberIds.includes(u.id));
        }
        return users; // no trip selected yet, or trip has no team assigned — show everyone
    }

    async function handleCreate(e: FormEvent) {
        e.preventDefault();
        setFormError(null);
        setSubmitting(true);
        try {
            await createFlight({
                tripId, userId, airline, flightNumber,
                departureTime, arrivalTime, departureAirport,
                arrivalAirport, aircraft, bookingReference,
            });
            setShowForm(false);
            resetForm();
            await loadAll();
        } catch (err: any) {
            setFormError(err?.response?.data?.message || "Failed to add flight.");
        } finally {
            setSubmitting(false);
        }
    }

    function startEdit(f: Flight) {
        setEditingId(f.id);
        setEditValues({ ...f });
    }

    async function saveEdit(original: Flight) {
        try {
            await updateFlight(original.id, {
                tripId: editValues.tripId ?? original.tripId,
                userId: editValues.userId ?? original.userId,
                airline: editValues.airline ?? original.airline,
                flightNumber: editValues.flightNumber ?? original.flightNumber,
                departureTime: editValues.departureTime ?? original.departureTime,
                arrivalTime: editValues.arrivalTime ?? original.arrivalTime,
                departureAirport: editValues.departureAirport ?? original.departureAirport,
                arrivalAirport: editValues.arrivalAirport ?? original.arrivalAirport,
                aircraft: editValues.aircraft ?? original.aircraft,
                bookingReference: editValues.bookingReference ?? original.bookingReference,
            });
            setEditingId(null);
            await loadAll();
        } catch {
            alert("Failed to update flight.");
        }
    }

    async function handleDelete(id: string) {
        if (!confirm("Delete this flight?")) return;
        try {
            await deleteFlight(id);
            await loadAll();
        } catch {
            alert("Failed to delete flight.");
        }
    }

    function tripLabel(id: string): string {
        const t = trips.find((t) => t.id === id);
        return t ? `${t.destinationCity} (${t.startDate})` : id;
    }

    function userLabel(id: string): string {
        const u = users.find((u) => u.id === id);
        return u ? u.name : id;
    }

    if (!user) return null;

    return (
        <DashboardLayout>
            <div className="space-y-6">
                <div className="flex justify-between items-center">
                    <h2 className="text-2xl font-bold">Flights on File</h2>
                    <button
                        onClick={() => { setShowForm((s) => !s); resetForm(); }}
                        className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
                    >
                        {showForm ? "Cancel" : "+ Add Flight"}
                    </button>
                </div>

                {showForm && (
                    <form onSubmit={handleCreate} className="bg-white rounded-xl shadow p-6 space-y-4">
                        {formError && <p className="text-red-600 text-sm">{formError}</p>}
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-sm font-medium mb-1">Trip</label>
                                <select required value={tripId} onChange={(e) => handleTripChange(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                                    <option value="">Select a trip</option>
                                    {trips.map((t) => <option key={t.id} value={t.id}>{t.destinationCity} ({t.startDate})</option>)}
                                </select>
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Traveller</label>
                                <select required value={userId} onChange={(e) => setUserId(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" disabled={!tripId}>
                                    <option value="">{tripId ? "Select a person" : "Select a trip first"}</option>
                                    {getAvailableTravellers().map((u) => <option key={u.id} value={u.id}>{u.name}</option>)}
                                </select>
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Airline</label>
                                <input required value={airline} onChange={(e) => setAirline(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Flight Number</label>
                                <input required value={flightNumber} onChange={(e) => setFlightNumber(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Departure Time</label>
                                <input type="datetime-local" required value={departureTime} onChange={(e) => setDepartureTime(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Arrival Time</label>
                                <input type="datetime-local" required value={arrivalTime} onChange={(e) => setArrivalTime(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Departure Airport (code)</label>
                                <input required value={departureAirport} onChange={(e) => setDepartureAirport(e.target.value.toUpperCase())} maxLength={3} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Arrival Airport (code)</label>
                                <input required value={arrivalAirport} onChange={(e) => setArrivalAirport(e.target.value.toUpperCase())} maxLength={3} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Aircraft</label>
                                <input value={aircraft} onChange={(e) => setAircraft(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Booking Reference</label>
                                <input value={bookingReference} onChange={(e) => setBookingReference(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                            </div>
                        </div>
                        <button type="submit" disabled={submitting} className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60">
                            {submitting ? "Saving..." : "Add Flight"}
                        </button>
                    </form>
                )}

                {loading && <p className="text-gray-500">Loading flights...</p>}
                {error && <p className="text-red-600">{error}</p>}

                {!loading && !error && (
                    <div className="bg-white rounded-xl shadow overflow-x-auto">
                        <table className="w-full text-sm">
                            <thead className="bg-gray-50 text-left text-gray-500">
                                <tr>
                                    <th className="p-3">Traveller</th>
                                    <th className="p-3">Route</th>
                                    <th className="p-3">Date</th>
                                    <th className="p-3">Flight No.</th>
                                    <th className="p-3">Depart</th>
                                    <th className="p-3">Arrive</th>
                                    <th className="p-3">Aircraft</th>
                                    <th className="p-3">Google Flights</th>
                                    <th className="p-3"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {flights.map((f) => {
                                    const isEditing = editingId === f.id;
                                    return (
                                        <tr key={f.id} className="border-t">
                                            <td className="p-3">{userLabel(f.userId)}</td>
                                            <td className="p-3">
                                                {isEditing ? (
                                                    <div className="flex gap-1">
                                                        <input
                                                            defaultValue={f.departureAirport}
                                                            onChange={(e) => setEditValues((v) => ({ ...v, departureAirport: e.target.value.toUpperCase() }))}
                                                            className="w-14 border border-gray-200 rounded p-1 text-xs"
                                                        />
                                                        <span>{"->"}</span>
                                                        <input
                                                            defaultValue={f.arrivalAirport}
                                                            onChange={(e) => setEditValues((v) => ({ ...v, arrivalAirport: e.target.value.toUpperCase() }))}
                                                            className="w-14 border border-gray-200 rounded p-1 text-xs"
                                                        />
                                                    </div>
                                                ) : (
                                                    <span>{f.departureAirport} {"->"} {f.arrivalAirport}</span>
                                                )}
                                            </td>
                                            <td className="p-3">{f.departureTime.split("T")[0]}</td>
                                            <td className="p-3">
                                                {isEditing ? (
                                                    <input
                                                        defaultValue={f.flightNumber}
                                                        onChange={(e) => setEditValues((v) => ({ ...v, flightNumber: e.target.value }))}
                                                        className="w-20 border border-gray-200 rounded p-1 text-xs"
                                                    />
                                                ) : (
                                                    <span>{f.flightNumber}</span>
                                                )}
                                            </td>
                                            <td className="p-3">{new Date(f.departureTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</td>
                                            <td className="p-3">{new Date(f.arrivalTime).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}</td>
                                            <td className="p-3">{f.aircraft || "—"}</td>
                                            <td className="p-3">

                                                <a href={googleFlightsUrl(f.departureAirport, f.arrivalAirport, f.departureTime)}
                                                    target="_blank"
                                                    rel="noopener noreferrer"
                                                    className="text-[#0f3c3c] hover:underline text-xs">

                                                    Search {"->"}

                                                </a>

                                            </td             >
                                            <td className="p-3">
                                                <div className="flex gap-2">
                                                    {isEditing ? (
                                                        <>
                                                            <button onClick={() => saveEdit(f)} className="text-green-600 text-xs hover:underline">Save</button>
                                                            <button onClick={() => setEditingId(null)} className="text-gray-500 text-xs hover:underline">Cancel</button>
                                                        </>
                                                    ) : (
                                                        <>
                                                            <button onClick={() => startEdit(f)} className="text-blue-600 text-xs hover:underline">Edit</button>
                                                            <button onClick={() => handleDelete(f.id)} className="text-red-600 text-xs hover:underline">Delete</button>
                                                        </>
                                                    )}
                                                </div>
                                            </td>
                                        </tr>
                                    );
                                })}
                                {flights.length === 0 && (
                                    <tr><td colSpan={9} className="p-4 text-center text-gray-400">No flights on file.</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </DashboardLayout >
    );
}
