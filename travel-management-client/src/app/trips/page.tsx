"use client";

import { useEffect, useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import { getTrips, createTrip, deleteTrip } from "@/services/tripService";
import { getCities } from "@/services/directoryService";
import { getUsers } from "@/services/userService";
import { getProjects } from "@/services/projectService";
import { getBusinessEntities } from "@/services/businessEntityService";
import { getHotelsByCity, createHotel } from "@/services/hotelService";
import CityAutocompleteInput from "@/components/CityAutocompleteInput";
import { createCity } from "@/services/directoryService";
import { Trip } from "@/types/trip";
import { City } from "@/types/city";
import { AppUser } from "@/types/user";
import { Project } from "@/types/project";
import { BusinessEntity } from "@/types/businessEntity";
import { Hotel } from "@/types/hotel";
import { usePolling } from "@/hooks/usePolling";
import { bulkCreateTrips } from "@/services/tripService";
import { BulkTripLeg } from "@/types/trip";
const OTHER_HOTEL = "__other__";

export default function TripsPage() {
  const { user } = useAuth();
  const router = useRouter();

  const [trips, setTrips] = useState<Trip[]>([]);
  const [cities, setCities] = useState<City[]>([]);
  const [users, setUsers] = useState<AppUser[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [entities, setEntities] = useState<BusinessEntity[]>([]);
  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);

  const [destinationCityId, setDestinationCityId] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [status, setStatus] = useState("Option");
  const [projectId, setProjectId] = useState("");
  const [businessEntityId, setBusinessEntityId] = useState("");
  const [hotelSelection, setHotelSelection] = useState("");
  const [customHotelName, setCustomHotelName] = useState("");
  const [transport, setTransport] = useState("");
  const [notes, setNotes] = useState("");
  const [teamMemberIds, setTeamMemberIds] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [mode, setMode] = useState<"none" | "single" | "bulk">("none");
  const [bulkLegs, setBulkLegs] = useState<BulkTripLeg[]>([
    { destinationCityId: "", startDate: "", endDate: "", projectId: null, businessEntityId: null, status: "Option" },
  ]);
  const [bulkError, setBulkError] = useState<string | null>(null);
  const [bulkSubmitting, setBulkSubmitting] = useState(false);

  useEffect(() => {
    if (!user) {
      router.push("/login");
      return;
    }
    loadAll();
  }, [user, router]);

  async function loadAll(silent = false) {
    if (!silent) {
      setLoading(true);
      setError(null);
    }
    try {
      const [tripsData, citiesData, usersData, projectsData, entitiesData] = await Promise.all([
        getTrips(),
        getCities(),
        getUsers(),
        getProjects(),
        getBusinessEntities(),
      ]);
      setTrips(tripsData);
      setCities(citiesData);
      setUsers(usersData);
      setProjects(projectsData);
      setEntities(entitiesData);
    } catch {
      if (!silent) setError("Failed to load trips.");
    } finally {
      if (!silent) setLoading(false);
    }
  }

  usePolling(() => {
    if (!showForm) loadAll(true);
  }, 30000);
  async function handleCityChange(cityId: string) {
    setDestinationCityId(cityId);
    setHotelSelection("");
    setCustomHotelName("");
    if (!cityId) {
      setHotels([]);
      return;
    }
    try {
      const hotelData = await getHotelsByCity(cityId);
      setHotels(hotelData);
    } catch {
      setHotels([]);
    }
  }
  function addBulkLeg() {
    setBulkLegs((prev) => [
      ...prev,
      { destinationCityId: "", startDate: "", endDate: "", projectId: null, businessEntityId: null, status: "Option" },
    ]);
  }

  function updateBulkLeg(index: number, field: keyof BulkTripLeg, value: string) {
    setBulkLegs((prev) =>
      prev.map((leg, i) =>
        i === index
          ? { ...leg, [field]: field === "projectId" || field === "businessEntityId" ? (value || null) : value }
          : leg
      )
    );
  }

  function removeBulkLeg(index: number) {
    setBulkLegs((prev) => prev.filter((_, i) => i !== index));
  }

  async function handleBulkCreate(e: FormEvent) {
    e.preventDefault();
    setBulkError(null);
    setBulkSubmitting(true);
    try {
      await bulkCreateTrips({ trips: bulkLegs });
      setMode("none");
      setBulkLegs([{ destinationCityId: "", startDate: "", endDate: "", projectId: null, businessEntityId: null, status: "Option" }]);
      await loadAll();
    } catch (err: any) {
      setBulkError(err?.response?.data?.message || "Failed to create trip legs.");
    } finally {
      setBulkSubmitting(false);
    }
  }

  function toggleTeamMember(id: string) {
    setTeamMemberIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  }

  function resetForm() {
    setDestinationCityId("");
    setStartDate("");
    setEndDate("");
    setStatus("Option");
    setProjectId("");
    setBusinessEntityId("");
    setHotelSelection("");
    setCustomHotelName("");
    setHotels([]);
    setTransport("");
    setNotes("");
    const ceo = users.find((u) => u.isCeo);
    setTeamMemberIds(ceo ? [ceo.id] : []);
    setFormError(null);
  }
  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    setSubmitting(true);
    try {
      let hotelName = "";

      if (hotelSelection === OTHER_HOTEL) {
        if (customHotelName.trim() === "") {
          setFormError("Please enter a hotel name.");
          setSubmitting(false);
          return;
        }
        const newHotel = await createHotel({
          cityId: destinationCityId,
          name: customHotelName.trim(),
          isCustom: true,
        });
        hotelName = newHotel.name;
      } else if (hotelSelection) {
        const chosen = hotels.find((h) => h.id === hotelSelection);
        hotelName = chosen?.name || "";
      }

      await createTrip({
        destinationCityId,
        startDate,
        endDate,
        projectId: projectId || null,
        businessEntityId: businessEntityId || null,
        status,
        hotel: hotelName,
        transport,
        notes,
        teamMemberIds,
      });
      setShowForm(false);
      resetForm();
      await loadAll();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || "Failed to create trip.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(id: string) {
    if (!confirm("Delete this trip?")) return;
    try {
      await deleteTrip(id);
      await loadAll();
    } catch {
      alert("Failed to delete trip.");
    }
  }

  if (!user) return null;

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h2 className="text-2xl font-bold">Trips</h2>
          <div className="flex gap-2">
            <button
              onClick={() => { setMode(mode === "single" ? "none" : "single"); setShowForm(mode !== "single"); if (mode !== "single") resetForm(); }}
              className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
            >
              {mode === "single" ? "Cancel" : "+ New Trip"}
            </button>
            <button
              onClick={() => setMode(mode === "bulk" ? "none" : "bulk")}
              className="bg-white border border-[#0f3c3c] text-[#0f3c3c] px-4 py-2 rounded-lg text-sm"
            >
              {mode === "bulk" ? "Cancel" : "+ Bulk Add Legs"}
            </button>
          </div>
        </div>

        {mode === "single" && (
          <form onSubmit={handleCreate} className="bg-white rounded-xl shadow p-6 space-y-4">
            {formError && <p className="text-red-600 text-sm">{formError}</p>}

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Destination City</label>
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
                        setFormError("Failed to create city.");
                        return;
                      }
                    }
                    await handleCityChange(match.id);
                  }}
                />
                {destinationCityId && (
                  <p className="text-xs text-gray-500 mt-1">
                    Selected: {cities.find((c) => c.id === destinationCityId)?.name}, {cities.find((c) => c.id === destinationCityId)?.country}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Status</label>
                <select
                  value={status}
                  onChange={(e) => setStatus(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                >
                  <option value="Confirmed">Confirmed</option>
                  <option value="Option">Option</option>
                  <option value="Tentative">Tentative</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Project</label>
                <select
                  value={projectId}
                  onChange={(e) => setProjectId(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                >
                  <option value="">— None —</option>
                  {projects.map((p) => (
                    <option key={p.id} value={p.id}>{p.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Entity</label>
                <select
                  value={businessEntityId}
                  onChange={(e) => setBusinessEntityId(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                >
                  <option value="">— None —</option>
                  {entities.map((e) => (
                    <option key={e.id} value={e.id}>{e.name}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Start Date</label>
                <input
                  type="date"
                  required
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">End Date</label>
                <input
                  type="date"
                  required
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Hotel</label>
                <select
                  value={hotelSelection}
                  onChange={(e) => setHotelSelection(e.target.value)}
                  disabled={!destinationCityId}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm disabled:bg-gray-100"
                >
                  <option value="">
                    {destinationCityId ? "— None —" : "Select a city first"}
                  </option>
                  {hotels.map((h) => (
                    <option key={h.id} value={h.id}>{h.name}</option>
                  ))}
                  {destinationCityId && <option value={OTHER_HOTEL}>Other (add new)...</option>}
                </select>
                {hotelSelection === OTHER_HOTEL && (
                  <input
                    type="text"
                    placeholder="New hotel name"
                    value={customHotelName}
                    onChange={(e) => setCustomHotelName(e.target.value)}
                    className="w-full border border-gray-200 rounded-lg p-2 text-sm mt-2"
                  />
                )}
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Transport</label>
                <input
                  type="text"
                  value={transport}
                  onChange={(e) => setTransport(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Notes</label>
              <textarea
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                rows={2}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">
                Team Members
              </label>
              <div className="flex flex-wrap gap-3">
                {users.map((u) => (
                  <label key={u.id} className="flex items-center gap-1 text-sm">
                    <input
                      type="checkbox"
                      checked={teamMemberIds.includes(u.id)}
                      onChange={() => toggleTeamMember(u.id)}
                    />
                    {u.name}{u.isCeo && <span className="text-xs text-[#0f3c3c] ml-1">(CEO)</span>}
                  </label>
                ))}
              </div>
              {!teamMemberIds.includes(users.find((u) => u.isCeo)?.id || "") && (
                <p className="text-xs text-amber-600 mt-2">
                  This trip doesn't include the CEO — confirm this is intentional (e.g. a delegation trip).
                </p>
              )}
            </div>
            <button
              type="submit"
              disabled={submitting}
              className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60"
            >
              {submitting ? "Creating..." : "Create Trip"}
            </button>
          </form>
        )}
        {mode === "bulk" && (
          <form onSubmit={handleBulkCreate} className="bg-white rounded-xl shadow p-6 space-y-4">
            {bulkError && <p className="text-red-600 text-sm">{bulkError}</p>}
            <p className="text-sm text-gray-500">Add multiple trip legs at once — useful for multi-city itineraries.</p>

            <div className="space-y-3">
              {bulkLegs.map((leg, i) => (
                <div key={i} className="grid grid-cols-6 gap-2 items-end bg-gray-50 p-3 rounded-lg">
                  <div className="col-span-2">
                    <label className="block text-xs text-gray-500 mb-1">City</label>
                    <select
                      required
                      value={leg.destinationCityId}
                      onChange={(e) => updateBulkLeg(i, "destinationCityId", e.target.value)}
                      className="w-full border border-gray-200 rounded p-1.5 text-sm"
                    >
                      <option value="">Select</option>
                      {cities.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                    </select>
                  </div>
                  <div>
                    <label className="block text-xs text-gray-500 mb-1">From</label>
                    <input
                      type="date" required value={leg.startDate}
                      onChange={(e) => updateBulkLeg(i, "startDate", e.target.value)}
                      className="w-full border border-gray-200 rounded p-1.5 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-xs text-gray-500 mb-1">To</label>
                    <input
                      type="date" required value={leg.endDate}
                      onChange={(e) => updateBulkLeg(i, "endDate", e.target.value)}
                      className="w-full border border-gray-200 rounded p-1.5 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-xs text-gray-500 mb-1">Project</label>
                    <select
                      value={leg.projectId || ""}
                      onChange={(e) => updateBulkLeg(i, "projectId", e.target.value)}
                      className="w-full border border-gray-200 rounded p-1.5 text-sm"
                    >
                      <option value="">—</option>
                      {projects.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
                    </select>
                  </div>
                  <div className="flex gap-1">
                    <div className="flex-1">
                      <label className="block text-xs text-gray-500 mb-1">Status</label>
                      <select
                        value={leg.status}
                        onChange={(e) => updateBulkLeg(i, "status", e.target.value)}
                        className="w-full border border-gray-200 rounded p-1.5 text-sm"
                      >
                        <option value="Confirmed">Confirmed</option>
                        <option value="Option">Option</option>
                        <option value="Tentative">Tentative</option>
                      </select>
                    </div>
                    {bulkLegs.length > 1 && (
                      <button type="button" onClick={() => removeBulkLeg(i)} className="text-red-500 text-xs px-1">✕</button>
                    )}
                  </div>
                </div>
              ))}
            </div>

            <div className="flex justify-between items-center">
              <button type="button" onClick={addBulkLeg} className="text-sm text-[#0f3c3c] hover:underline">
                + Add another leg
              </button>
              <button
                type="submit"
                disabled={bulkSubmitting}
                className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60"
              >
                {bulkSubmitting ? "Creating..." : `Create ${bulkLegs.length} Trip${bulkLegs.length > 1 ? "s" : ""}`}
              </button>
            </div>
          </form>
        )}

        {loading && <p className="text-gray-500">Loading trips...</p>}
        {error && <p className="text-red-600">{error}</p>}

        {!loading && !error && (
          <div className="bg-white rounded-xl shadow overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 text-left text-gray-500">
                <tr>
                  <th className="p-3">Destination</th>
                  <th className="p-3">Dates</th>
                  <th className="p-3">Status</th>
                  <th className="p-3">Project</th>
                  <th className="p-3">Entity</th>
                  <th className="p-3">Hotel</th>
                  <th className="p-3">Team</th>
                  <th className="p-3"></th>
                </tr>
              </thead>
              <tbody>
                {trips.map((t) => (
                  <tr key={t.id} className="border-t">
                    <td className="p-3 font-medium">
                      <Link href={`/trips/${t.id}`} className="text-[#0f3c3c] hover:underline">
                        {t.destinationCity}
                      </Link>
                    </td>
                    <td className="p-3">{t.startDate} → {t.endDate}</td>
                    <td className="p-3">{t.status}</td>
                    <td className="p-3">{t.projectName || "—"}</td>
                    <td className="p-3">{t.businessEntityName || "—"}</td>
                    <td className="p-3">{t.hotel || "—"}</td>
                    <td className="p-3">{t.teamMemberIds.length}</td>
                    <td className="p-3">
                      <button
                        onClick={() => handleDelete(t.id)}
                        className="text-red-600 text-sm hover:underline"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
                {trips.length === 0 && (
                  <tr>
                    <td colSpan={8} className="p-4 text-center text-gray-400">
                      No trips yet.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}