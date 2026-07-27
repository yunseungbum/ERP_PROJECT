export type ExpenseResponse = {
  expenseId: number
  scheduleId: number | null
  expenseItem: string
  amount: number
  paymentDate: string
  notes: string
  payerName: string
  isSettled: boolean
}

export type ExpenseSummaryResponse = {
  totalAmount: number
  unsettledAmounts: Record<string, number>
  expenses: ExpenseResponse[]
}

export type ExpenseSaveRequest = {
  expenseItem: string
  amount: number
  paymentDate: string
  notes: string
  payerName: string
  isSettled: boolean
}
