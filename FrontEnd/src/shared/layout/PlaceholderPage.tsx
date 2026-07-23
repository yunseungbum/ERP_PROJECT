type PlaceholderPageProps = {
  title: string
  description: string
}

export function PlaceholderPage({ title, description }: PlaceholderPageProps) {
  return (
    <main className="dashboard-main">
      <header className="dashboard-header">
        <p>Buddy FC 통합 관리 시스템</p>
        <h1>{title}</h1>
      </header>

      <section className="placeholder-panel">
        <span aria-hidden="true">준비 중</span>
        <h2>{title}</h2>
        <p>{description}</p>
      </section>
    </main>
  )
}
