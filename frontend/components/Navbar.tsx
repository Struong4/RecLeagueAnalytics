'use client'

import Link from 'next/link'
import { usePathname } from 'next/navigation'

const LINKS = [
  { href: '/',           label: 'Stats'      },
  { href: '/teams',      label: 'Teams'      },
  { href: '/analytics',  label: 'Analytics'  },
  { href: '/ingest',     label: 'Ingest'     },
]

export default function Navbar() {
  const pathname = usePathname()

  return (
    <header className="fixed top-0 left-0 right-0 z-50 h-14 bg-surface border-b border-dim flex items-center px-6 gap-8">
      {/* Logo */}
      <Link href="/" className="flex items-center gap-2 shrink-0">
        <span className="text-accent text-xl">🏀</span>
        <span className="font-bold text-white text-sm tracking-wide">RecLeague</span>
      </Link>

      {/* Nav links */}
      <nav className="flex items-center gap-1">
        {LINKS.map(({ href, label }) => {
          const active =
            href === '/' ? pathname === '/' : pathname.startsWith(href)
          return (
            <Link
              key={href}
              href={href}
              className={`px-3 py-1.5 text-sm rounded transition-colors ${
                active
                  ? 'text-accent border-b-2 border-accent pb-[5px]'
                  : 'text-muted hover:text-white'
              }`}
            >
              {label}
            </Link>
          )
        })}
      </nav>
    </header>
  )
}
