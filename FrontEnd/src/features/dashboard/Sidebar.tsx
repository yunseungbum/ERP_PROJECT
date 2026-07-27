import type { LoginResponse } from '../login/loginTypes'
import { NavLink } from 'react-router-dom'
import buddyErpBrand from '../../assets/buddy-erp-brand.png'
import { dashboardMenuItems } from './dashboardData'

type SidebarProps = {
  user: LoginResponse
  onLogout: () => void
}

export function Sidebar({ user, onLogout }: SidebarProps) {
  return (
    <aside className="dashboard-sidebar">
      <NavLink
        className="sidebar-brand"
        to="/dashboard"
        aria-label="대시보드로 이동"
      >
        <span className="brand-mark">
          <img src={buddyErpBrand} alt="Buddy ERP" />
        </span>
        <div><strong>Buddy FC</strong><span>Manager ERP</span></div>
      </NavLink>

      <nav className="sidebar-menu" aria-label="ERP 주요 메뉴">
        {dashboardMenuItems.map((item) => (
          <NavLink
            className={({ isActive }) => isActive ? 'sidebar-menu-item is-active' : 'sidebar-menu-item'}
            key={item.path}
            to={item.path}
          >
            <span className="menu-icon" aria-hidden="true">{item.icon}</span>
            <span>{item.label}</span>
          </NavLink>
        ))}
      </nav>

      <div className="sidebar-user">
        <div className="user-avatar" aria-hidden="true">{user.name.slice(0, 1)}</div>
        <div className="sidebar-user-info"><strong>{user.name}</strong><span>{user.roles.join(', ')}</span></div>
        <button type="button" className="logout-button" onClick={onLogout}>로그아웃</button>
      </div>
    </aside>
  )
}
