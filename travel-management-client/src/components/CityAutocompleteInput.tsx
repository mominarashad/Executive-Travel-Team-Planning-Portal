"use client";

import { useState, useRef, useEffect } from "react";
import { WORLD_CITIES, WorldCity } from "@/data/worldCities";

interface Props {
  onSelect: (city: WorldCity) => void;
  placeholder?: string;
}

export default function CityAutocompleteInput({ onSelect, placeholder }: Props) {
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const matches = query.trim().length > 0
    ? WORLD_CITIES.filter((c) =>
        c.name.toLowerCase().includes(query.toLowerCase())
      ).slice(0, 8)
    : [];

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function handleSelect(city: WorldCity) {
    setQuery(city.name);
    setOpen(false);
    onSelect(city);
  }

  return (
    <div ref={containerRef} className="relative">
      <input
        type="text"
        value={query}
        placeholder={placeholder || "Type a city name..."}
        onChange={(e) => {
          setQuery(e.target.value);
          setOpen(true);
        }}
        onFocus={() => setOpen(true)}
        className="w-full border border-gray-200 rounded-lg p-2 text-sm"
      />
      {open && matches.length > 0 && (
        <div className="absolute z-20 mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg max-h-48 overflow-y-auto">
          {matches.map((c, i) => (
            <button
              key={i}
              type="button"
              onClick={() => handleSelect(c)}
              className="block w-full text-left px-3 py-2 text-sm hover:bg-gray-50"
            >
              {c.name}, {c.country}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}