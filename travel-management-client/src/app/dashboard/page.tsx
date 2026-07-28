"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import StatCard from "@/components/dashboard/StatCard";
import { getDashboard } from "@/services/dashboardService";
import { DashboardData } from "@/types/dashboard";
import { usePolling } from "@/hooks/usePolling";
export default function DashboardPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      router.push("/login");
      return;
    }

    getDashboard()
      .then(setData)
      .catch(() => setError("Failed to load dashboard data."))
      .finally(() => setLoading(false));
  }, [user, router]);

  usePolling(() => {
    if (user) getDashboard().then(setData).catch(() => {});
  }, 30000);
  if (!user) return null;

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="bg-white p-6 rounded-xl shadow">
          <h2 className="text-2xl font-bold">Welcome, {user.name}</h2>
          <p className="text-gray-600 mt-2">
            Manage your trips, meetings, flights and users from one place.
          </p>
        </div>

        {loading && <p className="text-gray-500">Loading dashboard...</p>}
        {error && <p className="text-red-600">{error}</p>}

        {data && (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-6">
              <StatCard title="Upcoming Trips" value={data.upcomingTripsCount} />
              <StatCard title="Travel Days (This Year)" value={data.totalTravelDaysThisYear} />
              <StatCard title="Upcoming Meetings" value={data.upcomingMeetingsCount} />
              <StatCard title="Travelers This Week" value={data.travelersThisWeekCount} />
            </div>

            <div className="bg-white rounded-xl shadow p-6">
              <h3 className="text-gray-500 text-sm mb-2">Next Departure</h3>
              {data.nextDeparture ? (
                <p className="text-lg">
                  <strong>{data.nextDeparture.city}</strong> —{" "}
                  {data.nextDeparture.startDate} to {data.nextDeparture.endDate}{" "}
                  ({data.nextDeparture.daysUntil} days away, {data.nextDeparture.status})
                </p>
              ) : (
                <p className="text-gray-500">No upcoming trips scheduled.</p>
              )}
            </div>

            {data.tripsNeedingAttentionCount > 0 && (
              <div className="bg-yellow-50 border border-yellow-200 rounded-xl p-4 text-yellow-800">
                {data.tripsNeedingAttentionCount} upcoming trip(s) are missing hotel or transport details.
              </div>
            )}
          </>
        )}
      </div>
    </DashboardLayout>
  );
}