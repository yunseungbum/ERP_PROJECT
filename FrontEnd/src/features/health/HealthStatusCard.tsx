import { getApiUrl } from '../../shared/api/apiClient'
import type { ApiStatus } from './healthTypes'
import './health.css'

type HealthStatusCardProps = {
  status: ApiStatus
  checkedAt: string | null
  onCheck: () => Promise<void>
}

export function HealthStatusCard({
  status,
  checkedAt,
  onCheck,
}: HealthStatusCardProps) {
  const isLoading = status === 'loading'

  return (
    <section className="connection-card">
      <p className="eyebrow">Buddy FC ERP</p>
      <h1>개발 환경 연결 확인</h1>
      <p className="description">
        로그인 기능을 만들기 전에 React가 ASP.NET Core API와 통신할 수
        있는지 먼저 확인합니다.
      </p>

      <div className={`status-box status-${status}`} aria-live="polite">
        <span className="status-dot" aria-hidden="true" />
        <div>
          <strong>{statusText[status]}</strong>
          {checkedAt && <p>확인 시각: {checkedAt}</p>}
        </div>
      </div>

      <button type="button" onClick={onCheck} disabled={isLoading}>
        {isLoading ? '확인 중...' : 'API 연결 확인'}
      </button>

      <p className="endpoint">GET {getApiUrl('/api/health')}</p>
    </section>
  )
}

const statusText: Record<ApiStatus, string> = {
  idle: '아직 연결을 확인하지 않았습니다.',
  loading: '백엔드에 요청을 보내고 있습니다.',
  success: '프론트엔드와 백엔드가 정상 연결되었습니다.',
  error: '백엔드 연결에 실패했습니다.',
}
