import { useState } from 'react'
import { ApiError } from '../../shared/api/apiClient'
import { login } from './loginApi'
import type {
  LoginRequest,
  LoginResponse,
  LoginStatus,
} from './loginTypes'

export function useLogin(
  onSuccess: (response: LoginResponse) => void,
) {
  const [status, setStatus] = useState<LoginStatus>('idle')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const submitLogin = async (request: LoginRequest) => {
    setStatus('loading')
    setErrorMessage(null)

    try {
      const response = await login(request)

      setStatus('idle')
      onSuccess(response)
    } catch (error) {
      setStatus('error')
      setErrorMessage(getLoginErrorMessage(error))
    }
  }

  return {
    status,
    errorMessage,
    submitLogin,
  }
}

function getLoginErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message
  }

  return '서버에 연결할 수 없습니다. 잠시 후 다시 시도해 주세요.'
}
