import type { LoginResponse } from './loginTypes'

type LoginSuccessCardProps = {
  user: LoginResponse
  onLogout: () => void
}

export function LoginSuccessCard({
  user,
  onLogout,
}: LoginSuccessCardProps) {
  return (
    <main className="login-page">
      <section className="login-card success-card">
        <p className="eyebrow">Buddy FC ERP</p>
        <h1>{user.name}님, 환영합니다.</h1>
        <p className="login-description">
          백엔드 임시 계정 검증을 통과했습니다.
        </p>

        <dl className="user-summary">
          <div>
            <dt>사용자 ID</dt>
            <dd>{user.userId}</dd>
          </div>
          <div>
            <dt>역할</dt>
            <dd>{user.roles.join(', ')}</dd>
          </div>
        </dl>

        <button type="button" className="secondary-button" onClick={onLogout}>
          로그인 화면으로 돌아가기
        </button>

        <p className="temporary-notice">
          현재는 토큰이 없는 임시 로그인 단계이므로 새로고침하면 로그인 상태가
          초기화됩니다.
        </p>
      </section>
    </main>
  )
}
