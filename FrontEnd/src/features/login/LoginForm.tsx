import { useState, type FormEvent } from 'react'
import type { LoginRequest, LoginStatus } from './loginTypes'

type LoginFormProps = {
  status: LoginStatus
  serverError: string | null
  onSubmit: (request: LoginRequest) => Promise<void>
}

type LoginFieldErrors = {
  loginId?: string
  password?: string
}

export function LoginForm({
  status,
  serverError,
  onSubmit,
}: LoginFormProps) {
  const [loginId, setLoginId] = useState('')
  const [password, setPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState<LoginFieldErrors>({})
  const isLoading = status === 'loading'

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const trimmedLoginId = loginId.trim()
    const validationErrors = validateLogin(trimmedLoginId, password)

    setFieldErrors(validationErrors)

    if (Object.keys(validationErrors).length > 0) {
      return
    }

    await onSubmit({
      loginId: trimmedLoginId,
      password,
    })
  }

  return (
    <form className="login-form" onSubmit={handleSubmit} noValidate>
      <div className="form-field">
        <label htmlFor="loginId">아이디</label>
        <input
          id="loginId"
          name="loginId"
          type="text"
          value={loginId}
          onChange={(event) => setLoginId(event.target.value)}
          aria-invalid={Boolean(fieldErrors.loginId)}
          aria-describedby={fieldErrors.loginId ? 'loginId-error' : undefined}
          autoComplete="username"
          disabled={isLoading}
        />
        {fieldErrors.loginId && (
          <p id="loginId-error" className="field-error">
            {fieldErrors.loginId}
          </p>
        )}
      </div>

      <div className="form-field">
        <label htmlFor="password">비밀번호</label>
        <input
          id="password"
          name="password"
          type="password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          aria-invalid={Boolean(fieldErrors.password)}
          aria-describedby={fieldErrors.password ? 'password-error' : undefined}
          autoComplete="current-password"
          disabled={isLoading}
        />
        {fieldErrors.password && (
          <p id="password-error" className="field-error">
            {fieldErrors.password}
          </p>
        )}
      </div>

      {serverError && (
        <div className="server-error" role="alert">
          {serverError}
        </div>
      )}

      <button type="submit" disabled={isLoading}>
        {isLoading ? '로그인 중...' : '로그인'}
      </button>
    </form>
  )
}

function validateLogin(
  loginId: string,
  password: string,
): LoginFieldErrors {
  const errors: LoginFieldErrors = {}

  if (!loginId) {
    errors.loginId = '아이디를 입력해 주세요.'
  }

  if (!password) {
    errors.password = '비밀번호를 입력해 주세요.'
  }

  return errors
}
