import { Link } from 'react-router-dom'
import { dashboardNotices, dashboardSummaries } from './dashboardData'
import './dashboard.css'

export function DashboardPage() {
  return (
    <main className="dashboard-main">
        <header className="dashboard-header">
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>팀 운영 대시보드</h1>
        </header>

        <section className="notice-panel" aria-labelledby="notice-title">
          <div className="section-heading">
            <div><span className="section-icon" aria-hidden="true">!</span><h2 id="notice-title">공지사항</h2></div>
            <button type="button">더보기 <span aria-hidden="true">›</span></button>
          </div>
          <div className="notice-list">
            {dashboardNotices.map((notice) => (
              <article className="notice-item" key={notice.title}>
                <span className="notice-dot" aria-hidden="true" />
                <strong>{notice.title}</strong>
                <p>{notice.description}</p>
                <span className={notice.isImportant ? 'notice-status is-important' : 'notice-status'}>{notice.status}</span>
              </article>
            ))}
          </div>
        </section>

        <section className="summary-grid" aria-label="ERP 주요 현황">
          {dashboardSummaries.map((summary) => (
            <Link className="summary-card" key={summary.title} to={summary.path}>
              <span className="summary-icon" aria-hidden="true">{summary.icon}</span>
              <strong>{summary.title}</strong>
              <span>{summary.description}</span>
            </Link>
          ))}
        </section>
    </main>
  )
}
