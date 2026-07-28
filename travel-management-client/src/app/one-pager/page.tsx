"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import { getUsers } from "@/services/userService";
import { AppUser } from "@/types/user";

export default function OnePagerPickerPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [users, setUsers] = useState<AppUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user) {
      router.push("/login");
      return;
    }
    getUsers()
      .then(setUsers)
      .catch(() => setError("Failed to load people."))
      .finally(() => setLoading(false));
  }, [user, router]);

  if (!user) return null;

  return (
    <DashboardLayout>
      <div className="space-y-4">
        <h2 className="text-2xl font-bold">One-Pagers</h2>
        <p className="text-gray-600">Select a person to generate their printable briefing.</p>

        {loading && <p className="text-gray-500">Loading...</p>}
        {error && <p className="text-red-600">{error}</p>}

        {!loading && !error && (
          <div className="bg-white rounded-xl shadow divide-y">
            {users.map((u) => (
              <Link
                key={u.id}
                href={`/one-pager/${u.id}`}
                className="flex justify-between items-center p-4 hover:bg-gray-50"
              >
                <div>
                  <p className="font-medium">{u.name}</p>
                  <p className="text-sm text-gray-500">{u.title} · {u.function}</p>
                </div>
                <span className="text-[#0f3c3c] text-sm">View →</span>
              </Link>
            ))}
          </div>
        )}
      </div>
    </DashboardLayout>
  );
}