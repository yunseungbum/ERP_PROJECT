import { useEffect, useState } from 'react'
import { BrowserRouter } from 'react-router-dom'
import { getCurrentUser } from '../features/login/loginApi'
import type { LoginResponse } from '../features/login/loginTypes'
import {
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from '../shared/auth/accessTokenStore'
import { AppRoutes } from './AppRoutes'

export function App() {
  const [currentUser, setCurrentUser] = useState<LoginResponse | null>(null)
  const [isRestoringLogin, setIsRestoringLogin] = useState(true)

  useEffect(() => {
    async function restoreLogin() {
      const storedAccessToken = getAccessToken()

      if (!storedAccessToken) {
        setIsRestoringLogin(false)
        return
      }

      try {
        const user = await getCurrentUser()
        setCurrentUser({
          userId: user.userId,
          name: user.name,
          roles: [user.role],
          accessToken: storedAccessToken,
          expiresAt: '',
        })
      } catch {
        clearAccessToken()
      } finally {
        setIsRestoringLogin(false)
      }
    }

    void restoreLogin()
  }, [])

  const handleLoginSuccess = (response: LoginResponse) => {
    setAccessToken(response.accessToken)
    setCurrentUser(response)
  }

  const handleLogout = () => {
    clearAccessToken()
    setCurrentUser(null)
  }

  if (isRestoringLogin) {
    return <div>로그인 정보를 확인하는 중입니다.</div>
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
