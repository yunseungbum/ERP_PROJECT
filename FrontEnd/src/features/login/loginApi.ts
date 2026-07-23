import { apiPost } from '../../shared/api/apiClient'
import type { LoginRequest, LoginResponse } from './loginTypes'

export function login(request: LoginRequest): Promise<LoginResponse> {
  return apiPost<LoginResponse, LoginRequest>('/api/auth/login', request)
}
