import { useCallback, useEffect, useState } from 'react'
import { BrowserRouter } from 'react-router-dom'
import { getCurrentUser } from '../features/login/loginApi'
import type { LoginResponse } from '../features/login/loginTypes'
import {
  clearAccessToken,
  getAccessToken,
  getAccessTokenExpiresAt,
  setAccessToken,
} from '../shared/auth/accessTokenStore'
import { sessionExpiredEventName } from '../shared/auth/sessionExpiration'
import { AppRoutes } from './AppRoutes'

export function App() {
  const [currentUser, setCurrentUser] = useState<LoginResponse | null>(null)
  const [isRestoringLogin, setIsRestoringLogin] = useState(true)
  const [isSessionExpiredModalOpen, setIsSessionExpiredModalOpen] =
    useState(false)

  const expireSession = useCallback(() => {
    clearAccessToken()
    setCurrentUser(null)
    setIsSessionExpiredModalOpen(true)
  }, [])

  useEffect(() => {
    window.addEventListener(sessionExpiredEventName, expireSession)

    return () => {
      window.removeEventListener(sessionExpiredEventName, expireSession)
    }
  }, [expireSession])

  useEffect(() => {
    async function restoreLogin() {
      const storedAccessToken = getAccessToken()
      const storedExpiresAt = getAccessTokenExpiresAt()

      if (!storedAccessToken) {
        setIsRestoringLogin(false)
        return
      }

      if (storedExpiresAt &&
          new Date(storedExpiresAt).getTime() <= Date.now()) {
        expireSession()
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
          expiresAt: storedExpiresAt ?? '',
        })
      } catch {
        clearAccessToken()
      } finally {
        setIsRestoringLogin(false)
      }
    }

    void restoreLogin()
  }, [expireSession])

  useEffect(() => {
    if (!currentUser) return

    const expiresAt = currentUser.expiresAt || getAccessTokenExpiresAt()
    if (!expiresAt) return

    const millisecondsUntilExpiration =
      new Date(expiresAt).getTime() - Date.now()

    if (millisecondsUntilExpiration <= 0) {
      expireSession()
      return
    }

    const expirationTimer = window.setTimeout(
      expireSession,
      millisecondsUntilExpiration,
    )

    return () => window.clearTimeout(expirationTimer)
  }, [currentUser, expireSession])

  const handleLoginSuccess = (response: LoginResponse) => {
    setAccessToken(response.accessToken, response.expiresAt)
    setCurrentUser(response)
    setIsSessionExpiredModalOpen(false)
  }

  const handleLogout = () => {
    clearAccessToken()
    setCurrentUser(null)
  }

  if (isRestoringLogin) {
    return <div>로그인 정보를 확인하는 중입니다.</div>
  }

  return (
    <>
      <BrowserRouter>
        <AppRoutes
          currentUser={currentUser}
          onLoginSuccess={handleLoginSuccess}
          onLogout={handleLogout}
        />
      </BrowserRouter>

      {isSessionExpiredModalOpen && (
        <div className="session-expired-backdrop">
          <section
            className="session-expired-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="session-expired-title"
          >
            <h2 id="session-expired-title">로그아웃 되었습니다</h2>
            <p>로그인 유지시간이 만료되었습니다. 다시 로그인해 주세요.</p>
            <button
              type="button"
              onClick={() => setIsSessionExpiredModalOpen(false)}
            >
              확인
            </button>
          </section>
        </div>
      )}
    </>
  )
}
