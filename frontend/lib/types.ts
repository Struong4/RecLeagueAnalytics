export interface Player {
  id: number
  name: string
  position: string
  jerseyNumber: number
  teamId: number
}

// TeamDto.season is a string in the C# DTO
export interface Team {
  id: number
  name: string
  season: string
}

// GameDto.season is an int in the C# DTO
export interface Game {
  id: number
  gameDate: string
  homeTeamId: number
  awayTeamId: number
  homeScore: number
  awayScore: number
  location: string
  season: number
}

export interface PlayerAnalytics {
  playerId: number
  playerName: string
  teamName: string
  gamesPlayed: number
  pointsPerGame: number
  reboundsPerGame: number
  assistsPerGame: number
  stealsPerGame: number
  blocksPerGame: number
  // Percentages are 0–1 range from the API — multiply by 100 before displaying
  trueShootingPct: number | null
  fieldGoalPct: number | null
  threePointPct: number | null
  freeThrowPct: number | null
  astToRatio: number | null
  per36: number | null
  usageRate: number | null
}

export interface PlayerTrend {
  gameId: number
  gameDate: string
  points: number
  trueShootingPct: number | null
  per36: number | null
  rollingAvgTsPct: number | null
  rollingAvgPer36: number | null
}

export interface TeamAnalytics {
  teamId: number
  teamName: string
  season: string
  gamesPlayed: number
  wins: number
  losses: number
  winPct: number
  pointsPerGame: number
  reboundsPerGame: number
  assistsPerGame: number
  offensiveEfficiency: number
  defensiveEfficiency: number
  netEfficiency: number
}

export interface LeaderboardEntry {
  playerId: number
  playerName: string
  teamName: string
  value: number
}

export interface SeasonSummary {
  season: number
  totalGames: number
  totalTeams: number
  totalPlayers: number
  topScorer: LeaderboardEntry | null
  topEfficiency: LeaderboardEntry | null
}

// Combined type for the main stats table — PlayerAnalytics + position/jersey from Player
export interface PlayerRow extends PlayerAnalytics {
  position: string
  jerseyNumber: number
}
