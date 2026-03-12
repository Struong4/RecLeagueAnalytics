import { notFound } from 'next/navigation'
import Link from 'next/link'
import { getTeam, getTeamAnalytics } from '@/lib/api'
import StatCard from '@/components/StatCard'

interface Props {
  params: { id: string }
}

export default async function TeamPage({ params }: Props) {
  const id = parseInt(params.id, 10)
  if (isNaN(id)) notFound()

  let team, analytics
  try {
    ;[team, analytics] = await Promise.all([getTeam(id), getTeamAnalytics(id)])
  } catch {
    notFound()
  }

  const winPct = analytics.gamesPlayed > 0
    ? (analytics.winPct * 100).toFixed(1) + '%'
    : '—'

  return (
    <main className="min-h-screen bg-canvas px-4 py-6">
      <div className="max-w-3xl mx-auto">
        <Link href="/teams" className="text-muted text-xs hover:text-accent transition-colors mb-6 inline-block">
          ← All Teams
        </Link>

        {/* Header */}
        <div className="mb-6">
          <div className="flex items-baseline gap-3">
            <h1 className="text-3xl font-bold text-white">{team.name}</h1>
            <span className="text-accent font-mono text-sm font-bold bg-overlay px-2 py-1 rounded">
              {team.season}
            </span>
          </div>
        </div>

        {analytics.gamesPlayed === 0 ? (
          <div className="text-muted text-sm py-12 text-center">
            No games played yet for this team
          </div>
        ) : (
          <>
            {/* Record */}
            <section className="mb-6">
              <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
                Record
              </h2>
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
                <StatCard
                  label="W – L"
                  value={`${analytics.wins} – ${analytics.losses}`}
                  highlight
                />
                <StatCard label="Win %" value={winPct} />
                <StatCard label="Games"  value={analytics.gamesPlayed} />
              </div>
            </section>

            {/* Per-game averages */}
            <section className="mb-6">
              <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
                Team Averages
              </h2>
              <div className="grid grid-cols-3 gap-3">
                <StatCard label="PPG" value={analytics.pointsPerGame.toFixed(1)} />
                <StatCard label="RPG" value={analytics.reboundsPerGame.toFixed(1)} />
                <StatCard label="APG" value={analytics.assistsPerGame.toFixed(1)} />
              </div>
            </section>

            {/* Efficiency */}
            <section>
              <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
                Efficiency
              </h2>
              <div className="grid grid-cols-3 gap-3">
                <StatCard
                  label="Off. Eff."
                  value={analytics.offensiveEfficiency.toFixed(1)}
                  sub="Points scored / game"
                />
                <StatCard
                  label="Def. Eff."
                  value={analytics.defensiveEfficiency.toFixed(1)}
                  sub="Points allowed / game"
                />
                <StatCard
                  label="Net Eff."
                  value={`${analytics.netEfficiency >= 0 ? '+' : ''}${analytics.netEfficiency.toFixed(1)}`}
                  sub="Scored minus allowed"
                  positive={analytics.netEfficiency > 0}
                  negative={analytics.netEfficiency < 0}
                />
              </div>
            </section>
          </>
        )}
      </div>
    </main>
  )
}
