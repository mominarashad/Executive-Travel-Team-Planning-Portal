"use client";

import { useEffect, useState, ChangeEvent } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import { exportData, importData } from "@/services/dataManagementService";

export default function DataManagementPage() {
  const { user } = useAuth();
  const router = useRouter();

  const [exporting, setExporting] = useState(false);
  const [exportError, setExportError] = useState<string | null>(null);

  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [importing, setImporting] = useState(false);
  const [importError, setImportError] = useState<string | null>(null);
  const [importSuccess, setImportSuccess] = useState<string | null>(null);
  const [confirmStep, setConfirmStep] = useState(false);

  useEffect(() => {
    if (!user) {
      router.push("/login");
    }
  }, [user, router]);

  async function handleExport() {
    setExporting(true);
    setExportError(null);
    try {
      const blob = await exportData();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `travelmanagement-export-${new Date().toISOString().slice(0, 10)}.json`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch {
      setExportError("Failed to export data.");
    } finally {
      setExporting(false);
    }
  }

  function handleFileSelect(e: ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0] || null;
    setSelectedFile(file);
    setImportError(null);
    setImportSuccess(null);
    setConfirmStep(false);
  }

  async function handleImport() {
    if (!selectedFile) return;
    setImporting(true);
    setImportError(null);
    setImportSuccess(null);
    try {
      const text = await selectedFile.text();
      let parsed: unknown;
      try {
        parsed = JSON.parse(text);
      } catch {
        setImportError("That file isn't valid JSON.");
        setImporting(false);
        return;
      }
      const result = await importData(parsed);
      setImportSuccess(result.message || "Data imported successfully.");
      setSelectedFile(null);
      setConfirmStep(false);
    } catch (err: any) {
      setImportError(err?.response?.data?.message || "Import failed. No data was changed.");
    } finally {
      setImporting(false);
    }
  }

  if (!user) return null;

  return (
    <DashboardLayout>
      <div className="space-y-6 max-w-2xl">
        <h2 className="text-2xl font-bold">Data Management</h2>

        <div className="bg-white rounded-xl shadow p-6">
          <h3 className="font-semibold mb-1">Export Data</h3>
          <p className="text-sm text-gray-500 mb-4">
            Download a full backup of all trips, meetings, flights, team plans, directory,
            hotels, projects, and entities as a JSON file. User accounts are included for
            reference but without passwords.
          </p>
          {exportError && <p className="text-red-600 text-sm mb-2">{exportError}</p>}
          <button
            onClick={handleExport}
            disabled={exporting}
            className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60"
          >
            {exporting ? "Exporting..." : "Export as JSON"}
          </button>
        </div>

        <div className="bg-white rounded-xl shadow p-6">
          <h3 className="font-semibold mb-1">Import Data</h3>
          <p className="text-sm text-gray-500 mb-4">
            This <strong>replaces all business data</strong> (trips, meetings, flights, team
            plans, directory, hotels, projects, entities) with the contents of the selected
            file. User accounts and logins are not affected.
          </p>

          {importError && <p className="text-red-600 text-sm mb-2">{importError}</p>}
          {importSuccess && <p className="text-green-600 text-sm mb-2">{importSuccess}</p>}

          <input
            type="file"
            accept="application/json"
            onChange={handleFileSelect}
            className="text-sm mb-3 block"
          />

          {selectedFile && !confirmStep && (
            <button
              onClick={() => setConfirmStep(true)}
              className="bg-white border border-red-400 text-red-600 px-4 py-2 rounded-lg text-sm"
            >
              Import "{selectedFile.name}"
            </button>
          )}

          {confirmStep && (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4 mt-2">
              <p className="text-sm text-red-800 mb-3">
                This will permanently overwrite all current trips, meetings, flights, team
                plans, directory, hotels, projects, and entities. This cannot be undone. Are
                you sure?
              </p>
              <div className="flex gap-3">
                <button
                  onClick={handleImport}
                  disabled={importing}
                  className="bg-red-600 text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60"
                >
                  {importing ? "Importing..." : "Yes, overwrite data"}
                </button>
                <button
                  onClick={() => setConfirmStep(false)}
                  className="bg-white border border-gray-300 px-4 py-2 rounded-lg text-sm"
                >
                  Cancel
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </DashboardLayout>
  );
}