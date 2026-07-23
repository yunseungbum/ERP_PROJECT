export type LoginRequest = {
  loginId: string
  password: string
}

export type LoginResponse = {
  userId: number
  name: string
  roles: UserRole[]
  accessToken: string
  expiresAt: string
}

export type LoginStatus = 'idle' | 'loading' | 'error'
import type { UserRole } from '../../shared/auth/roles'
