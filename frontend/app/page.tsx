import { getPlayers, getTeams, getPlayerAnalytics } from '@/lib/api'
import StatsTable from '@/components/StatsTable'
import type { PlayerRow } from '@/lib/types'

// No caching — always fetch fresh data
export const dynamic = 'force-dynamic'

export default async function StatsPage() {
  let players, teams
  try {
    ;[players, teams] = await Promise.all([getPlayers(), getTeams()])
  } catch {
    return (
      <main className="min-h-screen bg-canvas flex items-center justify-center">
        <div className="text-center">
          <p className="text-red-400 text-sm mb-2">Could not connect to the API</p>
          <p className="text-muted text-xs">Make sure the backend is running ({process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:8080'})</p>
        </div>
      </main>
    )
  }

  // Fan out analytics calls in parallel — allSettled so one failure doesn't break the page
  const analyticsResults = await Promise.allSettled(
    players.map(p => getPlayerAnalytics(p.id)),
  )

  const rows: PlayerRow[] = players.map((player, i) => {
    const result = analyticsResults[i]
    if (result.status === 'fulfilled') {
      return {
        ...result.value,
        position: player.position,
        jerseyNumber: player.jerseyNumber,
      }
    }
    // Player has no stat lines yet — show zeroed row
    return {
      playerId: player.id,
      playerName: player.name,
      teamName: '',
      gamesPlayed: 0,
      pointsPerGame: 0,
      reboundsPerGame: 0,
      assistsPerGame: 0,
      stealsPerGame: 0,
      blocksPerGame: 0,
      trueShootingPct: null,
      fieldGoalPct: null,
      threePointPct: null,
      freeThrowPct: null,
      astToRatio: null,
      per36: null,
      usageRate: null,
      position: player.position,
      jerseyNumber: player.jerseyNumber,
    }
  })

  return (
    <main className="min-h-screen bg-canvas px-4 py-6">
      <div className="max-w-screen-2xl mx-auto">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <span className="text-accent">📊</span>
            Player Statistics
          </h1>
          <p className="text-muted text-sm mt-1">
            All-time averages · {rows.length} players across {teams.length} teams
          </p>
        </div>
        <StatsTable players={rows} teams={teams} />
      </div>
    </main>
  )
}
