import Link from 'next/link'
import type { LeaderboardEntry } from '@/lib/types'

interface Props {
  title: string
  entries: LeaderboardEntry[]
  valueLabel: string
  formatValue: (v: number) => string
}

export default function LeaderboardTable({ title, entries, valueLabel, formatValue }: Props) {
  return (
    <div className="bg-surface border border-dim rounded-lg overflow-hidden">
      <div className="px-4 py-3 border-b border-dim">
        <h2 className="text-sm font-bold uppercase tracking-wider text-muted">{title}</h2>
      </div>
      <table className="w-full text-sm">
        <thead>
          <tr className="bg-overlay border-b border-dim">
            <th className="text-muted text-xs uppercase tracking-wider px-4 py-2 text-left w-8">#</th>
            <th className="text-muted text-xs uppercase tracking-wider px-4 py-2 text-left">Player</th>
            <th className="text-muted text-xs uppercase tracking-wider px-4 py-2 text-left">Team</th>
            <th className="text-accent text-xs uppercase tracking-wider px-4 py-2 text-right">
              {valueLabel}
            </th>
          </tr>
        </thead>
        <tbody>
          {entries.map((entry, i) => (
            <tr key={entry.playerId} className="border-b border-dim hover:bg-overlay transition-colors">
              <td className="px-4 py-2.5 font-mono text-xs text-muted">
                {i === 0 ? '🥇' : i === 1 ? '🥈' : i === 2 ? '🥉' : i + 1}
              </td>
              <td className="px-4 py-2.5">
                <Link
                  href={`/players/${entry.playerId}`}
                  className="font-semibold text-white hover:text-accent transition-colors"
                >
                  {entry.playerName}
                </Link>
              </td>
              <td className="px-4 py-2.5 text-muted">{entry.teamName}</td>
              <td className="px-4 py-2.5 text-right font-mono font-bold text-accent">
                {formatValue(entry.value)}
              </td>
            </tr>
          ))}
          {entries.length === 0 && (
            <tr>
              <td colSpan={4} className="text-center text-muted py-8">
                No data yet
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  )
}
