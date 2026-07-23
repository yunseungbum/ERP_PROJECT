export type DashboardNotice = {
  title: string
  description: string
  status: string
  isImportant?: boolean
}

export type DashboardSummary = {
  title: string
  description: string
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
  { label: '유니폼', icon: '10', path: '/uniforms' },
  { label: '물품 리스트', icon: '□', path: '/inventory' },
  { label: '포메이션', icon: '⚽', path: '/formations' },
]

export const dashboardNotices: DashboardNotice[] = [
  { title: '8월 회비 납부 안내', description: '8월 회비 납부를 8월 25일까지 완료해 주세요.', status: 'D-5', isImportant: true },
  { title: '토요일 경기 장소 변경 안내', description: '이번 주 토요일 경기가 잠실 보조구장에서 진행됩니다.', status: 'NEW', isImportant: true },
  { title: '유니폼 추가 주문 신청', description: '하반기 유니폼 추가 주문을 8월 20일까지 신청받습니다.', status: '07/22' },
]

export const dashboardSummaries: DashboardSummary[] = [
  { title: '회원정보', description: '등록 회원 128명', icon: '♙', path: '/members' },
  { title: '참석 현황', description: '이번 주 참석 18명', icon: '✓', path: '/attendance' },
  { title: '지출 내역', description: '이번 달 지출 1,250,000원', icon: '▥', path: '/expenses' },
  { title: '회비', description: '미납 회원 4명', icon: '₩', path: '/dues' },
  { title: '경기 일정', description: '이번 주 경기 2건', icon: '●', path: '/schedules' },
  { title: '유니폼', description: '보유 유니폼 35벌', icon: '10', path: '/uniforms' },
  { title: '물품 리스트', description: '등록 물품 27개', icon: '□', path: '/inventory' },
  { title: '포메이션 정리', description: '최근 포메이션 4-3-3', icon: '⚽', path: '/formations' },
]
