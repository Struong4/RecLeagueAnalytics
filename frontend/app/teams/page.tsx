import Link from 'next/link'
import { getTeams } from '@/lib/api'

export const revalidate = 60

export default async function TeamsPage() {
  let teams
  try {
    teams = await getTeams()
  } catch {
    return (
      <main className="min-h-screen bg-canvas flex items-center justify-center">
        <p className="text-red-400 text-sm">Could not load teams — is the API running?</p>
      </main>
    )
  }

  return (
    <main className="min-h-screen bg-canvas px-4 py-6">
      <div className="max-w-4xl mx-auto">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-white flex items-center gap-2">
            <span className="text-accent">🏆</span>
            Teams
          </h1>
          <p className="text-muted text-sm mt-1">{teams.length} teams registered</p>
        </div>

        {teams.length === 0 ? (
          <div className="text-center text-muted py-20 text-sm">
            No teams yet — use the Ingest page to add game data
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {teams.map(team => (
              <Link
                key={team.id}
                href={`/teams/${team.id}`}
                className="bg-surface border border-dim rounded-lg p-5 hover:border-accent transition-colors group"
              >
                <div className="flex items-start justify-between mb-3">
                  <span className="text-2xl">🏀</span>
                  <span className="text-xs font-bold bg-overlay px-2 py-1 rounded text-accent font-mono">
                    {team.season}
                  </span>
                </div>
                <h2 className="text-white font-bold text-lg group-hover:text-accent transition-colors">
                  {team.name}
                </h2>
                <p className="text-muted text-xs mt-2">View analytics →</p>
              </Link>
            ))}
          </div>
        )}
      </div>
    </main>
  )
}
