"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const links = [
  { href: "/dashboard", label: "Dashboard" },
  { href: "/trips", label: "Trips" },
  { href: "/flights", label: "Flights" },
  { href: "/calendar", label: "Team Calendar" },
  { href: "/team-plan", label: "Team Plan" },
  { href: "/directory", label: "Directory" },
  { href: "/one-pager", label: "One-Pagers" },
  { href: "/data", label: "Data Management" },
];

export default function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="w-64 bg-gradient-to-b from-emerald-950 via-slate-950 to-slate-950 text-slate-300 min-h-screen p-6 border-r border-emerald-900/40">
      <div className="flex items-center gap-2 mb-10">
        <div className="w-8 h-8 rounded-lg bg-emerald-500 flex items-center justify-center">
          <span className="text-slate-950 font-bold text-sm">M</span>
        </div>
        <h1 className="text-lg font-semibold text-white tracking-tight">
          Travel Management
        </h1>
      </div>

      <nav className="space-y-1">
        {links.map((link) => {
          const active = pathname === link.href;
          return (
            <Link
              key={link.href}
              href={link.href}
              className={`flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                active
                  ? "bg-emerald-500/10 text-emerald-400 border-l-2 border-emerald-400"
                  : "text-slate-400 hover:bg-white/5 hover:text-emerald-300 border-l-2 border-transparent"
              }`}
            >
              {link.label}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}