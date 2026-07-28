"use client";

import { useEffect, useState, FormEvent } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import { getTripById, updateTrip } from "@/services/tripService";
import { getContactsByCity } from "@/services/directoryService";
import { getUsers } from "@/services/userService";
import { createMeeting, deleteMeeting } from "@/services/meetingService";
import { getFlights, createFlight } from "@/services/flightService";
import { Flight } from "@/types/flight";
import { TripDetail } from "@/types/trip";
import { Contact } from "@/types/directory";
import { AppUser } from "@/types/user";

import { getCities } from "@/services/directoryService";
import { getProjects } from "@/services/projectService";
import { getBusinessEntities } from "@/services/businessEntityService";
import { getHotelsByCity, createHotel } from "@/services/hotelService";
import CityAutocompleteInput from "@/components/CityAutocompleteInput";
import { createCity } from "@/services/directoryService";
import { City } from "@/types/city";
import { Project } from "@/types/project";
import { BusinessEntity } from "@/types/businessEntity";
import { Hotel } from "@/types/hotel";

const PRIORITIES = ["High", "Medium", "Low"];
const STATUSES = ["Proposed", "Requested", "Confirmed", "Tentative", "Declined", "Completed"];

interface MaterialRow {
  description: string;
  ownerId: string;
}

export default function TripDetailPage() {
  const { user } = useAuth();
  const router = useRouter();
  const params = useParams<{ tripId: string }>();

  const [trip, setTrip] = useState<TripDetail | null>(null);
  const [contacts, setContacts] = useState<Contact[]>([]);
  const [users, setUsers] = useState<AppUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [contactId, setContactId] = useState("");
  const [displayOrder, setDisplayOrder] = useState(1);
  const [priority, setPriority] = useState("Medium");
  const [status, setStatus] = useState("Proposed");
  const [scheduledTime, setScheduledTime] = useState("");
  const [agenda, setAgenda] = useState("");
  const [attendeeIds, setAttendeeIds] = useState<string[]>([]);
  const [materials, setMaterials] = useState<MaterialRow[]>([]);
  const [formError, setFormError] = useState<string | null>(null);
  const [editing, setEditing] = useState(false);
  const [cities, setCities] = useState<City[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [entities, setEntities] = useState<BusinessEntity[]>([]);
  const [hotels, setHotels] = useState<Hotel[]>([]);

  const [editCityId, setEditCityId] = useState("");
  const [editStartDate, setEditStartDate] = useState("");
  const [editEndDate, setEditEndDate] = useState("");
  const [editStatus, setEditStatus] = useState("Option");
  const [editProjectId, setEditProjectId] = useState("");
  const [editEntityId, setEditEntityId] = useState("");
  const [editHotelSelection, setEditHotelSelection] = useState("");
  const [editCustomHotel, setEditCustomHotel] = useState("");
  const [editTransport, setEditTransport] = useState("");
  const [editNotes, setEditNotes] = useState("");
  const [editTeamMemberIds, setEditTeamMemberIds] = useState<string[]>([]);
  const [editError, setEditError] = useState<string | null>(null);
  const [tripFlights, setTripFlights] = useState<Flight[]>([]);
  const [showFlightCapture, setShowFlightCapture] = useState(false);
  const [originCity, setOriginCity] = useState("");
  const [flightTravellerIds, setFlightTravellerIds] = useState<string[]>([]);
  const [flightAirline, setFlightAirline] = useState("");
  const [flightNumber, setFlightNumber] = useState("");
  const [departureAirport, setDepartureAirport] = useState("");
  const [arrivalAirport, setArrivalAirport] = useState("");
  const [departDate, setDepartDate] = useState("");
  const [departTime, setDepartTime] = useState("");
  const [arriveDate, setArriveDate] = useState("");
  const [arriveTime, setArriveTime] = useState("");
  const [flightAircraft, setFlightAircraft] = useState("");
  const [flightBookingRef, setFlightBookingRef] = useState("");
  const [flightError, setFlightError] = useState<string | null>(null);
  const [flightSubmitting, setFlightSubmitting] = useState(false);
  const [editSubmitting, setEditSubmitting] = useState(false);

  const OTHER_HOTEL = "__other__";
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!user) {
      router.push("/login");
      return;
    }
    loadAll();
  }, [user, router, params.tripId]);

  async function loadAll() {
    setLoading(true);
    setError(null);
    try {
      const tripData = await getTripById(params.tripId);
      setTrip(tripData);
      const [contactsData, usersData, citiesData, projectsData, entitiesData, hotelsData, allFlights] = await Promise.all([
        getContactsByCity(tripData.destinationCityId),
        getUsers(),
        getCities(),
        getProjects(),
        getBusinessEntities(),
        getHotelsByCity(tripData.destinationCityId),
        getFlights(),
      ]);
      setContacts(contactsData);
      setUsers(usersData);
      setCities(citiesData);
      setProjects(projectsData);
      setEntities(entitiesData);
      setHotels(hotelsData);
      setTripFlights(allFlights.filter((f) => f.tripId === tripData.id));
      setDisplayOrder((tripData.meetings?.length || 0) + 1);
    } catch {
      setError("Failed to load trip.");
    } finally {
      setLoading(false);
    }
  }
  function startEditTrip() {
    if (!trip) return;
    setEditCityId(trip.destinationCityId);
    setEditStartDate(trip.startDate);
    setEditEndDate(trip.endDate);
    setEditStatus(trip.status);
    setEditProjectId(trip.projectId || "");
    setEditEntityId(trip.businessEntityId || "");

    const matchingHotel = hotels.find(
      (h) => h.name.toLowerCase() === (trip.hotel || "").toLowerCase()
    );
    setEditHotelSelection(matchingHotel ? matchingHotel.id : "");
    setEditCustomHotel("");

    setEditTransport(trip.transport || "");
    setEditNotes(trip.notes || "");
    setEditTeamMemberIds(trip.teamMemberIds || []);
    setEditError(null);
    setEditing(true);
  }
  function toggleEditTeamMember(id: string) {
    setEditTeamMemberIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  }

  async function handleEditCityChange(cityId: string) {
    setEditCityId(cityId);
    setEditHotelSelection("");
    try {
      const hotelData = await getHotelsByCity(cityId);
      setHotels(hotelData);
    } catch {
      setHotels([]);
    }
  }

  async function handleSaveTripEdit(e: FormEvent) {
    e.preventDefault();
    if (!trip) return;
    setEditError(null);
    setEditSubmitting(true);
    try {
      let hotelName = editCustomHotel;

      if (editHotelSelection === OTHER_HOTEL) {
        if (editCustomHotel.trim() === "") {
          setEditError("Please enter a hotel name.");
          setEditSubmitting(false);
          return;
        }
        const newHotel = await createHotel({
          cityId: editCityId,
          name: editCustomHotel.trim(),
          isCustom: true,
        });
        hotelName = newHotel.name;
      } else if (editHotelSelection) {
        const chosen = hotels.find((h) => h.id === editHotelSelection);
        hotelName = chosen?.name || editCustomHotel;
      }

      await updateTrip(trip.id, {
        destinationCityId: (trip.meetings?.length || 0) > 0 ? trip.destinationCityId : editCityId,
        startDate: editStartDate,
        endDate: editEndDate,
        projectId: editProjectId || null,
        businessEntityId: editEntityId || null,
        status: editStatus,
        hotel: hotelName,
        transport: editTransport,
        notes: editNotes,
        teamMemberIds: editTeamMemberIds,
      });
      setEditing(false);
      await loadAll();
    } catch (err: any) {
      setEditError(err?.response?.data?.message || "Failed to update trip.");
    } finally {
      setEditSubmitting(false);
    }
  }

  function resetForm() {
    setContactId("");
    setPriority("Medium");
    setStatus("Proposed");
    setScheduledTime("");
    setAgenda("");
    setAttendeeIds([]);
    setMaterials([]);
    setFormError(null);
    setDisplayOrder((trip?.meetings?.length || 0) + 1);
  }

  function toggleAttendee(id: string) {
    setAttendeeIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  }

  function addMaterialRow() {
    setMaterials((prev) => [...prev, { description: "", ownerId: "" }]);
  }

  function updateMaterialRow(index: number, field: keyof MaterialRow, value: string) {
    setMaterials((prev) => prev.map((m, i) => (i === index ? { ...m, [field]: value } : m)));
  }

  function removeMaterialRow(index: number) {
    setMaterials((prev) => prev.filter((_, i) => i !== index));
  }

  function googleFlightsSearchUrl(origin: string, destinationCity: string, date: string): string {
    const query = origin
      ? `Flights from ${origin} to ${destinationCity} on ${date}`
      : `Flights to ${destinationCity} on ${date}`;
    return `https://www.google.com/travel/flights?q=${encodeURIComponent(query)}`;
  }
  function openFlightCapture() {
    if (!trip) return;
    setFlightTravellerIds(trip.teamMembers.length > 0 ? [trip.teamMembers[0].id] : []);
    setFlightAirline("");
    setFlightNumber("");
    setDepartureAirport("");
    setArrivalAirport("");
    setDepartDate(trip.startDate);
    setDepartTime("");
    setArriveDate(trip.startDate);
    setArriveTime("");
    setFlightAircraft("");
    setFlightBookingRef("");
    setFlightError(null);
    setShowFlightCapture(true);
  }
  function toggleFlightTraveller(id: string) {
    setFlightTravellerIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  }

  async function handleSaveFlight(e: FormEvent) {
    e.preventDefault();
    if (!trip) return;
    if (flightTravellerIds.length === 0) {
      setFlightError("Select at least one traveller.");
      return;
    }
    setFlightError(null);
    setFlightSubmitting(true);
    try {
      for (const uid of flightTravellerIds) {
        await createFlight({
          tripId: trip.id,
          userId: uid,
          airline: flightAirline,
          flightNumber,
          departureTime: `${departDate}T${departTime}:00`,
          arrivalTime: `${arriveDate}T${arriveTime}:00`,
          departureAirport: departureAirport.toUpperCase(),
          arrivalAirport: arrivalAirport.toUpperCase(),
          aircraft: flightAircraft,
          bookingReference: flightBookingRef,
        });
      }
      setShowFlightCapture(false);
      await loadAll();
    } catch (err: any) {
      setFlightError(err?.response?.data?.message || "Failed to save flight for one or more travellers.");
    } finally {
      setFlightSubmitting(false);
    }
  }

  async function handleCreateMeeting(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    setSubmitting(true);
    try {
      await createMeeting({
        tripId: params.tripId,
        contactId,
        displayOrder,
        priority,
        status,
        scheduledTime: scheduledTime || null,
        projectId: null,
        businessEntityId: null,
        agenda,
        attendeeIds,
        materials: materials
          .filter((m) => m.description.trim() !== "")
          .map((m) => ({ description: m.description, ownerId: m.ownerId || null })),
      });
      setShowForm(false);
      resetForm();
      await loadAll();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || "Failed to create meeting.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDeleteMeeting(id: string) {
    if (!confirm("Delete this meeting?")) return;
    try {
      await deleteMeeting(id);
      await loadAll();
    } catch {
      alert("Failed to delete meeting.");
    }
  }

  if (!user) return null;

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <Link href="/trips" className="text-sm text-[#0f3c3c] hover:underline">← Back to Trips</Link>

        {loading && <p className="text-gray-500">Loading trip...</p>}
        {error && <p className="text-red-600">{error}</p>}

        {trip && (
          <>
            <div className="bg-white rounded-xl shadow p-6">
              {!editing ? (
                <>
                  <div className="flex justify-between items-start">
                    <div>
                      <h2 className="text-2xl font-bold">{trip.destinationCity}</h2>
                      <p className="text-gray-500 text-sm">{trip.startDate} → {trip.endDate} · {trip.status}</p>
                    </div>
                    <div className="flex gap-3 items-center">
                      <button onClick={startEditTrip} className="text-sm text-[#0f3c3c] hover:underline">
                        Edit Trip
                      </button>
                      <Link href={`/one-pager/${trip.teamMemberIds[0] || ""}`} className="text-sm text-[#0f3c3c] hover:underline">
                        View One-Pager →
                      </Link>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-4 mt-4 text-sm">
                    <p><span className="text-gray-500">Project:</span> {trip.projectName || "—"}</p>
                    <p><span className="text-gray-500">Entity:</span> {trip.businessEntityName || "—"}</p>
                    <p><span className="text-gray-500">Hotel:</span> {trip.hotel || "—"}</p>
                    <p><span className="text-gray-500">Transport:</span> {trip.transport || "—"}</p>
                  </div>
                  {trip.teamMembers.length > 0 && (
                    <p className="text-sm mt-3">
                      <span className="text-gray-500">Team:</span> {trip.teamMembers.map((m) => m.name).join(", ")}
                    </p>
                  )}

                  <div className="mt-4 pt-4 border-t">
                    <label className="block text-xs text-gray-500 mb-1">From city (origin) — for search</label>
                    <div className="flex gap-2 flex-wrap items-center">
                      <input
                        type="text"
                        placeholder="e.g. New York, United States"
                        value={originCity}
                        onChange={(e) => setOriginCity(e.target.value)}
                        className="border border-gray-200 rounded-lg p-2 text-sm flex-1 min-w-[200px]"
                      />

                      <a href={googleFlightsSearchUrl(originCity, trip.destinationCity, trip.startDate)}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="inline-block bg-white border border-[#0f3c3c] text-[#0f3c3c] px-4 py-2 rounded-lg text-sm hover:bg-gray-50"
                      >
                        Search Google Flights {"->"}
                      </a>
                      <button
                        type="button"
                        onClick={openFlightCapture}
                        className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
                      >
                        + Add the flight I picked
                      </button>
                    </div>
                    <p className="text-xs text-gray-400 mt-1">
                      Search on Google Flights, then come back here and type in what you found.
                    </p>

                    {showFlightCapture && (
                      <form onSubmit={handleSaveFlight} className="bg-gray-50 rounded-lg p-4 mt-3 space-y-3">
                        {flightError && <p className="text-red-600 text-sm">{flightError}</p>}
                        <div className="grid grid-cols-2 gap-3">
                          <div className="col-span-2">
                            <label className="block text-xs text-gray-500 mb-2">Traveller(s)</label>
                            <div className="flex flex-wrap gap-3">
                              {(trip.teamMembers.length > 0 ? trip.teamMembers : users).map((u) => (
                                <label key={u.id} className="flex items-center gap-1 text-sm">
                                  <input
                                    type="checkbox"
                                    checked={flightTravellerIds.includes(u.id)}
                                    onChange={() => toggleFlightTraveller(u.id)}
                                  />
                                  {u.name}
                                </label>
                              ))}
                            </div>
                            <p className="text-xs text-gray-400 mt-1">
                              Check one person for an individual booking, or several if they're all on the same flight.
                            </p>
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Airline</label>
                            <input required value={flightAirline} onChange={(e) => setFlightAirline(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Flight Number</label>
                            <input required value={flightNumber} onChange={(e) => setFlightNumber(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Aircraft</label>
                            <input value={flightAircraft} onChange={(e) => setFlightAircraft(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Departure Airport (code)</label>
                            <input required maxLength={3} value={departureAirport} onChange={(e) => setDepartureAirport(e.target.value.toUpperCase())} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Arrival Airport (code)</label>
                            <input required maxLength={3} value={arrivalAirport} onChange={(e) => setArrivalAirport(e.target.value.toUpperCase())} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Depart Date</label>
                            <input type="date" required value={departDate} onChange={(e) => setDepartDate(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Depart Time</label>
                            <input type="time" required value={departTime} onChange={(e) => setDepartTime(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Arrive Date</label>
                            <input type="date" required value={arriveDate} onChange={(e) => setArriveDate(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div>
                            <label className="block text-xs text-gray-500 mb-1">Arrive Time</label>
                            <input type="time" required value={arriveTime} onChange={(e) => setArriveTime(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                          <div className="col-span-2">
                            <label className="block text-xs text-gray-500 mb-1">Booking Reference</label>
                            <input value={flightBookingRef} onChange={(e) => setFlightBookingRef(e.target.value)} className="w-full border border-gray-200 rounded p-2 text-sm" />
                          </div>
                        </div>
                        <div className="flex gap-3">
                          <button type="submit" disabled={flightSubmitting} className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60">
                            {flightSubmitting ? "Saving..." : "Save & Add to Trip"}
                          </button>
                          <button type="button" onClick={() => setShowFlightCapture(false)} className="bg-white border border-gray-300 px-4 py-2 rounded-lg text-sm">
                            Cancel
                          </button>
                        </div>
                      </form>
                    )}

                    {tripFlights.length > 0 && (
                      <div className="mt-4">
                        <p className="text-xs text-gray-500 mb-2">Flights on this trip:</p>
                        <div className="space-y-1">
                          {tripFlights.map((f) => (
                            <div key={f.id} className="text-sm bg-gray-50 rounded p-2 flex justify-between">
                              <span>{f.airline} {f.flightNumber} — {f.departureAirport} → {f.arrivalAirport}</span>
                              <span className="text-gray-500">{f.departureTime.split("T")[0]}</span>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                </>
              ) : (
                <form onSubmit={handleSaveTripEdit} className="space-y-4">
                  {editError && <p className="text-red-600 text-sm">{editError}</p>}

                  <div>
                    <label className="block text-sm font-medium mb-1">Destination City</label>
                    {(trip.meetings?.length || 0) > 0 ? (
                      <>
                        <input
                          disabled
                          value={`${trip.destinationCity}`}
                          className="w-full border border-gray-200 rounded-lg p-2 text-sm bg-gray-100 text-gray-500"
                        />
                        <p className="text-xs text-amber-600 mt-1">
                          Destination can't be changed once this trip has meetings — {trip.meetings.length} meeting
                          {trip.meetings.length > 1 ? "s" : ""} already reference contacts in {trip.destinationCity}.
                          Delete the meetings first if you need to change the city.
                        </p>
                      </>
                    ) : (
                      <>
                        <CityAutocompleteInput
                          onSelect={async (picked) => {
                            let match = cities.find(
                              (c) => c.name.toLowerCase() === picked.name.toLowerCase() &&
                                c.country.toLowerCase() === picked.country.toLowerCase()
                            );
                            if (!match) {
                              try {
                                match = await createCity({ name: picked.name, country: picked.country });
                                setCities((prev) => [...prev, match!]);
                              } catch {
                                setEditError("Failed to create city.");
                                return;
                              }
                            }
                            await handleEditCityChange(match.id);
                          }}
                        />
                        <p className="text-xs text-gray-500 mt-1">
                          Current: {cities.find((c) => c.id === editCityId)?.name || trip.destinationCity}
                        </p>
                      </>
                    )}
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium mb-1">Status</label>
                      <select value={editStatus} onChange={(e) => setEditStatus(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                        <option value="Confirmed">Confirmed</option>
                        <option value="Option">Option</option>
                        <option value="Tentative">Tentative</option>
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium mb-1">Project</label>
                      <select value={editProjectId} onChange={(e) => setEditProjectId(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                        <option value="">— None —</option>
                        {projects.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium mb-1">Entity</label>
                      <select value={editEntityId} onChange={(e) => setEditEntityId(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                        <option value="">— None —</option>
                        {entities.map((ent) => <option key={ent.id} value={ent.id}>{ent.name}</option>)}
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium mb-1">Start Date</label>
                      <input type="date" required value={editStartDate} onChange={(e) => setEditStartDate(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                    </div>

                    <div>
                      <label className="block text-sm font-medium mb-1">End Date</label>
                      <input type="date" required value={editEndDate} onChange={(e) => setEditEndDate(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                    </div>

                    <div>
                      <label className="block text-sm font-medium mb-1">Hotel</label>
                      <select value={editHotelSelection} onChange={(e) => setEditHotelSelection(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                        <option value="">— None —</option>
                        {hotels.map((h) => <option key={h.id} value={h.id}>{h.name}</option>)}
                        <option value={OTHER_HOTEL}>Other (add new)...</option>
                      </select>
                      {editHotelSelection === OTHER_HOTEL && (
                        <input
                          type="text" placeholder="New hotel name" value={editCustomHotel}
                          onChange={(e) => setEditCustomHotel(e.target.value)}
                          className="w-full border border-gray-200 rounded-lg p-2 text-sm mt-2"
                        />
                      )}
                    </div>

                    <div>
                      <label className="block text-sm font-medium mb-1">Transport</label>
                      <input type="text" value={editTransport} onChange={(e) => setEditTransport(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-1">Notes</label>
                    <textarea value={editNotes} onChange={(e) => setEditNotes(e.target.value)} rows={2} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-2">Team Members</label>
                    <div className="flex flex-wrap gap-3">
                      {users.map((u) => (
                        <label key={u.id} className="flex items-center gap-1 text-sm">
                          <input type="checkbox" checked={editTeamMemberIds.includes(u.id)} onChange={() => toggleEditTeamMember(u.id)} />
                          {u.name}
                        </label>
                      ))}
                    </div>
                  </div>

                  <div className="flex gap-3">
                    <button type="submit" disabled={editSubmitting} className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60">
                      {editSubmitting ? "Saving..." : "Save Changes"}
                    </button>
                    <button type="button" onClick={() => setEditing(false)} className="bg-white border border-gray-300 px-4 py-2 rounded-lg text-sm">
                      Cancel
                    </button>
                  </div>
                </form>
              )}
            </div>

            <div className="flex justify-between items-center">
              <h3 className="text-xl font-semibold">Meetings</h3>
              <button
                onClick={() => { setShowForm((s) => !s); if (!showForm) resetForm(); }}
                className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
              >
                {showForm ? "Cancel" : "+ New Meeting"}
              </button>
            </div>

            {showForm && (
              <form onSubmit={handleCreateMeeting} className="bg-white rounded-xl shadow p-6 space-y-4">
                {formError && <p className="text-red-600 text-sm">{formError}</p>}

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium mb-1">Contact</label>
                    <select required value={contactId} onChange={(e) => setContactId(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                      <option value="">Select a contact</option>
                      {contacts.map((c) => (
                        <option key={c.id} value={c.id}>{c.name}{c.organization && ` — ${c.organization}`}</option>
                      ))}
                    </select>
                    {contacts.length === 0 && (
                      <p className="text-xs text-gray-400 mt-1">
                        No contacts for this city yet — add some in the Directory first.
                      </p>
                    )}
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-1">Order</label>
                    <input type="number" min={1} required value={displayOrder} onChange={(e) => setDisplayOrder(Number(e.target.value))} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-1">Priority</label>
                    <select value={priority} onChange={(e) => setPriority(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                      {PRIORITIES.map((p) => <option key={p} value={p}>{p}</option>)}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-1">Status</label>
                    <select value={status} onChange={(e) => setStatus(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm">
                      {STATUSES.map((s) => <option key={s} value={s}>{s}</option>)}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium mb-1">Time</label>
                    <input type="time" value={scheduledTime} onChange={(e) => setScheduledTime(e.target.value)} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium mb-1">Agenda</label>
                  <textarea value={agenda} onChange={(e) => setAgenda(e.target.value)} rows={2} className="w-full border border-gray-200 rounded-lg p-2 text-sm" />
                </div>

                <div>
                  <label className="block text-sm font-medium mb-2">Attending Team</label>
                  <div className="flex flex-wrap gap-3">
                    {(trip.teamMembers.length > 0 ? trip.teamMembers : users).map((u) => (
                      <label key={u.id} className="flex items-center gap-1 text-sm">
                        <input type="checkbox" checked={attendeeIds.includes(u.id)} onChange={() => toggleAttendee(u.id)} />
                        {u.name}
                      </label>
                    ))}
                  </div>
                </div>

                <div>
                  <div className="flex justify-between items-center mb-2">
                    <label className="block text-sm font-medium">Materials</label>
                    <button type="button" onClick={addMaterialRow} className="text-xs text-[#0f3c3c] hover:underline">+ Add material</button>
                  </div>
                  <div className="space-y-2">
                    {materials.map((m, i) => (
                      <div key={i} className="flex gap-2 items-center">
                        <input
                          placeholder="Description"
                          value={m.description}
                          onChange={(e) => updateMaterialRow(i, "description", e.target.value)}
                          className="flex-1 border border-gray-200 rounded-lg p-2 text-sm"
                        />
                        <select
                          value={m.ownerId}
                          onChange={(e) => updateMaterialRow(i, "ownerId", e.target.value)}
                          className="border border-gray-200 rounded-lg p-2 text-sm"
                        >
                          <option value="">No owner</option>
                          {(trip.teamMembers.length > 0 ? trip.teamMembers : users).map((u) => <option key={u.id} value={u.id}>{u.name}</option>)}
                        </select>
                        <button type="button" onClick={() => removeMaterialRow(i)} className="text-red-500 text-xs">✕</button>
                      </div>
                    ))}
                  </div>
                </div>

                <button type="submit" disabled={submitting} className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60">
                  {submitting ? "Saving..." : "Create Meeting"}
                </button>
              </form>
            )}

            <div className="space-y-3">
              {(trip.meetings || []).map((m) => (
                <div key={m.id} className="bg-white rounded-xl shadow p-5">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="font-medium">#{m.displayOrder} — {m.contactName}</p>
                      <p className="text-xs text-gray-500">
                        {m.scheduledTime && `${m.scheduledTime} · `}{m.priority} priority · {m.status}
                      </p>
                    </div>
                    <button onClick={() => handleDeleteMeeting(m.id)} className="text-red-600 text-xs hover:underline">Delete</button>
                  </div>
                  <p className="text-sm mt-2">{m.agenda}</p>
                  {m.attendees.length > 0 && (
                    <p className="text-xs text-gray-500 mt-2">Team: {m.attendees.map((a) => a.name).join(", ")}</p>
                  )}
                  {m.materials.length > 0 && (
                    <ul className="text-xs text-gray-600 mt-2 list-disc pl-4">
                      {m.materials.map((mat) => (
                        <li key={mat.id}>{mat.description}{mat.ownerName && ` — ${mat.ownerName}`}</li>
                      ))}
                    </ul>
                  )}
                </div>
              ))}
              {(trip.meetings || []).length === 0 && (
                <p className="text-gray-400 text-center py-6">No meetings scheduled for this trip yet.</p>
              )}
            </div>
          </>
        )}
      </div>
    </DashboardLayout>
  );
}