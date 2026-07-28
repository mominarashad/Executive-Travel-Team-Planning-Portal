"use client";

import { useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { login } from "@/services/authService";
import {
  Mail,
  Lock,
  Eye,
  EyeOff,
  Plane,
  Calendar,
  Users,
  MapPin,
} from "lucide-react";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const router = useRouter();
  const auth = useAuth();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const data = await login({ email, password });

      auth.login(data.token, data.user);

      router.push("/dashboard");
    } catch (err) {
      console.log(err);
      setError("Invalid email or password. Please try again.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="min-h-screen flex bg-white">
      {/* Left panel */}
      <div className="w-full lg:w-1/2 flex items-center justify-center px-8 py-12">
        <div className="w-full max-w-sm">

          {/* Logo */}
          <div className="flex items-center gap-2 mb-10">
            <div className="w-9 h-9 rounded-lg bg-[#0f3c3c] flex items-center justify-center text-white font-semibold text-sm">
              MGH
            </div>

            <span className="text-lg font-semibold text-gray-900">
              Meridian Group Holdings
            </span>
          </div>


          <h1 className="text-2xl font-bold text-gray-900 mb-2">
            Welcome Back
          </h1>

          <p className="text-sm text-gray-500 mb-8 leading-relaxed">
            Sign in to access the executive travel and team planning dashboard.
          </p>


          <form onSubmit={handleSubmit} className="space-y-5">

            {/* Email */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1.5">
                Email
              </label>

              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />

                <input
                  type="email"
                  required
                  placeholder="Enter your email"
                  className="w-full pl-10 pr-3 py-3 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-[#0f3c3c]/20 focus:border-[#0f3c3c]"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
            </div>


            {/* Password */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1.5">
                Password
              </label>

              <div className="relative">

                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />

                <input
                  type={showPassword ? "text" : "password"}
                  required
                  placeholder="Enter your password"
                  className="w-full pl-10 pr-10 py-3 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-[#0f3c3c]/20 focus:border-[#0f3c3c]"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />


                <button
                  type="button"
                  onClick={() => setShowPassword((s) => !s)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                  aria-label={
                    showPassword ? "Hide password" : "Show password"
                  }
                >
                  {showPassword ? (
                    <EyeOff className="w-4 h-4" />
                  ) : (
                    <Eye className="w-4 h-4" />
                  )}

                </button>

              </div>
            </div>


            {/* Forgot password */}
            <div className="flex justify-end">
              <a
                href="/forgot-password"
                className="text-sm text-[#0f3c3c] font-medium hover:underline"
              >
                Forgot Password?
              </a>
            </div>


            {error && (
              <p className="text-sm text-red-600 -mt-1">
                {error}
              </p>
            )}


            <button
              type="submit"
              disabled={loading}
              className="w-full bg-[#0f3c3c] text-white py-3 rounded-lg text-sm font-medium hover:bg-[#0c3030] transition-colors disabled:opacity-60"
            >
              {loading ? "Signing In..." : "Sign In"}
            </button>


          </form>

        </div>
      </div>



      {/* Right panel */}
      <div className="hidden lg:flex w-1/2 relative bg-gradient-to-br from-[#0f3c3c] to-[#0a2828] text-white p-14 flex-col justify-center overflow-hidden">

        <div className="max-w-md">

          <h2 className="text-4xl font-bold leading-tight mb-6">
            Plan the CEO&apos;s Journey, Effortlessly
          </h2>


          <p className="text-white/70 text-base mb-12 leading-relaxed">
            One dashboard for international travel, team calendars,
            contacts, and meeting logistics — built for Meridian Group
            Holdings&apos; executive office.
          </p>



          <div className="space-y-6">


            <Feature
              icon={<Plane className="w-5 h-5 text-white" />}
              title="Trip Planning"
              text="Coordinate the CEO's international business trips end to end."
            />


            <Feature
              icon={<Calendar className="w-5 h-5 text-white" />}
              title="Team Calendar"
              text="Track the full team's schedule alongside every trip."
            />


            <Feature
              icon={<MapPin className="w-5 h-5 text-white" />}
              title="City Contact Directory"
              text="Find the right contact, organized by city, in seconds."
            />


            <Feature
              icon={<Users className="w-5 h-5 text-white" />}
              title="Meeting Management"
              text="Schedule per-trip meetings with agendas and materials attached."
            />

          </div>

        </div>

      </div>


    </main>
  );
}



function Feature({
  icon,
  title,
  text,
}: {
  icon: React.ReactNode;
  title: string;
  text: string;
}) {

  return (
    <div className="flex items-start gap-4">

      <div className="w-10 h-10 rounded-lg bg-white/10 flex items-center justify-center shrink-0">
        {icon}
      </div>


      <div>
        <p className="font-medium">
          {title}
        </p>

        <p className="text-sm text-white/60">
          {text}
        </p>

      </div>

    </div>
  );
}