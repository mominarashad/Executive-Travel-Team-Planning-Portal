"use client";

import { useEffect, useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import {
  getTeamPlans,
  createTeamPlan,
  updateTeamPlan,
  deleteTeamPlan,
  bulkCreateTeamPlans,
} from "@/services/teamPlanService";
import { getCities } from "@/services/directoryService";
import { getUsers } from "@/services/userService";
import { TeamPlanEntry } from "@/types/teamPlan";
import { City } from "@/types/city";
import { AppUser } from "@/types/user";
import { usePolling } from "@/hooks/usePolling";
const TYPES = ["Trip", "Option", "Vacation", "Remote"];
const APPROVALS = ["", "Pending", "Approved", "Rejected"];

export default function TeamPlanPage() {
  const { user } = useAuth();
  const router = useRouter();

  const [entries, setEntries] = useState<TeamPlanEntry[]>([]);
  const [cities, setCities] = useState<City[]>([]);
  const [users, setUsers] = useState<AppUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [mode, setMode] = useState<"none" | "single" | "bulk">("none");
  const [formError, setFormError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  // shared form fields
  const [userId, setUserId] = useState("");
  const [bulkUserIds, setBulkUserIds] = useState<string[]>([]);
  const [cityId, setCityId] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [type, setType] = useState("Trip");
  const [notes, setNotes] = useState("");

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
      const [entriesData, citiesData, usersData] = await Promise.all([
        getTeamPlans(),
        getCities(),
        getUsers(),
      ]);
      setEntries(entriesData);
      setCities(citiesData);
      setUsers(usersData);
    } catch {
      if (!silent) setError("Failed to load team plan data.");
    } finally {
      if (!silent) setLoading(false);
    }
  }

  usePolling(() => {
    if (mode === "none") loadAll(true);
  }, 30000);
  function resetForm() {
    setUserId("");
    setBulkUserIds([]);
    setCityId("");
    setFromDate("");
    setToDate("");
    setType("Trip");
    setNotes("");
    setFormError(null);
  }

  function toggleBulkUser(id: string) {
    setBulkUserIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  }

  async function handleCreateSingle(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    setSubmitting(true);
    try {
      await createTeamPlan({
        userId,
        cityId: cityId || null,
        fromDate,
        toDate,
        type,
        approvalStatus: type === "Vacation" ? "Pending" : "",
        notes,
      });
      setMode("none");
      resetForm();
      await loadAll();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || "Failed to create entry.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleCreateBulk(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    setSubmitting(true);
    try {
      await bulkCreateTeamPlans({
        userIds: bulkUserIds,
        cityId: cityId || null,
        fromDate,
        toDate,
        type,
        approvalStatus: type === "Vacation" ? "Pending" : "",
        notes,
      });
      setMode("none");
      resetForm();
      await loadAll();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || "Failed to create bulk entries.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleApprovalChange(entry: TeamPlanEntry, newStatus: string) {
    try {
      await updateTeamPlan(entry.id, {
        userId: entry.userId,
        cityId: entry.cityId,
        fromDate: entry.fromDate,
        toDate: entry.toDate,
        type: entry.type,
        approvalStatus: newStatus,
        notes: entry.notes,
      });
      await loadAll();
    } catch {
      alert("Failed to update approval status.");
    }
  }

  async function handleDelete(id: string) {
    if (!confirm("Delete this entry?")) return;
    try {
      await deleteTeamPlan(id);
      await loadAll();
    } catch {
      alert("Failed to delete entry.");
    }
  }

  if (!user) return null;

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex justify-between items-center flex-wrap gap-2">
          <h2 className="text-2xl font-bold">Team Plan</h2>
          <div className="flex gap-2">
            <button
              onClick={() => { setMode(mode === "single" ? "none" : "single"); resetForm(); }}
              className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
            >
              {mode === "single" ? "Cancel" : "+ New Entry"}
            </button>
            <button
              onClick={() => { setMode(mode === "bulk" ? "none" : "bulk"); resetForm(); }}
              className="bg-white border border-[#0f3c3c] text-[#0f3c3c] px-4 py-2 rounded-lg text-sm"
            >
              {mode === "bulk" ? "Cancel" : "+ Bulk Add"}
            </button>
          </div>
        </div>

        {(mode === "single" || mode === "bulk") && (
          <form
            onSubmit={mode === "single" ? handleCreateSingle : handleCreateBulk}
            className="bg-white rounded-xl shadow p-6 space-y-4"
          >
            {formError && <p className="text-red-600 text-sm">{formError}</p>}

            {mode === "single" ? (
              <div>
                <label className="block text-sm font-medium mb-1">Person</label>
                <select
                  required
                  value={userId}
                  onChange={(e) => setUserId(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                >
                  <option value="">Select a person</option>
                  {users.map((u) => (
                    <option key={u.id} value={u.id}>{u.name}</option>
                  ))}
                </select>
              </div>
            ) : (
              <div>
                <label className="block text-sm font-medium mb-2">People</label>
                <div className="flex flex-wrap gap-3">
                  {users.map((u) => (
                    <label key={u.id} className="flex items-center gap-1 text-sm">
                      <input
                        type="checkbox"
                        checked={bulkUserIds.includes(u.id)}
                        onChange={() => toggleBulkUser(u.id)}
                      />
                      {u.name}
                    </label>
                  ))}
                </div>
              </div>
            )}

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">City (optional)</label>
                <select
                  value={cityId}
                  onChange={(e) => setCityId(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                >
                  <option value="">— None —</option>
                  {cities.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}, {c.country}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Type</label>
                <select
                  value={type}
                  onChange={(e) => setType(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                >
                  {TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">From Date</label>
                <input
                  type="date" required value={fromDate}
                  onChange={(e) => setFromDate(e.target.value)}
                  className="w-full border border-gray-200 rounded-lg p-2 text-sm"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">To Date</label>
                <input
                  type="date" required value={toDate}
                  onChange={(e) => setToDate(e.target.value)}
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

            <button
              type="submit"
              disabled={submitting}
              className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60"
            >
              {submitting ? "Saving..." : mode === "single" ? "Add Entry" : "Add to Selected People"}
            </button>
          </form>
        )}

        {loading && <p className="text-gray-500">Loading team plan...</p>}
        {error && <p className="text-red-600">{error}</p>}

        {!loading && !error && (
          <div className="bg-white rounded-xl shadow overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 text-left text-gray-500">
                <tr>
                  <th className="p-3">Person</th>
                  <th className="p-3">City</th>
                  <th className="p-3">Dates</th>
                  <th className="p-3">Type</th>
                  <th className="p-3">Approval</th>
                  <th className="p-3">Notes</th>
                  <th className="p-3"></th>
                </tr>
              </thead>
              <tbody>
                {entries.map((e) => (
                  <tr key={e.id} className="border-t">
                    <td className="p-3 font-medium">{e.userName}</td>
                    <td className="p-3">{e.cityName || "TBC"}</td>
                    <td className="p-3">{e.fromDate} → {e.toDate}</td>
                    <td className="p-3">{e.type}</td>
                    <td className="p-3">
                      {e.type === "Vacation" ? (
                        <select
                          value={e.approvalStatus}
                          onChange={(ev) => handleApprovalChange(e, ev.target.value)}
                          className="border border-gray-200 rounded p-1 text-xs"
                        >
                          {APPROVALS.filter((a) => a !== "").map((a) => (
                            <option key={a} value={a}>{a}</option>
                          ))}
                        </select>
                      ) : (
                        <span className="text-gray-400">—</span>
                      )}
                    </td>
                    <td className="p-3 text-gray-600">{e.notes || "—"}</td>
                    <td className="p-3">
                      <button
                        onClick={() => handleDelete(e.id)}
                        className="text-red-600 text-sm hover:underline"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
                {entries.length === 0 && (
                  <tr>
                    <td colSpan={7} className="p-4 text-center text-gray-400">No entries yet.</td>
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