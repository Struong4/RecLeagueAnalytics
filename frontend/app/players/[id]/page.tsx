import { notFound } from 'next/navigation'
import dynamic from 'next/dynamic'
import Link from 'next/link'
import { getPlayer, getPlayerAnalytics, getPlayerTrends } from '@/lib/api'
import StatCard from '@/components/StatCard'

// recharts must not SSR — window is not defined on the server
const TrendChart = dynamic(() => import('@/components/TrendChart'), { ssr: false })

interface Props {
  params: { id: string }
}

const pct = (v: number | null) => (v != null ? (v * 100).toFixed(1) + '%' : '—')
const num = (v: number | null, d = 1) => (v != null ? Number(v).toFixed(d) : '—')

export default async function PlayerPage({ params }: Props) {
  const id = parseInt(params.id, 10)
  if (isNaN(id)) notFound()

  let player, analytics, trends
  try {
    ;[player, analytics, trends] = await Promise.all([
      getPlayer(id),
      getPlayerAnalytics(id),
      getPlayerTrends(id),
    ])
  } catch {
    notFound()
  }

  return (
    <main className="min-h-screen bg-canvas px-4 py-6">
      <div className="max-w-4xl mx-auto">
        {/* Back link */}
        <Link href="/" className="text-muted text-xs hover:text-accent transition-colors mb-6 inline-block">
          ← All Players
        </Link>

        {/* Header */}
        <div className="mb-6">
          <div className="flex items-baseline gap-3">
            <span className="text-accent font-mono text-lg font-bold">#{player.jerseyNumber}</span>
            <h1 className="text-3xl font-bold text-white">{player.name}</h1>
          </div>
          <div className="flex gap-3 mt-1 text-sm text-muted">
            <span>{analytics.teamName}</span>
            {player.position && (
              <>
                <span>·</span>
                <span className="bg-overlay px-2 py-0.5 rounded text-xs font-bold text-white">
                  {player.position}
                </span>
              </>
            )}
            <span>·</span>
            <span>{analytics.gamesPlayed} games played</span>
          </div>
        </div>

        {/* Per-game averages */}
        <section className="mb-6">
          <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
            Per-Game Averages
          </h2>
          <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
            <StatCard label="PPG" value={num(analytics.pointsPerGame)} highlight />
            <StatCard label="RPG" value={num(analytics.reboundsPerGame)} />
            <StatCard label="APG" value={num(analytics.assistsPerGame)} />
            <StatCard label="SPG" value={num(analytics.stealsPerGame)} />
            <StatCard label="BPG" value={num(analytics.blocksPerGame)} />
          </div>
        </section>

        {/* Shooting efficiency */}
        <section className="mb-6">
          <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
            Shooting Efficiency
          </h2>
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
            <StatCard label="TS%" value={pct(analytics.trueShootingPct)} highlight sub="True Shooting" />
            <StatCard label="FG%"  value={pct(analytics.fieldGoalPct)} />
            <StatCard label="3P%"  value={pct(analytics.threePointPct)} />
            <StatCard label="FT%"  value={pct(analytics.freeThrowPct)} />
          </div>
        </section>

        {/* Advanced */}
        <section className="mb-8">
          <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
            Advanced
          </h2>
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
            <StatCard label="PER36"   value={num(analytics.per36)}    sub="Efficiency per 36 min" />
            <StatCard label="USG%"    value={pct(analytics.usageRate)} sub="Usage Rate" />
            <StatCard label="AST/TO"  value={num(analytics.astToRatio, 2)} sub="Assist-to-Turnover" />
          </div>
        </section>

        {/* Trend chart */}
        <section>
          <h2 className="text-xs font-bold uppercase tracking-widest text-muted mb-3">
            Game Trends
          </h2>
          <TrendChart trends={trends} />
        </section>
      </div>
    </main>
  )
}
