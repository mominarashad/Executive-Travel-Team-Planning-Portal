"use client";

import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";

export default function Header() {
  const { user, logout } = useAuth();
  const router = useRouter();

  function handleLogout() {
    logout();
    router.push("/login");
  }

  return (
    <header className="bg-white border-b border-gray-200 px-8 py-4 flex justify-between items-center">
      <div>
        <h1 className="text-xl font-bold text-slate-900">Dashboard</h1>
        <p className="text-sm text-gray-500">
          Welcome back, <span className="text-emerald-700 font-medium">{user?.name}</span>
        </p>
      </div>

      <div className="flex items-center gap-4">
        <div className="text-right">
          <p className="font-semibold text-slate-900 text-sm">{user?.name}</p>
          <p className="text-xs text-emerald-600 font-medium uppercase tracking-wide">
            {user?.role}
          </p>
        </div>
        <div className="w-9 h-9 rounded-full bg-emerald-100 text-emerald-700 flex items-center justify-center font-semibold text-sm">
          {user?.name?.charAt(0)}
        </div>
        <button
          onClick={handleLogout}
          className="text-sm text-gray-500 border border-gray-200 rounded-lg px-3 py-1.5 hover:bg-red-50 hover:text-red-600 hover:border-red-200 transition-colors"
        >
          Logout
        </button>
      </div>
    </header>
  );
}