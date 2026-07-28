import { useEffect, useState, type FormEvent } from 'react'
import { ApiError } from '../../shared/api/apiClient'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import {
  createExpense,
  deleteExpense,
  getExpenses,
  updateExpense,
  updateExpenseSettlement,
} from './expenseApi'
import type {
  ExpenseResponse,
  ExpenseSaveRequest,
  ExpenseSummaryResponse,
} from './expenseTypes'
import './expenses.css'

type ExpensesPageProps = {
  userRoles: readonly UserRole[]
}

const unsettledPayerNames = [
  '윤승범',
  '김찬욱',
  '윤진혁',
  '홍준수',
  '김주빈',
] as const

const payerNameOptions = [
  ...unsettledPayerNames,
  '회비',
] as const

type ExpenseFormValues = {
  expenseItem: string
  amount: string
  paymentDate: string
  notes: string
  payerName: string
  isSettled: boolean
}

const emptyFormValues: ExpenseFormValues = {
  expenseItem: '',
  amount: '',
  paymentDate: '',
  notes: '',
  payerName: '',
  isSettled: false,
}

export function ExpensesPage({ userRoles }: ExpensesPageProps) {
  const canWriteExpenses = hasPermission(userRoles, 'expenses', 'write')
  const [summary, setSummary] = useState<ExpenseSummaryResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [expandedExpenseId, setExpandedExpenseId] =
    useState<number | null>(null)
  const [savingExpenseId, setSavingExpenseId] =
    useState<number | null>(null)
  const [editingExpense, setEditingExpense] =
    useState<ExpenseResponse | null>(null)
  const [formValues, setFormValues] =
    useState<ExpenseFormValues>(emptyFormValues)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    void loadExpenses()
  }, [])

  async function loadExpenses() {
    setIsLoading(true)
    setErrorMessage('')

    try {
      setSummary(await getExpenses())
    } catch (error) {
      setErrorMessage(getExpenseErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  async function handleSettlementChange(
    expenseId: number,
    isSettled: boolean,
  ) {
    setSavingExpenseId(expenseId)

    try {
      await updateExpenseSettlement(expenseId, isSettled)
      setSummary(await getExpenses())
    } catch (error) {
      window.alert(getExpenseErrorMessage(error))
    } finally {
      setSavingExpenseId(null)
    }
  }

  function openCreateModal() {
    setEditingExpense(null)
    setFormValues(emptyFormValues)
    setIsModalOpen(true)
  }

  function openEditModal(expense: ExpenseResponse) {
    if (expense.scheduleId !== null) return

    setEditingExpense(expense)
    setFormValues({
      expenseItem: expense.expenseItem,
      amount: String(expense.amount),
      paymentDate: expense.paymentDate.slice(0, 10),
      notes: expense.notes,
      payerName: expense.payerName,
      isSettled: expense.isSettled,
    })
    setIsModalOpen(true)
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!formValues.expenseItem.trim() ||
        !formValues.amount ||
        !formValues.paymentDate ||
        !formValues.payerName) {
      window.alert(
        '지출항목, 금액, 결제일, 결제 인원은 필수입니다.',
      )
      return
    }

    const request: ExpenseSaveRequest = {
      expenseItem: formValues.expenseItem.trim(),
      amount: Number(formValues.amount),
      paymentDate: formValues.paymentDate,
      notes: formValues.notes.trim(),
      payerName: formValues.payerName,
      isSettled: formValues.isSettled,
    }

    setIsSaving(true)

    try {
      if (editingExpense) {
        await updateExpense(editingExpense.expenseId, request)
      } else {
        await createExpense(request)
      }

      setIsModalOpen(false)
      setSummary(await getExpenses())
    } catch (error) {
      window.alert(getExpenseErrorMessage(error))
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete(expense: ExpenseResponse) {
    if (expense.scheduleId !== null) return

    const confirmed = window.confirm(
      `${expense.expenseItem} 지출내역을 삭제할까요?`,
    )
    if (!confirmed) return

    try {
      await deleteExpense(expense.expenseId)
      setSummary(await getExpenses())
    } catch (error) {
      window.alert(getExpenseErrorMessage(error))
    }
  }

  return (
    <main className="dashboard-main expenses-page">
      <header className="expenses-header">
        <div>
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>지출 내역</h1>
          <span>등록 내역 {summary?.expenses.length ?? 0}건</span>
        </div>
        {canWriteExpenses && (
          <button
            type="button"
            className="expense-add-button"
            onClick={openCreateModal}
          >
            + 지출 추가
          </button>
        )}
      </header>

      <section className="unsettled-panel">
        <h2>미정산 내역</h2>
        <div className="unsettled-grid">
          {unsettledPayerNames.map((payerName) => (
            <article key={payerName}>
              <span>{payerName}</span>
              <strong>
                {(summary?.unsettledAmounts[payerName] ?? 0)
                  .toLocaleString('ko-KR')}원
              </strong>
            </article>
          ))}
        </div>
      </section>

      <section className="expenses-panel">
        <div className="expenses-list-header">
          <h2>지출 내용</h2>
          <p className="expense-total">
            지출 합계
            <strong>
              {(summary?.totalAmount ?? 0).toLocaleString('ko-KR')}원
            </strong>
          </p>
        </div>
        {isLoading && (
          <p className="expenses-message">지출 내역을 불러오는 중입니다.</p>
        )}
        {errorMessage && (
          <div className="expenses-message is-error">
            <p>{errorMessage}</p>
            <button type="button" onClick={() => void loadExpenses()}>
              다시 시도
            </button>
          </div>
        )}
        {!isLoading && !errorMessage && summary?.expenses.length === 0 && (
          <p className="expenses-message">등록된 지출 내역이 없습니다.</p>
        )}
        {!isLoading && !errorMessage && summary &&
          summary.expenses.length > 0 && (
          <div className="expenses-table-wrap">
            <table className="expenses-table">
              <thead>
                <tr>
                  <th>NO</th>
                  <th>지출항목</th>
                  <th>금액</th>
                  <th>결제일</th>
                  <th>비고</th>
                  <th>결제 인원</th>
                  <th>정산 여부</th>
                  {canWriteExpenses && <th>관리</th>}
                </tr>
              </thead>
              <tbody>
                {summary.expenses.map((expense, index) => (
                  <tr key={expense.expenseId}>
                    <td>{index + 1}</td>
                    <td className="expense-item">{expense.expenseItem}</td>
                    <td>{expense.amount.toLocaleString('ko-KR')}원</td>
                    <td>{formatPaymentDate(expense.paymentDate)}</td>
                    <td className="expense-notes">
                      <button
                        type="button"
                        className={
                          expandedExpenseId === expense.expenseId
                            ? 'expense-notes-button is-expanded'
                            : 'expense-notes-button'
                        }
                        onClick={() =>
                          setExpandedExpenseId((currentId) =>
                            currentId === expense.expenseId
                              ? null
                              : expense.expenseId,
                          )
                        }
                      >
                        {expense.notes || '-'}
                      </button>
                    </td>
                    <td>{expense.payerName}</td>
                    <td>
                      {canWriteExpenses ? (
                        <select
                          className={
                            expense.isSettled
                              ? 'settlement-select is-yes'
                              : 'settlement-select is-no'
                          }
                          value={expense.isSettled ? 'true' : 'false'}
                          disabled={savingExpenseId === expense.expenseId}
                          onChange={(event) =>
                            void handleSettlementChange(
                              expense.expenseId,
                              event.target.value === 'true',
                            )
                          }
                        >
                          <option value="false">X</option>
                          <option value="true">O</option>
                        </select>
                      ) : (
                        <strong className={
                          expense.isSettled
                            ? 'settlement-text is-yes'
                            : 'settlement-text is-no'
                        }>
                          {expense.isSettled ? 'O' : 'X'}
                        </strong>
                      )}
                    </td>
                    {canWriteExpenses && (
                      <td>
                        {expense.scheduleId === null ? (
                          <div className="expense-actions">
                            <button
                              type="button"
                              onClick={() => openEditModal(expense)}
                            >
                              수정
                            </button>
                            <button
                              type="button"
                              className="is-delete"
                              onClick={() => void handleDelete(expense)}
                            >
                              삭제
                            </button>
                          </div>
                        ) : (
                          <span className="schedule-linked-label">
                            경기 일정 연동
                          </span>
                        )}
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>

      {isModalOpen && (
        <div className="expense-modal-backdrop" role="presentation">
          <div
            className="expense-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="expense-modal-title"
          >
            <header>
              <h2 id="expense-modal-title">
                지출 {editingExpense ? '수정' : '추가'}
              </h2>
              <button
                type="button"
                aria-label="닫기"
                onClick={() => setIsModalOpen(false)}
              >
                ×
              </button>
            </header>

            <form onSubmit={(event) => void handleSubmit(event)}>
              <div className="expense-form-grid">
                <label>
                  <span>지출항목 *</span>
                  <input
                    value={formValues.expenseItem}
                    maxLength={100}
                    onChange={(event) =>
                      setFormValues({
                        ...formValues,
                        expenseItem: event.target.value,
                      })
                    }
                    required
                  />
                </label>
                <label>
                  <span>금액 *</span>
                  <input
                    type="number"
                    min="0"
                    step="1"
                    value={formValues.amount}
                    onChange={(event) =>
                      setFormValues({
                        ...formValues,
                        amount: event.target.value,
                      })
                    }
                    required
                  />
                </label>
                <label>
                  <span>결제일 *</span>
                  <input
                    type="date"
                    value={formValues.paymentDate}
                    onChange={(event) =>
                      setFormValues({
                        ...formValues,
                        paymentDate: event.target.value,
                      })
                    }
                    required
                  />
                </label>
                <label>
                  <span>결제 인원 *</span>
                  <select
                    value={formValues.payerName}
                    onChange={(event) =>
                      setFormValues({
                        ...formValues,
                        payerName: event.target.value,
                      })
                    }
                    required
                  >
                    <option value="">선택해 주세요</option>
                    {payerNameOptions.map((payerName) => (
                      <option key={payerName} value={payerName}>
                        {payerName}
                      </option>
                    ))}
                  </select>
                </label>
                <label>
                  <span>정산 여부</span>
                  <select
                    value={formValues.isSettled ? 'true' : 'false'}
                    onChange={(event) =>
                      setFormValues({
                        ...formValues,
                        isSettled: event.target.value === 'true',
                      })
                    }
                  >
                    <option value="false">X - 미정산</option>
                    <option value="true">O - 정산</option>
                  </select>
                </label>
                <label className="expense-notes-field">
                  <span>비고</span>
                  <textarea
                    rows={4}
                    maxLength={1000}
                    value={formValues.notes}
                    onChange={(event) =>
                      setFormValues({
                        ...formValues,
                        notes: event.target.value,
                      })
                    }
                  />
                </label>
              </div>

              <div className="expense-modal-actions">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                >
                  취소
                </button>
                <button
                  type="submit"
                  className="is-primary"
                  disabled={isSaving}
                >
                  {isSaving ? '저장 중...' : '저장'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </main>
  )
}

function formatPaymentDate(value: string) {
  return new Intl.DateTimeFormat('ko-KR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).format(new Date(value))
}

function getExpenseErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401) {
      return '로그인이 만료되었습니다. 다시 로그인해 주세요.'
    }
    if (error.status === 403) {
      return '지출 내역을 변경할 권한이 없습니다.'
    }
    return error.message
  }

  return '지출 내역 서버에 연결할 수 없습니다.'
}
