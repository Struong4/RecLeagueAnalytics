'use client'

import { useState, useMemo } from 'react'
import Link from 'next/link'
import type { PlayerRow, Team } from '@/lib/types'

type SortKey = keyof PlayerRow
type SortDir = 'asc' | 'desc'

interface Column {
  key: SortKey
  label: string
  fmt: (v: PlayerRow[SortKey]) => string
  align: 'left' | 'right'
}

const pct = (v: number | null) => (v != null ? (v * 100).toFixed(1) + '%' : '—')
const num = (v: number | null, d = 1) => (v != null ? Number(v).toFixed(d) : '—')

const COLS: Column[] = [
  { key: 'playerName',      label: 'PLAYER',  fmt: v => String(v),  align: 'left'  },
  { key: 'teamName',        label: 'TEAM',    fmt: v => String(v),  align: 'left'  },
  { key: 'position',        label: 'POS',     fmt: v => String(v) || '—', align: 'left'  },
  { key: 'gamesPlayed',     label: 'GP',      fmt: v => String(v),  align: 'right' },
  { key: 'pointsPerGame',   label: 'PPG',     fmt: v => num(v as number | null), align: 'right' },
  { key: 'reboundsPerGame', label: 'RPG',     fmt: v => num(v as number | null), align: 'right' },
  { key: 'assistsPerGame',  label: 'APG',     fmt: v => num(v as number | null), align: 'right' },
  { key: 'stealsPerGame',   label: 'SPG',     fmt: v => num(v as number | null), align: 'right' },
  { key: 'blocksPerGame',   label: 'BPG',     fmt: v => num(v as number | null), align: 'right' },
  { key: 'trueShootingPct', label: 'TS%',     fmt: v => pct(v as number | null), align: 'right' },
  { key: 'fieldGoalPct',    label: 'FG%',     fmt: v => pct(v as number | null), align: 'right' },
  { key: 'threePointPct',   label: '3P%',     fmt: v => pct(v as number | null), align: 'right' },
  { key: 'freeThrowPct',    label: 'FT%',     fmt: v => pct(v as number | null), align: 'right' },
  { key: 'astToRatio',      label: 'A/T',     fmt: v => num(v as number | null, 2), align: 'right' },
  { key: 'per36',           label: 'PER36',   fmt: v => num(v as number | null), align: 'right' },
  { key: 'usageRate',       label: 'USG%',    fmt: v => pct(v as number | null), align: 'right' },
]

const POSITIONS = ['ALL', 'PG', 'SG', 'SF', 'PF', 'C'] as const

interface Props {
  players: PlayerRow[]
  teams: Team[]
}

