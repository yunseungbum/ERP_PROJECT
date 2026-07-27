export type DashboardSummary = {
  title: string
  icon: string
  path: string
}

export const dashboardMenuItems = [
  { label: '대시보드', icon: '⌂', path: '/dashboard' },
  { label: '회원정보', icon: '♙', path: '/members' },
  { label: '참석 현황', icon: '✓', path: '/attendance' },
  { label: '경기 일정', icon: '●', path: '/schedules' },
  { label: '회비', icon: '₩', path: '/dues' },
  { label: '지출 내역', icon: '▥', path: '/expenses' },
  { label: '물품 리스트', icon: '□', path: '/inventory' },
  { label: '포메이션', icon: '⚽', path: '/formations' },
]

export const dashboardSummaries: DashboardSummary[] = [
  { title: '회원정보', icon: '♙', path: '/members' },
  { title: '참석 현황', icon: '✓', path: '/attendance' },
  { title: '지출 내역', icon: '▥', path: '/expenses' },
  { title: '회비', icon: '₩', path: '/dues' },
  { title: '경기 일정', icon: '●', path: '/schedules' },
  { title: '물품 리스트', icon: '□', path: '/inventory' },
  { title: '포메이션 정리', icon: '⚽', path: '/formations' },
]
