import { useState } from 'react'
import { getHealth } from './healthApi'
import type { ApiStatus } from './healthTypes'

export function useHealthCheck() {
  const [status, setStatus] = useState<ApiStatus>('idle')
  const [checkedAt, setCheckedAt] = useState<string | null>(null)

  const checkConnection = async () => {
    setStatus('loading')

    try {
      const data = await getHealth()

      setCheckedAt(new Date(data.checkedAt).toLocaleString('ko-KR'))
      setStatus(data.status === 'ok' ? 'success' : 'error')
    } catch {
      setCheckedAt(null)
      setStatus('error')
    }
  }

  return {
    status,
    checkedAt,
    checkConnection,
  }
}
