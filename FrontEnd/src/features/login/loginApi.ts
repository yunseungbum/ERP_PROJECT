import { apiGet, apiPost } from '../../shared/api/apiClient'
import type {
  CurrentUserResponse,
  LoginRequest,
  LoginResponse,
} from './loginTypes'

export function login(request: LoginRequest): Promise<LoginResponse> {
  return apiPost<LoginResponse, LoginRequest>('/api/auth/login', request)
}

export function getCurrentUser(): Promise<CurrentUserResponse> {
  return apiGet<CurrentUserResponse>('/api/auth/me')
}
