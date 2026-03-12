import type {
  Player,
  Team,
  Game,
  PlayerAnalytics,
  PlayerTrend,
  TeamAnalytics,
  LeaderboardEntry,
  SeasonSummary,
} from './types'

// Server components (Node.js) need an absolute URL — relative URLs don't resolve in Node.js.
// Client components (browser) use a relative path so the Next.js rewrite proxy handles CORS.
const getBase = () =>
  typeof window === 'undefined'
    ? `${process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:8080'}/api`
    : '/api'

async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${getBase()}${path}`, {
    ...options,
    // Disable Next.js's default fetch caching for all calls; pages opt in via revalidate
    cache: options?.method ? 'no-store' : 'no-store',
  })
  if (!res.ok) {
    const body = await res.text().catch(() => '')
    throw new Error(`${res.status}${body ? ': ' + body : ''}`)
  }
  return res.json() as Promise<T>
}

// Players
export const getPlayers   = ()           => apiFetch<Player[]>('/players')
export const getPlayer    = (id: number) => apiFetch<Player>(`/players/${id}`)

// Teams
export const getTeams     = ()           => apiFetch<Team[]>('/teams')
export const getTeam      = (id: number) => apiFetch<Team>(`/teams/${id}`)

// Games
export const getGames     = ()           => apiFetch<Game[]>('/games')

// Analytics
export const getPlayerAnalytics = (id: number) =>
  apiFetch<PlayerAnalytics>(`/analytics/player/${id}`)

export const getPlayerTrends = (id: number) =>
  apiFetch<PlayerTrend[]>(`/analytics/player/${id}/trends`)

export const getTeamAnalytics = (id: number) =>
  apiFetch<TeamAnalytics>(`/analytics/team/${id}`)

// Leaderboards
export const getScoringLeaderboard    = () =>
  apiFetch<LeaderboardEntry[]>('/analytics/leaderboards/scoring')

export const getEfficiencyLeaderboard = () =>
  apiFetch<LeaderboardEntry[]>('/analytics/leaderboards/efficiency')

// Season summary — year param is an integer matching GameDto.season
export const getSeasonSummary = (year: number) =>
  apiFetch<SeasonSummary>(`/analytics/season/${year}/summary`)

// Ingestion — POST /api/ingestion/game (not /api/ingestion)
export const ingestGame = (payload: unknown) =>
  apiFetch<unknown>('/ingestion/game', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
