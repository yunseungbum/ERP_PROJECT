import { LoginForm } from './LoginForm'
import { useLogin } from './useLogin'
import type { LoginResponse } from './loginTypes'
import './login.css'

type LoginPageProps = {
  onLoginSuccess: (response: LoginResponse) => void
}

export function LoginPage({ onLoginSuccess }: LoginPageProps) {
  const { status, errorMessage, submitLogin } = useLogin(onLoginSuccess)

  return (
    <main className="login-page">
      <section className="login-card">
        <p className="eyebrow">Buddy FC ERP</p>
        <h1>운영진 로그인</h1>
        <p className="login-description">
          회장, 총무, 감독 및 물품관리자 계정으로 로그인해 주세요.
        </p>

        <LoginForm
          status={status}
          serverError={errorMessage}
          onSubmit={submitLogin}
        />

        <div className="test-account">
          <strong>공개 체험 계정</strong>
          <span>아이디: guest</span>
          <span>비밀번호: 1234</span>
          <span>권한: 일반 팀원과 동일한 읽기 전용</span>
        </div>
      </section>
    </main>
  )
}
