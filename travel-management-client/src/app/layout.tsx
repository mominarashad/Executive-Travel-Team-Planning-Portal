import "./globals.css";
import type { Metadata } from "next";
import { AuthProvider } from "@/context/AuthContext";
export const metadata: Metadata = {
  title: "Travel Management",
  description: "Travel Management System",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
  <AuthProvider>
    {children}
  </AuthProvider>
</body>
    </html>
  );
}