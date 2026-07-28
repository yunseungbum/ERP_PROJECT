export type DuesMatrixResponse = {
  year: number
  monthlyFee: number
  totalPaidAmount: number
  totalUnpaidAmount: number
  unpaidMemberCount: number
  members: DuesMemberResponse[]
}

export type DuesMemberResponse = {
  memberId: number
  memberName: string
  isPaused: boolean
  hasUniform: boolean
  paidTotal: number
  unpaidTotal: number
  note: string
  dues: DuesCellResponse[]
}

export type DuesCellResponse = {
  month: number
  dueDate: string
  status: 'O' | 'X' | '-' | '·'
}
