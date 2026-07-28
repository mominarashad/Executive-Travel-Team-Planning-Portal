"use client";

import { useEffect, useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import DashboardLayout from "@/components/layout/DashboardLayout";
import CityAutocompleteInput from "@/components/CityAutocompleteInput";
import {
    getCities,
    createCity,
    deleteCity,
    getContactsByCity,
    createContact,
    deleteContact,
} from "@/services/directoryService";
import { City } from "@/types/city";
import { Contact } from "@/types/directory";

export default function DirectoryPage() {
    const { user } = useAuth();
    const router = useRouter();

    const [cities, setCities] = useState<City[]>([]);
    const [contactsByCity, setContactsByCity] = useState<Record<string, Contact[]>>({});
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showCityForm, setShowCityForm] = useState(false);
    const [cityName, setCityName] = useState("");
    const [cityCountry, setCityCountry] = useState("");
    const [cityFormError, setCityFormError] = useState<string | null>(null);

    const [contactFormCityId, setContactFormCityId] = useState<string | null>(null);
    const [contactName, setContactName] = useState("");
    const [contactOrg, setContactOrg] = useState("");
    const [contactRole, setContactRole] = useState("");
    const [contactEmail, setContactEmail] = useState("");
    const [contactPhone, setContactPhone] = useState("");
    const [contactFormError, setContactFormError] = useState<string | null>(null);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        if (!user) {
            router.push("/login");
            return;
        }
        loadCities();
    }, [user, router]);

    async function loadCities() {
        setLoading(true);
        setError(null);
        try {
            const citiesData = await getCities();
            setCities(citiesData);
            const entries = await Promise.all(
                citiesData.map(async (c) => [c.id, await getContactsByCity(c.id)] as const)
            );
            setContactsByCity(Object.fromEntries(entries));
        } catch {
            setError("Failed to load directory.");
        } finally {
            setLoading(false);
        }
    }

    async function handleCreateCity(e: FormEvent) {
        e.preventDefault();
        setCityFormError(null);
        setSubmitting(true);
        try {
            await createCity({ name: cityName, country: cityCountry });
            setShowCityForm(false);
            setCityName("");
            setCityCountry("");
            await loadCities();
        } catch (err: any) {
            setCityFormError(err?.response?.data?.message || "Failed to add city.");
        } finally {
            setSubmitting(false);
        }
    }

    async function handleDeleteCity(id: string) {
        if (!confirm("Delete this city and all its contacts?")) return;
        try {
            await deleteCity(id);
            await loadCities();
        } catch (err: any) {
            alert(err?.response?.data?.message || "Failed to delete city.");
        }
    }

    function resetContactForm() {
        setContactName("");
        setContactOrg("");
        setContactRole("");
        setContactEmail("");
        setContactPhone("");
        setContactFormError(null);
    }

    async function handleCreateContact(e: FormEvent, cityId: string) {
        e.preventDefault();
        setContactFormError(null);
        setSubmitting(true);
        try {
            await createContact({
                name: contactName,
                organization: contactOrg,
                role: contactRole,
                email: contactEmail,
                phone: contactPhone,
                sortOrder: 0,
                cityId,
            });
            setContactFormCityId(null);
            resetContactForm();
            await loadCities();
        } catch (err: any) {
            setContactFormError(err?.response?.data?.message || "Failed to add contact.");
        } finally {
            setSubmitting(false);
        }
    }

    async function handleDeleteContact(id: string) {
        if (!confirm("Remove this contact?")) return;
        try {
            await deleteContact(id);
            await loadCities();
        } catch {
            alert("Failed to delete contact.");
        }
    }

    if (!user) return null;

    return (
        <DashboardLayout>
            <div className="space-y-6">
                <div className="flex justify-between items-center">
                    <h2 className="text-2xl font-bold">Directory</h2>
                    <button
                        onClick={() => setShowCityForm((s) => !s)}
                        className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm"
                    >
                        {showCityForm ? "Cancel" : "+ Add City"}
                    </button>
                </div>

                {showCityForm && (
                    <form onSubmit={handleCreateCity} className="bg-white rounded-xl shadow p-6 flex gap-4 items-end flex-wrap">
                        {cityFormError && <p className="text-red-600 text-sm w-full">{cityFormError}</p>}
                        <div>
                            <label className="block text-sm font-medium mb-1">City Name</label>
                            <CityAutocompleteInput
                                onSelect={(city) => {
                                    setCityName(city.name);
                                    setCityCountry(city.country);
                                }}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Country</label>
                            <input
                                required value={cityCountry} onChange={(e) => setCityCountry(e.target.value)}
                                className="border border-gray-200 rounded-lg p-2 text-sm"
                            />
                        </div>
                        <button
                            type="submit" disabled={submitting}
                            className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm disabled:opacity-60"
                        >
                            Add
                        </button>
                    </form>
                )}

                {loading && <p className="text-gray-500">Loading directory...</p>}
                {error && <p className="text-red-600">{error}</p>}

                {!loading && !error && (
                    <div className="space-y-4">
                        {cities.map((city) => (
                            <div key={city.id} className="bg-white rounded-xl shadow p-5">
                                <div className="flex justify-between items-center mb-3">
                                    <h3 className="font-semibold text-lg">{city.name}, {city.country}</h3>
                                    <div className="flex gap-3">
                                        <button
                                            onClick={() => setContactFormCityId(contactFormCityId === city.id ? null : city.id)}
                                            className="text-sm text-[#0f3c3c] hover:underline"
                                        >
                                            {contactFormCityId === city.id ? "Cancel" : "+ Add Contact"}
                                        </button>
                                        <button
                                            onClick={() => handleDeleteCity(city.id)}
                                            className="text-sm text-red-600 hover:underline"
                                        >
                                            Delete City
                                        </button>
                                    </div>
                                </div>

                                {contactFormCityId === city.id && (
                                    <form
                                        onSubmit={(e) => handleCreateContact(e, city.id)}
                                        className="bg-gray-50 rounded-lg p-4 mb-3 grid grid-cols-2 gap-3"
                                    >
                                        {contactFormError && <p className="text-red-600 text-sm col-span-2">{contactFormError}</p>}
                                        <input placeholder="Name" required value={contactName} onChange={(e) => setContactName(e.target.value)} className="border border-gray-200 rounded p-2 text-sm" />
                                        <input placeholder="Organization" value={contactOrg} onChange={(e) => setContactOrg(e.target.value)} className="border border-gray-200 rounded p-2 text-sm" />
                                        <input placeholder="Role" value={contactRole} onChange={(e) => setContactRole(e.target.value)} className="border border-gray-200 rounded p-2 text-sm" />
                                        <input placeholder="Email" value={contactEmail} onChange={(e) => setContactEmail(e.target.value)} className="border border-gray-200 rounded p-2 text-sm" />
                                        <input placeholder="Phone" value={contactPhone} onChange={(e) => setContactPhone(e.target.value)} className="border border-gray-200 rounded p-2 text-sm col-span-2" />
                                        <button type="submit" disabled={submitting} className="bg-[#0f3c3c] text-white px-4 py-2 rounded-lg text-sm col-span-2 disabled:opacity-60">
                                            Save Contact
                                        </button>
                                    </form>
                                )}

                                <div className="flex flex-wrap gap-2">
                                    {(contactsByCity[city.id] || []).map((c) => (
                                        <div key={c.id} className="flex items-center gap-2 bg-gray-100 rounded-full pl-3 pr-2 py-1 text-sm">
                                            <span>
                                                <strong>{c.name}</strong>{c.organization && ` · ${c.organization}`}{c.role && ` (${c.role})`}
                                            </span>
                                            <button onClick={() => handleDeleteContact(c.id)} className="text-red-500 hover:text-red-700 text-xs">✕</button>
                                        </div>
                                    ))}
                                    {(contactsByCity[city.id] || []).length === 0 && (
                                        <span className="text-gray-400 text-sm">No contacts yet.</span>
                                    )}
                                </div>
                            </div>
                        ))}
                        {cities.length === 0 && <p className="text-gray-400 text-center py-8">No cities yet.</p>}
                    </div>
                )}
            </div>
        </DashboardLayout>
    );
}