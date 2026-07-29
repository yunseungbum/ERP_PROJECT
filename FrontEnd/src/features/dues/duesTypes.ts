export type DuesMatrixResponse = {
  year: number
  monthlyFee: number
  totalExecutionAmount: number
  totalPaidAmount: number
  totalExpenseAmount: number
  balanceAmount: number
  totalUnpaidAmount: number
  unpaidMemberCount: number
  summaryNote: string
  members: DuesMemberResponse[]
}

export type DuesMemberResponse = {
  memberId: number
  memberName: string
  isPaused: boolean
  hasUniform: boolean
  executionAmount: number
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
