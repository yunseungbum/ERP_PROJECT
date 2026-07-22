import { useState } from 'react'
import './App.css'

type ApiStatus = 'idle' | 'loading' | 'success' | 'error'

type HealthResponse = {
  status: string
  checkedAt: string
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

function App() {
  const [apiStatus, setApiStatus] = useState<ApiStatus>('idle')
  const [checkedAt, setCheckedAt] = useState<string | null>(null)

  const checkApiConnection = async () => {
    setApiStatus('loading')

    try {
      const response = await fetch(`${apiBaseUrl}/api/health`)

      if (!response.ok) {
        throw new Error(`API request failed: ${response.status}`)
      }

      const data: HealthResponse = await response.json()
      setCheckedAt(new Date(data.checkedAt).toLocaleString('ko-KR'))
      setApiStatus(data.status === 'ok' ? 'success' : 'error')
    } catch {
      setCheckedAt(null)
      setApiStatus('error')
    }
  }

  return (
    <main className="connection-page">
      <section className="connection-card">
        <p className="eyebrow">Buddy FC ERP</p>
        <h1>개발 환경 연결 확인</h1>
        <p className="description">
          로그인 기능을 만들기 전에 React가 ASP.NET Core API와 통신할 수
          있는지 먼저 확인합니다.
        </p>

        <div className={`status-box status-${apiStatus}`} aria-live="polite">
          <span className="status-dot" aria-hidden="true" />
          <div>
            <strong>{statusText[apiStatus]}</strong>
            {checkedAt && <p>확인 시각: {checkedAt}</p>}
          </div>
        </div>

        <button
          type="button"
          onClick={checkApiConnection}
          disabled={apiStatus === 'loading'}
        >
          {apiStatus === 'loading' ? '확인 중...' : 'API 연결 확인'}
        </button>

        <p className="endpoint">GET {apiBaseUrl}/api/health</p>
      </section>
    </main>
  )
}

const statusText: Record<ApiStatus, string> = {
  idle: '아직 연결을 확인하지 않았습니다.',
  loading: '백엔드에 요청을 보내고 있습니다.',
  success: '프론트엔드와 백엔드가 정상 연결되었습니다.',
  error: '백엔드 연결에 실패했습니다.',
}

export default App
