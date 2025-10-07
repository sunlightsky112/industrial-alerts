"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";

export default function Dashboard() {
  const qc = useQueryClient();

  // Config
  const { data: config } = useQuery({
    queryKey: ["config"],
    queryFn: async () => (await api.get("/config")).data,
  });

  const updateConfig = useMutation({
    mutationFn: (newCfg: { tempMax: number; humidityMax: number }) =>
      api.put("/config", newCfg),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["config"] }),
  });

  // Alerts
  const [status, setStatus] = useState<"open" | "ack" | "">("");
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  const { data: alerts } = useQuery({
    queryKey: ["alerts", status, from, to],
    queryFn: async () => {
      const params = new URLSearchParams();
      if (status) params.append("status", status);
      if (from) params.append("from", from);
      if (to) params.append("to", to);
      const res = await api.get(`/alerts?${params.toString()}`);
      return res.data;
    },
  });

  const ackAlert = useMutation({
    mutationFn: (id: string) => api.post(`/alerts/${id}/ack`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["alerts"] }),
  });

  return (
    <div className="p-6 space-y-6">
      <h1 className="text-2xl font-bold">Dashboard</h1>

      {/* Config card */}
      <div className="p-4 border rounded">
        <h2 className="font-semibold mb-2">Config</h2>
        {config && (
          <div>
            <p>Temp Max: {config.tempMax}</p>
            <p>Humidity Max: {config.humidityMax}</p>
            <button
              className="mt-2 bg-green-600 text-white px-3 py-1 rounded"
              onClick={() =>
                updateConfig.mutate({ tempMax: 75, humidityMax: 60 })
              }
            >
              Set to 75/60
            </button>
          </div>
        )}
      </div>

      <div className="flex gap-4 mb-4">
        <select
          className="border p-2"
          value={status}
          onChange={(e) => setStatus(e.target.value as any)}
        >
          <option value="">All</option>
          <option value="open">Open</option>
          <option value="ack">Acknowledged</option>
        </select>

        <input
          type="date"
          className="border p-2"
          value={from}
          onChange={(e) => setFrom(e.target.value)}
        />
        <input
          type="date"
          className="border p-2"
          value={to}
          onChange={(e) => setTo(e.target.value)}
        />
      </div>

      {/* Alerts table */}
      <div className="p-4 border rounded">
        <h2 className="font-semibold mb-2">Open Alerts</h2>
        <table className="w-full border">
          <thead>
            <tr className="bg-gray-100">
              <th className="p-2 border">Type</th>
              <th className="p-2 border">Value</th>
              <th className="p-2 border">Threshold</th>
              <th className="p-2 border">Created</th>
              <th className="p-2 border">Action</th>
            </tr>
          </thead>
          <tbody>
            {alerts?.map((a: any) => (
              <tr key={a.id}>
                <td className="p-2 border">{a.type}</td>
                <td className="p-2 border">{a.value}</td>
                <td className="p-2 border">{a.threshold}</td>
                <td className="p-2 border">
                  {new Date(a.createdAt).toLocaleString()}
                </td>
                <td className="p-2 border">
                  <button
                    className="bg-blue-600 text-white px-2 py-1 rounded"
                    onClick={() => ackAlert.mutate(a.id)}
                  >
                    Ack
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
