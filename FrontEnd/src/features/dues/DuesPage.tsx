import { useEffect, useState } from 'react'
import { ApiError } from '../../shared/api/apiClient'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import {
  getDuesMatrix,
  updateDuesSummaryNote,
  updateMemberDue,
  updateMemberDueExecution,
  updateMemberDueNote,
} from './duesApi'
import type { DuesMatrixResponse } from './duesTypes'
import './dues.css'

type DuesPageProps = {
  userRoles: readonly UserRole[]
}

const currentYear = new Date().getFullYear()
const yearOptions = Array.from(
  { length: Math.max(2030 - currentYear + 1, 1) },
  (_, index) => currentYear + index,
)

export function DuesPage({ userRoles }: DuesPageProps) {
  const canWriteDues = hasPermission(userRoles, 'dues', 'write')
  const [selectedYear, setSelectedYear] = useState(currentYear)
  const [matrix, setMatrix] = useState<DuesMatrixResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [savingCellKey, setSavingCellKey] = useState<string | null>(null)
  const [noteMember, setNoteMember] = useState<{
    memberId: number
    memberName: string
  } | null>(null)
  const [noteContent, setNoteContent] = useState('')
  const [isNoteSaving, setIsNoteSaving] = useState(false)
  const [executionDrafts, setExecutionDrafts] = useState<
    Record<number, string>
  >({})
  const [savingExecutionMemberId, setSavingExecutionMemberId] =
    useState<number | null>(null)
  const [isSummaryNoteOpen, setIsSummaryNoteOpen] = useState(false)
  const [summaryNoteContent, setSummaryNoteContent] = useState('')
  const [isSummaryNoteSaving, setIsSummaryNoteSaving] = useState(false)

  useEffect(() => {
    void loadDues(selectedYear)
  }, [selectedYear])

  async function loadDues(year: number) {
    setIsLoading(true)
    setErrorMessage('')

    try {
      const response = await getDuesMatrix(year)
      setMatrix(response)
      setExecutionDrafts(
        Object.fromEntries(
          response.members.map((member) => [
            member.memberId,
            String(member.executionAmount),
          ]),
        ),
      )
      setSummaryNoteContent(response.summaryNote)
    } catch (error) {
      setErrorMessage(getDuesErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  async function handleDueChange(
    memberId: number,
    month: number,
    currentStatus: 'O' | 'X' | '-' | '·',
  ) {
    const nextStatus: 'O' | 'X' | '-' =
      currentStatus === 'O'
        ? 'X'
        : currentStatus === 'X'
          ? '-'
          : 'O'
    const cellKey = `${memberId}-${month}`
    setSavingCellKey(cellKey)

    try {
      await updateMemberDue(
        memberId,
        selectedYear,
        month,
        nextStatus,
      )
      setMatrix(await getDuesMatrix(selectedYear))
    } catch (error) {
      window.alert(getDuesErrorMessage(error))
    } finally {
      setSavingCellKey(null)
    }
  }

  function getMonthlyPaidAmount(month: number) {
    if (!matrix) return 0

    return matrix.members.filter((member) =>
      member.dues.some((due) =>
        due.month === month && due.status === 'O',
      ),
    ).length * matrix.monthlyFee
  }

  function openNoteModal(
    memberId: number,
    memberName: string,
    note: string,
  ) {
    setNoteMember({ memberId, memberName })
    setNoteContent(note)
  }

  async function handleNoteSave() {
    if (!noteMember) return

    setIsNoteSaving(true)
    try {
      await updateMemberDueNote(
        noteMember.memberId,
        selectedYear,
        noteContent.trim(),
      )
      setNoteMember(null)
      setMatrix(await getDuesMatrix(selectedYear))
    } catch (error) {
      window.alert(getDuesErrorMessage(error))
    } finally {
      setIsNoteSaving(false)
    }
  }

  async function handleExecutionSave(memberId: number) {
    if (!matrix) return

    const amount = Number(executionDrafts[memberId] ?? 0)
    const member = matrix.members.find((item) => item.memberId === memberId)

    if (!Number.isFinite(amount) || amount < 0) {
      window.alert('집행액은 0 이상의 숫자로 입력해 주세요.')
      setExecutionDrafts((current) => ({
        ...current,
        [memberId]: String(member?.executionAmount ?? 0),
      }))
      return
    }

    if (amount === member?.executionAmount) return

    setSavingExecutionMemberId(memberId)
    try {
      await updateMemberDueExecution(memberId, selectedYear, amount)
      await loadDues(selectedYear)
    } catch (error) {
      window.alert(getDuesErrorMessage(error))
    } finally {
      setSavingExecutionMemberId(null)
    }
  }

  async function handleSummaryNoteSave() {
    setIsSummaryNoteSaving(true)
    try {
      await updateDuesSummaryNote(
        selectedYear,
        summaryNoteContent.trim(),
      )
      setIsSummaryNoteOpen(false)
      await loadDues(selectedYear)
    } catch (error) {
      window.alert(getDuesErrorMessage(error))
    } finally {
      setIsSummaryNoteSaving(false)
    }
  }

  return (
    <main className="dashboard-main dues-page">
      <header className="dues-header">
        <div>
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>회비 관리</h1>
          <span>월 회비 20,000원</span>
        </div>
        <label className="dues-year-selector">
          <span>조회 연도</span>
          <select
            value={selectedYear}
            onChange={(event) =>
              setSelectedYear(Number(event.target.value))
            }
          >
            {yearOptions.map((year) => (
              <option key={year} value={year}>{year}년</option>
            ))}
          </select>
        </label>
      </header>

      <section className="dues-summary" aria-label="회비 요약">
        <article>
          <span>집행액</span>
          <strong>
            {(matrix?.totalExecutionAmount ?? 0)
              .toLocaleString('ko-KR')}원
          </strong>
        </article>
        <article>
          <span>납부액</span>
          <strong className="is-paid">
            {(matrix?.totalPaidAmount ?? 0).toLocaleString('ko-KR')}원
          </strong>
        </article>
        <article>
          <span>지출액</span>
          <strong className="is-expense">
            {(matrix?.totalExpenseAmount ?? 0)
              .toLocaleString('ko-KR')}원
          </strong>
        </article>
        <article>
          <span>차액</span>
          <strong className={
            (matrix?.balanceAmount ?? 0) < 0
              ? 'is-unpaid'
              : 'is-paid'
          }>
            {(matrix?.balanceAmount ?? 0).toLocaleString('ko-KR')}원
          </strong>
        </article>
        <article>
          <span>비고</span>
          <button
            type="button"
            className="dues-summary-note-button"
            onClick={() => setIsSummaryNoteOpen(true)}
          >
            {matrix?.summaryNote || (canWriteDues ? '비고 작성' : '-')}
          </button>
        </article>
      </section>

      <section className="dues-panel">
        {isLoading && (
          <p className="dues-message">회비 현황을 불러오는 중입니다.</p>
        )}
        {errorMessage && (
          <div className="dues-message is-error">
            <p>{errorMessage}</p>
            <button
              type="button"
              onClick={() => void loadDues(selectedYear)}
            >
              다시 시도
            </button>
          </div>
        )}
        {!isLoading &&
          !errorMessage &&
          matrix &&
          matrix.members.length === 0 && (
          <p className="dues-message">등록된 회원이 없습니다.</p>
        )}
        {!isLoading &&
          !errorMessage &&
          matrix &&
          matrix.members.length > 0 && (
          <div className="dues-table-wrap">
            <table className="dues-table">
              <thead>
                <tr>
                  <th className="dues-fixed dues-fixed-no">NO</th>
                  <th className="dues-fixed dues-fixed-name">이름</th>
                  <th className="dues-fixed dues-fixed-uniform">
                    유니폼
                  </th>
                  <th className="dues-fixed dues-fixed-execution">
                    집행액
                  </th>
                  <th className="dues-fixed dues-fixed-paid">
                    납부 합계
                  </th>
                  <th className="dues-fixed dues-fixed-unpaid">
                    미납 금액
                  </th>
                  <th className="dues-fixed dues-fixed-note">메모</th>
                  {Array.from({ length: 12 }, (_, index) => (
                    <th key={index + 1}>{index + 1}/20</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {matrix.members.map((member, index) => (
                  <tr
                    key={member.memberId}
                    className={member.isPaused ? 'is-paused' : ''}
                  >
                    <td className="dues-fixed dues-fixed-no">
                      {index + 1}
                    </td>
                    <td className="dues-fixed dues-fixed-name dues-member-name">
                      {member.memberName}
                    </td>
                    <td className="dues-fixed dues-fixed-uniform">
                      {member.hasUniform ? 'O' : 'X'}
                    </td>
                    <td className="dues-fixed dues-fixed-execution">
                      {canWriteDues ? (
                        <input
                          type="number"
                          min="0"
                          step="1000"
                          className="dues-execution-input"
                          disabled={
                            savingExecutionMemberId === member.memberId
                          }
                          value={
                            executionDrafts[member.memberId] ??
                            String(member.executionAmount)
                          }
                          onChange={(event) =>
                            setExecutionDrafts((current) => ({
                              ...current,
                              [member.memberId]: event.target.value,
                            }))
                          }
                          onBlur={() =>
                            void handleExecutionSave(member.memberId)
                          }
                          onKeyDown={(event) => {
                            if (event.key === 'Enter') {
                              event.currentTarget.blur()
                            }
                          }}
                          aria-label={`${member.memberName} 집행액`}
                        />
                      ) : (
                        `${member.executionAmount
                          .toLocaleString('ko-KR')}원`
                      )}
                    </td>
                    <td className="dues-fixed dues-fixed-paid dues-paid-total">
                      {member.paidTotal.toLocaleString('ko-KR')}원
                    </td>
                    <td className="dues-fixed dues-fixed-unpaid dues-unpaid-total">
                      {member.unpaidTotal.toLocaleString('ko-KR')}원
                    </td>
                    <td className="dues-fixed dues-fixed-note">
                      <button
                        type="button"
                        className="dues-note-button"
                        onClick={() =>
                          openNoteModal(
                            member.memberId,
                            member.memberName,
                            member.note,
                          )
                        }
                      >
                        {member.note || (canWriteDues
                          ? '메모 작성'
                          : '-')}
                      </button>
                    </td>
                    {member.dues.map((due) => (
                      <td
                        key={due.month}
                        className={`dues-status is-${getStatusClass(
                          due.status,
                        )}`}
                      >
                        {canWriteDues &&
                        !(selectedYear === 2026 && due.month < 4) ? (
                          <button
                            type="button"
                            disabled={
                              savingCellKey ===
                              `${member.memberId}-${due.month}`
                            }
                            onClick={() =>
                              void handleDueChange(
                                member.memberId,
                                due.month,
                                due.status,
                              )
                            }
                          >
                            {due.status}
                          </button>
                        ) : (
                          due.status
                        )}
                      </td>
                    ))}
                  </tr>
                ))}
                <tr className="dues-total-row">
                  <th className="dues-fixed dues-fixed-no" />
                  <th className="dues-fixed dues-fixed-name">합계</th>
                  <th className="dues-fixed dues-fixed-uniform" />
                  <td className="dues-fixed dues-fixed-execution">
                    {matrix.totalExecutionAmount.toLocaleString('ko-KR')}
                  </td>
                  <td className="dues-fixed dues-fixed-paid">
                    {matrix.totalPaidAmount.toLocaleString('ko-KR')}
                  </td>
                  <td className="dues-fixed dues-fixed-unpaid">
                    {matrix.totalUnpaidAmount.toLocaleString('ko-KR')}
                  </td>
                  <td className="dues-fixed dues-fixed-note">-</td>
                  {Array.from({ length: 12 }, (_, index) => (
                    <td key={index + 1}>
                      {getMonthlyPaidAmount(index + 1)
                      .toLocaleString('ko-KR')}
                    </td>
                  ))}
                </tr>
              </tbody>
            </table>
          </div>
        )}
      </section>

      {isSummaryNoteOpen && (
        <div className="dues-note-backdrop" role="presentation">
          <div
            className="dues-note-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="dues-summary-note-title"
          >
            <header>
              <h2 id="dues-summary-note-title">
                {selectedYear}년 회비 비고
              </h2>
              <button
                type="button"
                aria-label="닫기"
                onClick={() => setIsSummaryNoteOpen(false)}
              >
                ×
              </button>
            </header>
            <div className="dues-note-body">
              {canWriteDues ? (
                <textarea
                  rows={6}
                  maxLength={1000}
                  value={summaryNoteContent}
                  placeholder="연도별 회비 관련 비고를 입력해 주세요."
                  onChange={(event) =>
                    setSummaryNoteContent(event.target.value)
                  }
                />
              ) : (
                <p>
                  {summaryNoteContent || '등록된 비고가 없습니다.'}
                </p>
              )}
              <div className="dues-note-actions">
                <button
                  type="button"
                  onClick={() => setIsSummaryNoteOpen(false)}
                >
                  {canWriteDues ? '취소' : '닫기'}
                </button>
                {canWriteDues && (
                  <button
                    type="button"
                    className="is-primary"
                    disabled={isSummaryNoteSaving}
                    onClick={() => void handleSummaryNoteSave()}
                  >
                    {isSummaryNoteSaving ? '저장 중...' : '저장'}
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>
      )}

      {noteMember && (
        <div className="dues-note-backdrop" role="presentation">
          <div
            className="dues-note-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="dues-note-title"
          >
            <header>
              <h2 id="dues-note-title">
                {noteMember.memberName} · {selectedYear}년 메모
              </h2>
              <button
                type="button"
                aria-label="닫기"
                onClick={() => setNoteMember(null)}
              >
                ×
              </button>
            </header>
            <div className="dues-note-body">
              {canWriteDues ? (
                <textarea
                  rows={6}
                  maxLength={1000}
                  value={noteContent}
                  placeholder="회비 관련 메모를 입력해 주세요."
                  onChange={(event) => setNoteContent(event.target.value)}
                />
              ) : (
                <p>{noteContent || '등록된 메모가 없습니다.'}</p>
              )}
              <div className="dues-note-actions">
                <button type="button" onClick={() => setNoteMember(null)}>
                  {canWriteDues ? '취소' : '닫기'}
                </button>
                {canWriteDues && (
                  <button
                    type="button"
                    className="is-primary"
                    disabled={isNoteSaving}
                    onClick={() => void handleNoteSave()}
                  >
                    {isNoteSaving ? '저장 중...' : '저장'}
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </main>
  )
}

function getStatusClass(status: 'O' | 'X' | '-' | '·') {
  if (status === 'O') return 'paid'
  if (status === 'X') return 'unpaid'
  if (status === '-') return 'exempt'
  return 'pending'
}

function getDuesErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401) {
      return '로그인이 만료되었습니다. 다시 로그인해 주세요.'
    }
    if (error.status === 403) {
      return '회비 현황을 변경할 권한이 없습니다.'
    }
    return error.message
  }

  return '회비 서버에 연결할 수 없습니다.'
}
