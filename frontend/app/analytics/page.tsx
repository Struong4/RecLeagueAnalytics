'use client'

import { useEffect, useState } from 'react'
import LeaderboardTable from '@/components/LeaderboardTable'
import type { LeaderboardEntry, SeasonSummary } from '@/lib/types'
import { getScoringLeaderboard, getEfficiencyLeaderboard, getSeasonSummary } from '@/lib/api'
import Link from 'next/link'

export default function AnalyticsPage() {
  const [scoring, setScoring]         = useState<LeaderboardEntry[]>([])
  const [efficiency, setEfficiency]   = useState<LeaderboardEntry[]>([])
  const [summary, setSummary]         = useState<SeasonSummary | null>(null)
  const [year, setYear]               = useState(new Date().getFullYear())
  const [loading, setLoading]         = useState(true)
  const [summaryLoading, setSummaryLoading] = useState(false)

  // Load leaderboards on mount
  useEffect(() => {
    Promise.allSettled([getScoringLeaderboard(), getEfficiencyLeaderboard()]).then(
      ([s, e]) => {
        if (s.status === 'fulfilled') setScoring(s.value)
        if (e.status === 'fulfilled') setEfficiency(e.value)
        setLoading(false)
      },
    )
  }, [])

  // Load season summary when year changes
  useEffect(() => {
    setSummaryLoading(true)
    setSummary(null)
    getSeasonSummary(year)
      .then(setSummary)
      .catch(() => setSummary(null))
      .finally(() => setSummaryLoading(false))
  }, [year])

  return (
    <main className="min-h-screen bg-canvas px-4 py-6">
      <div className="max-w-5xl mx-auto">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <span className="text-accent">📈</span>
            Analytics
          </h1>
        </div>

        {/* Season summary */}
        <section className="mb-8">
          <div className="flex items-center gap-4 mb-3">
            <h2 className="text-xs font-bold uppercase tracking-widest text-muted">
              Season Summary
            </h2>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setYear(y => y - 1)}
                className="text-muted hover:text-white px-2 py-0.5 rounded bg-overlay transition-colors text-sm"
              >
                ‹
              </button>
              <span className="text-white font-mono font-bold text-sm w-12 text-center">{year}</span>
              <button
                onClick={() => setYear(y => y + 1)}
                className="text-muted hover:text-white px-2 py-0.5 rounded bg-overlay transition-colors text-sm"
              >
                ›
              </button>
            </div>
          </div>

          {summaryLoading ? (
            <div className="bg-surface border border-dim rounded-lg p-6 text-muted text-sm text-center animate-pulse">
              Loading season {year}...
            </div>
          ) : summary ? (
            <div className="bg-surface border border-dim rounded-lg p-5">
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-4">
                <div>
                  <p className="text-muted text-xs uppercase tracking-wider mb-1">Games</p>
                  <p className="text-white font-bold font-mono text-xl">{summary.totalGames}</p>
                </div>
                <div>
                  <p className="text-muted text-xs uppercase tracking-wider mb-1">Teams</p>
                  <p className="text-white font-bold font-mono text-xl">{summary.totalTeams}</p>
                </div>
                <div>
                  <p className="text-muted text-xs uppercase tracking-wider mb-1">Players</p>
                  <p className="text-white font-bold font-mono text-xl">{summary.totalPlayers}</p>
                </div>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-4 border-t border-dim">
                {summary.topScorer && (
                  <div>
                    <p className="text-muted text-xs uppercase tracking-wider mb-1">Top Scorer</p>
                    <Link
                      href={`/players/${summary.topScorer.playerId}`}
                      className="text-white font-semibold hover:text-accent transition-colors"
                    >
                      {summary.topScorer.playerName}
                    </Link>
                    <span className="text-accent font-mono font-bold ml-2">
                      {summary.topScorer.value.toFixed(1)} PPG
                    </span>
                    <span className="text-muted text-xs ml-2">{summary.topScorer.teamName}</span>
                  </div>
                )}
                {summary.topEfficiency && (
                  <div>
                    <p className="text-muted text-xs uppercase tracking-wider mb-1">
                      Top Efficiency (TS%)
                    </p>
                    <Link
                      href={`/players/${summary.topEfficiency.playerId}`}
                      className="text-white font-semibold hover:text-accent transition-colors"
                    >
                      {summary.topEfficiency.playerName}
                    </Link>
                    <span className="text-accent font-mono font-bold ml-2">
                      {(summary.topEfficiency.value * 100).toFixed(1)}%
                    </span>
                    <span className="text-muted text-xs ml-2">{summary.topEfficiency.teamName}</span>
                  </div>
                )}
              </div>
            </div>
          ) : (
            <div className="bg-surface border border-dim rounded-lg p-6 text-muted text-sm text-center">
              No data for season {year}
            </div>
          )}
        </section>

        {/* Leaderboards */}
        {loading ? (
          <div className="text-muted text-sm text-center py-12 animate-pulse">
            Loading leaderboards...
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <LeaderboardTable
              title="Scoring Leaders"
              entries={scoring}
              valueLabel="PPG"
              formatValue={v => v.toFixed(1)}
            />
            <LeaderboardTable
              title="Efficiency Leaders (TS%)"
              entries={efficiency}
              valueLabel="TS%"
              formatValue={v => (v * 100).toFixed(1) + '%'}
            />
          </div>
        )}
      </div>
    </main>
  )
}
