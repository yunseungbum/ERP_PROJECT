import { Navigate, Route, Routes } from 'react-router-dom'
import { AttendancePage } from '../features/attendance/AttendancePage'
import { DashboardPage } from '../features/dashboard/DashboardPage'
import { DuesPage } from '../features/dues/DuesPage'
import { ExpensesPage } from '../features/expenses/ExpensesPage'
import { FormationPage } from '../features/formation/FormationPage'
import { InventoryPage } from '../features/inventory/InventoryPage'
import { LoginPage } from '../features/login/LoginPage'
import type { LoginResponse } from '../features/login/loginTypes'
import { MembersPage } from '../features/members/MembersPage'
import { MemberFormPage } from '../features/members/MemberFormPage'
import { SchedulePage } from '../features/schedule/SchedulePage'
import { MainLayout } from '../shared/layout/MainLayout'

type AppRoutesProps = {
  currentUser: LoginResponse | null
  onLoginSuccess: (user: LoginResponse) => void
  onLogout: () => void
}

export function AppRoutes({
  currentUser,
  onLoginSuccess,
  onLogout,
}: AppRoutesProps) {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          currentUser
            ? <Navigate to="/dashboard" replace />
            : <LoginPage onLoginSuccess={onLoginSuccess} />
        }
      />

      <Route
        element={
          currentUser
            ? <MainLayout user={currentUser} onLogout={onLogout} />
            : <Navigate to="/login" replace />
        }
      >
        <Route
          path="/dashboard"
          element={<DashboardPage userRoles={currentUser?.roles ?? []} />}
        />
        <Route
          path="/members"
          element={<MembersPage userRoles={currentUser?.roles ?? []} />}
        />
        <Route
          path="/members/new"
          element={<MemberFormPage userRoles={currentUser?.roles ?? []} />}
        />
        <Route
          path="/members/:memberId/edit"
          element={<MemberFormPage userRoles={currentUser?.roles ?? []} />}
        />
        <Route
          path="/attendance"
          element={<AttendancePage userRoles={currentUser?.roles ?? []} />}
        />
        <Route
          path="/schedules"
          element={<SchedulePage userRoles={currentUser?.roles ?? []} />}
        />
        <Route path="/dues" element={<DuesPage />} />
        <Route
          path="/expenses"
          element={<ExpensesPage userRoles={currentUser?.roles ?? []} />}
        />
        <Route path="/uniforms" element={<Navigate to="/inventory" replace />} />
        <Route
          path="/inventory"
          element={<InventoryPage userRoles={currentUser?.roles ?? []} />}
        />
        <Route
          path="/formations"
          element={<FormationPage userRoles={currentUser?.roles ?? []} />}
        />
      </Route>

      <Route
        path="*"
        element={<Navigate to={currentUser ? '/dashboard' : '/login'} replace />}
      />
    </Routes>
  )
}