export default function StatsTable({ players, teams }: Props) {
  const [sortKey, setSortKey]       = useState<SortKey>('pointsPerGame')
  const [sortDir, setSortDir]       = useState<SortDir>('desc')
  const [position, setPosition]     = useState('ALL')
  const [teamFilter, setTeamFilter] = useState('')
  const [search, setSearch]         = useState('')

  function handleSort(key: SortKey) {
    if (key === sortKey) {
      setSortDir(d => (d === 'desc' ? 'asc' : 'desc'))
    } else {
      setSortKey(key)
      setSortDir('desc')
    }
  }

  const filtered = useMemo(
    () =>
      players
        .filter(p => position === 'ALL' || p.position === position)
        .filter(p => !teamFilter || p.teamName === teamFilter)
        .filter(p => !search || p.playerName.toLowerCase().includes(search.toLowerCase())),
    [players, position, teamFilter, search],
  )

  const sorted = useMemo(() => {
    return [...filtered].sort((a, b) => {
      const av = a[sortKey]
      const bv = b[sortKey]

      if (typeof av === 'string' && typeof bv === 'string') {
        return sortDir === 'asc' ? av.localeCompare(bv) : bv.localeCompare(av)
      }

      // Numeric: push nulls to the bottom regardless of direction
      const aNum = (av as number | null) ?? (sortDir === 'desc' ? -Infinity : Infinity)
      const bNum = (bv as number | null) ?? (sortDir === 'desc' ? -Infinity : Infinity)
      return sortDir === 'asc' ? aNum - bNum : bNum - aNum
    })
  }, [filtered, sortKey, sortDir])

  const teamNames = useMemo(
    () => [...new Set(players.map(p => p.teamName))].filter(Boolean).sort(),
    [players],
  )

  return (
    <div className="space-y-4">
      {/* Filter bar */}
      <div className="flex flex-wrap gap-3 items-center">
        <input
          type="text"
          placeholder="Search players..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          className="bg-surface border border-dim text-white placeholder-muted text-sm px-3 py-1.5 rounded focus:outline-none focus:border-accent w-44"
        />

        {/* Position tabs */}
        <div className="flex gap-1">
          {POSITIONS.map(pos => (
            <button
              key={pos}
              onClick={() => setPosition(pos)}
              className={`px-3 py-1 text-xs font-bold rounded transition-colors ${
                position === pos
                  ? 'bg-accent text-black'
                  : 'bg-overlay text-muted hover:text-white'
              }`}
            >
              {pos}
            </button>
          ))}
        </div>

        {/* Team dropdown */}
        <select
          value={teamFilter}
          onChange={e => setTeamFilter(e.target.value)}
          className="bg-surface border border-dim text-white text-sm px-3 py-1.5 rounded focus:outline-none focus:border-accent"
        >
          <option value="">All Teams</option>
          {teamNames.map(name => (
            <option key={name} value={name}>
              {name}
            </option>
          ))}
        </select>

        <span className="text-muted text-xs ml-auto">{sorted.length} players</span>
      </div>

      {/* Table */}
      <div className="overflow-x-auto rounded border border-dim">
        <table className="w-full text-sm border-collapse min-w-max">
          <thead>
            <tr className="bg-overlay border-b border-dim">
              <th className="text-muted text-xs font-bold uppercase tracking-wider px-3 py-2.5 text-left w-8 select-none">
                #
              </th>
              {COLS.map(col => (
                <th
                  key={col.key}
                  onClick={() => handleSort(col.key)}
                  className={`text-xs font-bold uppercase tracking-wider px-3 py-2.5 cursor-pointer select-none whitespace-nowrap transition-colors hover:text-white ${
                    col.align === 'left' ? 'text-left' : 'text-right'
                  } ${sortKey === col.key ? 'text-accent' : 'text-muted'}`}
                >
                  {col.label}
                  {sortKey === col.key && (
                    <span className="ml-1 text-accent">{sortDir === 'desc' ? '↓' : '↑'}</span>
                  )}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {sorted.length === 0 ? (
              <tr>
                <td
                  colSpan={COLS.length + 1}
                  className="text-center text-muted py-16 text-sm"
                >
                  No players found
                </td>
              </tr>
            ) : (
              sorted.map((row, i) => (
                <tr
                  key={row.playerId}
                  className="border-b border-dim hover:bg-overlay transition-colors"
                >
                  <td className="px-3 py-2 text-muted text-xs font-mono">{i + 1}</td>
                  {COLS.map(col => {
                    const val = row[col.key]
                    const display = col.fmt(val)
                    const isSortedCol = col.key === sortKey

                    return (
                      <td
                        key={col.key}
                        className={`px-3 py-2 font-mono whitespace-nowrap text-sm ${
                          col.align === 'left' ? 'text-left' : 'text-right'
                        } ${isSortedCol ? 'bg-[#1a1f27]' : ''}`}
                      >
                        {col.key === 'playerName' ? (
                          <Link
                            href={`/players/${row.playerId}`}
                            className="font-semibold text-white hover:text-accent transition-colors"
                          >
                            {display}
                          </Link>
                        ) : (
                          <span className={col.key === 'teamName' ? 'text-muted' : 'text-slate-300'}>
                            {display}
                          </span>
                        )}
                      </td>
                    )
                  })}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
