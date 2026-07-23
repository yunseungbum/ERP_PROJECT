import { apiGet } from '../../shared/api/apiClient'
import type { HealthResponse } from './healthTypes'

export function getHealth(): Promise<HealthResponse> {
  return apiGet<HealthResponse>('/api/health')
}
