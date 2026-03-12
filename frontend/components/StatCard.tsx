interface StatCardProps {
  label: string
  value: string | number
  sub?: string
  highlight?: boolean
  positive?: boolean   // green tint
  negative?: boolean   // red tint
}

export default function StatCard({
  label,
  value,
  sub,
  highlight = false,
  positive = false,
  negative = false,
}: StatCardProps) {
  const valueColor = highlight
    ? 'text-accent'
    : positive
    ? 'text-green-400'
    : negative
    ? 'text-red-400'
    : 'text-white'

  return (
    <div className="bg-surface border border-dim rounded-lg p-4 flex flex-col gap-1">
      <span className="text-muted text-xs uppercase tracking-widest">{label}</span>
      <span className={`text-2xl font-bold font-mono ${valueColor}`}>{value}</span>
      {sub && <span className="text-muted text-xs">{sub}</span>}
    </div>
  )
}
