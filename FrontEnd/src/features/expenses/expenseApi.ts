import {
  apiDelete,
  apiGet,
  apiPatch,
  apiPost,
  apiPut,
} from '../../shared/api/apiClient'
import type {
  ExpenseResponse,
  ExpenseSaveRequest,
  ExpenseSummaryResponse,
} from './expenseTypes'

export function getExpenses(): Promise<ExpenseSummaryResponse> {
  return apiGet<ExpenseSummaryResponse>('/api/expenses')
}

export function updateExpenseSettlement(
  expenseId: number,
  isSettled: boolean,
): Promise<ExpenseResponse> {
  return apiPatch<ExpenseResponse, { isSettled: boolean }>(
    `/api/expenses/${expenseId}/settlement`,
    { isSettled },
  )
}

export function createExpense(
  request: ExpenseSaveRequest,
): Promise<ExpenseResponse> {
  return apiPost<ExpenseResponse, ExpenseSaveRequest>(
    '/api/expenses',
    request,
  )
}

export function updateExpense(
  expenseId: number,
  request: ExpenseSaveRequest,
): Promise<ExpenseResponse> {
  return apiPut<ExpenseResponse, ExpenseSaveRequest>(
    `/api/expenses/${expenseId}`,
    request,
  )
}

export function deleteExpense(expenseId: number): Promise<void> {
  return apiDelete(`/api/expenses/${expenseId}`)
}
