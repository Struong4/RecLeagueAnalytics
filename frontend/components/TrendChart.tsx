'use client'

import {
  ComposedChart,
  Bar,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts'
import type { PlayerTrend } from '@/lib/types'

interface Props {
  trends: PlayerTrend[]
}

export default function TrendChart({ trends }: Props) {
  if (trends.length === 0) {
    return (
      <div className="bg-surface border border-dim rounded-lg p-8 text-center text-muted text-sm">
        No game data available yet
      </div>
    )
  }

  const data = trends.map(t => ({
    date: new Date(t.gameDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
    pts: t.points,
    ts: t.trueShootingPct != null ? +(t.trueShootingPct * 100).toFixed(1) : null,
    rollingTs: t.rollingAvgTsPct != null ? +(t.rollingAvgTsPct * 100).toFixed(1) : null,
  }))

  return (
    <div className="bg-surface border border-dim rounded-lg p-4">
      <h3 className="text-sm font-bold uppercase tracking-wider text-muted mb-4">
        Game-by-Game Trend
      </h3>
      <ResponsiveContainer width="100%" height={280}>
        <ComposedChart data={data} margin={{ top: 5, right: 30, left: 0, bottom: 5 }}>
          <CartesianGrid strokeDasharray="3 3" stroke="#30363d" vertical={false} />
          <XAxis
            dataKey="date"
            tick={{ fill: '#8b949e', fontSize: 11 }}
            axisLine={{ stroke: '#30363d' }}
            tickLine={false}
          />
          {/* Left axis: points */}
          <YAxis
            yAxisId="pts"
            tick={{ fill: '#8b949e', fontSize: 11 }}
            axisLine={false}
            tickLine={false}
            width={28}
          />
          {/* Right axis: TS% */}
          <YAxis
            yAxisId="ts"
            orientation="right"
            tick={{ fill: '#8b949e', fontSize: 11 }}
            axisLine={false}
            tickLine={false}
            tickFormatter={v => v + '%'}
            width={40}
          />
          <Tooltip
            contentStyle={{
              backgroundColor: '#21262d',
              border: '1px solid #30363d',
              borderRadius: '6px',
              fontSize: '12px',
              color: '#e6edf3',
            }}
            formatter={(value: number, name: string) => {
              if (name === 'pts') return [value, 'Points']
              if (name === 'ts') return [value + '%', 'TS%']
              if (name === 'rollingTs') return [value + '%', '3-Game Avg TS%']
              return [value, name]
            }}
          />
          <Legend
            wrapperStyle={{ fontSize: '11px', color: '#8b949e', paddingTop: '8px' }}
            formatter={(value) => {
              if (value === 'pts') return 'Points'
              if (value === 'ts') return 'TS%'
              if (value === 'rollingTs') return '3-Game Avg TS%'
              return value
            }}
          />
          <Bar yAxisId="pts" dataKey="pts" fill="#21262d" stroke="#30363d" radius={[2, 2, 0, 0]} />
          <Line
            yAxisId="ts"
            type="monotone"
            dataKey="ts"
            stroke="#8b949e"
            strokeWidth={1.5}
            dot={false}
            connectNulls
          />
          <Line
            yAxisId="ts"
            type="monotone"
            dataKey="rollingTs"
            stroke="#f0b429"
            strokeWidth={2}
            dot={false}
            connectNulls
          />
        </ComposedChart>
      </ResponsiveContainer>
    </div>
  )
}
