import { Outlet } from 'react-router-dom'
import { Sidebar } from '../../features/dashboard/Sidebar'
import type { LoginResponse } from '../../features/login/loginTypes'
import '../../features/dashboard/dashboard.css'

type MainLayoutProps = {
  user: LoginResponse
  onLogout: () => void
}

export function MainLayout({ user, onLogout }: MainLayoutProps) {
  return (
    <div className="dashboard-layout">
      <Sidebar user={user} onLogout={onLogout} />
      <Outlet />
    </div>
  )
}
