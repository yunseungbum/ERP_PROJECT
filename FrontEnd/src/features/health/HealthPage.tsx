import { HealthStatusCard } from './HealthStatusCard'
import { useHealthCheck } from './useHealthCheck'

export function HealthPage() {
  const { status, checkedAt, checkConnection } = useHealthCheck()

  return (
    <main className="connection-page">
      <HealthStatusCard
        status={status}
        checkedAt={checkedAt}
        onCheck={checkConnection}
      />
    </main>
  )
}
