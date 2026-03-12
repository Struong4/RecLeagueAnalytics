import type { Metadata } from 'next'
import './globals.css'
import Navbar from '@/components/Navbar'

export const metadata: Metadata = {
  title: 'RecLeague Analytics',
  description: 'Recreational basketball league statistics and analytics',
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="bg-canvas text-white min-h-screen">
        <Navbar />
        <div className="pt-14">{children}</div>
      </body>
    </html>
  )
}
