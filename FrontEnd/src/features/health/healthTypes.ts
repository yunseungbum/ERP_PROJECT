export type ApiStatus = 'idle' | 'loading' | 'success' | 'error'

export type HealthResponse = {
  status: string
  checkedAt: string
}
