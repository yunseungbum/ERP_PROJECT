import { useState } from 'react'
import { BrowserRouter } from 'react-router-dom'
import type { LoginResponse } from '../features/login/loginTypes'
import {
  clearAccessToken,
  setAccessToken,
} from '../shared/auth/accessTokenStore'
import { AppRoutes } from './AppRoutes'

export function App() {
  const [currentUser, setCurrentUser] = useState<LoginResponse | null>(null)

  const handleLoginSuccess = (response: LoginResponse) => {
    setAccessToken(response.accessToken)
    setCurrentUser(response)
  }

  const handleLogout = () => {
    clearAccessToken()
    setCurrentUser(null)
  }

  return (
    <BrowserRouter>
      <AppRoutes
        currentUser={currentUser}
        onLoginSuccess={handleLoginSuccess}
        onLogout={handleLogout}
      />
    </BrowserRouter>
  )
}
