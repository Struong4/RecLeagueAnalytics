'use client'

import { useState } from 'react'
import { ingestGame } from '@/lib/api'

type Status = 'idle' | 'loading' | 'success' | 'error' | 'duplicate'

const PLACEHOLDER = `{
  "gameDate": "2026-01-15T19:00:00",
  "season": 2026,
  "location": "Main Gym",
  "homeTeam": {
    "name": "Thunder",
    "division": "East",
    "finalScore": 88,
    "players": [
      {
        "firstName": "John",
        "lastName": "Smith",
        "jerseyNumber": 23,
        "position": "SG",
        "stats": {
          "minutesPlayed": 32,
          "points": 18,
          "rebounds": 4,
          "assists": 3,
          "steals": 1,
          "blocks": 0,
          "turnovers": 2,
          "personalFouls": 3,
          "fieldGoalsMade": 7,
          "fieldGoalsAttempted": 15,
          "threePointersMade": 2,
          "threePointersAttempted": 5,
          "freeThrowsMade": 2,
          "freeThrowsAttempted": 2,
          "offensiveRebounds": 1,
          "defensiveRebounds": 3
        }
      }
    ]
  },
  "awayTeam": {
    "name": "Lightning",
    "division": "West",
    "finalScore": 82,
    "players": []
  }
}`

export default function IngestPage() {
  const [text, setText]       = useState('')
  const [status, setStatus]   = useState<Status>('idle')
  const [message, setMessage] = useState('')

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setStatus('loading')
    setMessage('')

    let parsed: unknown
    try {
      parsed = JSON.parse(text)
    } catch (err) {
      setStatus('error')
      setMessage('Invalid JSON: ' + (err instanceof Error ? err.message : 'parse error'))
      return
    }

    try {
      await ingestGame(parsed)
      setStatus('success')
      setMessage('Game ingested successfully!')
      setText('')
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Unknown error'
      if (msg.startsWith('409')) {
        setStatus('duplicate')
        setMessage('Duplicate game — a game with the same teams and date already exists.')
      } else {
        setStatus('error')
        setMessage(msg)
      }
    }
  }

  return (
    <main className="min-h-screen bg-canvas px-4 py-6">
      <div className="max-w-3xl mx-auto">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <span className="text-accent">📥</span>
            Ingest Game Data
          </h1>
          <p className="text-muted text-sm mt-1">
            Paste a game JSON payload and submit to record a game and all player stats
          </p>
        </div>

        {/* Status banner */}
        {status === 'success' && (
          <div className="mb-4 px-4 py-3 rounded bg-green-900/40 border border-green-700 text-green-400 text-sm">
            ✓ {message}
          </div>
        )}
        {status === 'duplicate' && (
          <div className="mb-4 px-4 py-3 rounded bg-amber-900/40 border border-amber-700 text-amber-400 text-sm">
            ⚠ {message}
          </div>
        )}
        {status === 'error' && (
          <div className="mb-4 px-4 py-3 rounded bg-red-900/40 border border-red-700 text-red-400 text-sm">
            ✕ {message}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="text-xs font-bold uppercase tracking-wider text-muted mb-2 block">
              JSON Payload
            </label>
            <textarea
              value={text}
              onChange={e => setText(e.target.value)}
              placeholder={PLACEHOLDER}
              rows={30}
              className="w-full bg-surface border border-dim text-white placeholder-muted/50 text-xs font-mono px-4 py-3 rounded-lg focus:outline-none focus:border-accent resize-y"
              spellCheck={false}
            />
          </div>

          <button
            type="submit"
            disabled={status === 'loading' || !text.trim()}
            className="px-6 py-2.5 bg-accent text-black font-bold text-sm rounded transition-opacity disabled:opacity-40 hover:opacity-90"
          >
            {status === 'loading' ? 'Submitting…' : 'Submit Game'}
          </button>
        </form>

        {/* Schema reference */}
        <div className="mt-8 bg-surface border border-dim rounded-lg p-4">
          <h2 className="text-xs font-bold uppercase tracking-wider text-muted mb-3">
            Required Fields
          </h2>
          <ul className="text-xs text-muted space-y-1 font-mono">
            <li><span className="text-white">gameDate</span> — ISO 8601 datetime</li>
            <li><span className="text-white">season</span> — integer year (e.g. 2026)</li>
            <li><span className="text-white">location</span> — string</li>
            <li><span className="text-white">homeTeam / awayTeam</span> — name, division, finalScore, players[]</li>
            <li><span className="text-white">players[].position</span> — PG | SG | SF | PF | C</li>
            <li><span className="text-white">players[].stats</span> — all 16 stat fields required</li>
            <li><span className="text-white">minimum 5 players</span> per team</li>
          </ul>
        </div>
      </div>
    </main>
  )
}
