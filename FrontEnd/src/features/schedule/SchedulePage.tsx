import { useEffect, useState, type FormEvent } from 'react'
import { ApiError } from '../../shared/api/apiClient'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import {
  createSchedule,
  deleteSchedule,
  getSchedules,
  updateSchedule,
} from './scheduleApi'
import type {
  ScheduleResponse,
  ScheduleSaveRequest,
} from './scheduleTypes'
import './schedule.css'

type SchedulePageProps = {
  userRoles: readonly UserRole[]
}

type ScheduleFormValues = {
  startsAt: string
  opponentName: string
  venueName: string
  matchFee: string
  isMatchFeePaid: boolean
  payerName: string
  notes: string
  isCompleted: boolean
  opponentContact: string
}

const emptyFormValues: ScheduleFormValues = {
  startsAt: '',
  opponentName: '',
  venueName: '',
  matchFee: '',
  isMatchFeePaid: false,
  payerName: '',
  notes: '',
  isCompleted: false,
  opponentContact: '',
}

const hourlyTimeOptions = Array.from(
  { length: 24 },
  (_, hour) => `${String(hour).padStart(2, '0')}:00`,
)

const payerNameOptions = [
  '윤승범',
  '김찬욱',
  '윤진혁',
  '홍준수',
  '김주빈',
] as const

export function SchedulePage({ userRoles }: SchedulePageProps) {
  const canWriteSchedules = hasPermission(userRoles, 'schedules', 'write')
  const [schedules, setSchedules] = useState<ScheduleResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [editingSchedule, setEditingSchedule] =
    useState<ScheduleResponse | null>(null)
  const [formValues, setFormValues] =
    useState<ScheduleFormValues>(emptyFormValues)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const nextSchedule = schedules.find(
    (schedule) => new Date(schedule.startsAt).getTime() >= Date.now(),
  )
  const [expandedNoteScheduleId, setExpandedNoteScheduleId] =
    useState<number | null>(null)

  useEffect(() => {
    void loadSchedules()
  }, [])

  async function loadSchedules() {
    setIsLoading(true)
    setErrorMessage('')

    try {
      setSchedules(await getSchedules())
    } catch (error) {
      setErrorMessage(getScheduleErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  function openCreateModal() {
    setEditingSchedule(null)
    setFormValues(emptyFormValues)
    setIsModalOpen(true)
  }

  function openEditModal(schedule: ScheduleResponse) {
    setEditingSchedule(schedule)
    setFormValues({
      startsAt: `${schedule.startsAt.slice(0, 13)}:00`,
      opponentName: schedule.opponentName,
      venueName: schedule.venueName,
      matchFee: String(schedule.matchFee),
      isMatchFeePaid: schedule.isMatchFeePaid,
      payerName: schedule.payerName,
      notes: schedule.notes,
      isCompleted: schedule.isCompleted,
      opponentContact: schedule.opponentContact ?? '',
    })
    setIsModalOpen(true)
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!formValues.startsAt ||
        !formValues.opponentName.trim() ||
        !formValues.venueName.trim() ||
        !formValues.payerName ||
        formValues.matchFee === '') {
      window.alert(
        '일자, 시간, 상대팀, 장소, 매칭비, 결제 인원은 필수입니다.',
      )
      return
    }

    const request: ScheduleSaveRequest = {
      startsAt: formValues.startsAt,
      opponentName: formValues.opponentName.trim(),
      venueName: formValues.venueName.trim(),
      matchFee: Number(formValues.matchFee),
      isMatchFeePaid: formValues.isMatchFeePaid,
      payerName: formValues.payerName,
      notes: formValues.notes.trim(),
      isCompleted: formValues.isCompleted,
      opponentContact: formValues.opponentContact.trim() || null,
    }

    setIsSaving(true)

    try {
      const savedSchedule = editingSchedule
        ? await updateSchedule(editingSchedule.scheduleId, request)
        : await createSchedule(request)

      setSchedules((currentSchedules) => {
        const nextSchedules = editingSchedule
          ? currentSchedules.map((schedule) =>
              schedule.scheduleId === savedSchedule.scheduleId
                ? savedSchedule
                : schedule)
          : [...currentSchedules, savedSchedule]

        return nextSchedules.sort(
          (first, second) =>
            new Date(first.startsAt).getTime() -
            new Date(second.startsAt).getTime(),
        )
      })
      setIsModalOpen(false)
    } catch (error) {
      window.alert(getScheduleErrorMessage(error))
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete(schedule: ScheduleResponse) {
    const confirmed = window.confirm(
      `${formatDateTime(schedule.startsAt)} ${schedule.opponentName} 일정을 삭제할까요?\n연결된 포메이션과 참여 인원도 함께 삭제됩니다.`,
    )

    if (!confirmed) return

    try {
      await deleteSchedule(schedule.scheduleId)
      setSchedules((currentSchedules) =>
        currentSchedules.filter(
          (currentSchedule) =>
            currentSchedule.scheduleId !== schedule.scheduleId,
        ),
      )
    } catch (error) {
      window.alert(getScheduleErrorMessage(error))
    }
  }

  return (
    <main className="dashboard-main schedule-page">
      <header className="schedule-header">
        <div>
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>경기 일정</h1>
          <span>등록 일정 {schedules.length}건</span>
        </div>
        <div className="schedule-header-actions">
          <p className="schedule-next-match">
            {nextSchedule ? (
              <>
                * 다음 일정은{' '}
                <strong>
                  {formatDate(nextSchedule.startsAt)}{' '}
                  {formatTime(nextSchedule.startsAt)}{' '}
                  {nextSchedule.venueName} (vs {nextSchedule.opponentName})
                </strong>
                입니다.
              </>
            ) : (
              <span>* 다음 일정이 등록되어 있지 않습니다.</span>
            )}
          </p>
          {canWriteSchedules && (
            <button
              className="schedule-add-button"
              type="button"
              onClick={openCreateModal}
            >
              + 경기 일정 추가
            </button>
          )}
        </div>
      </header>

      <section className="schedule-panel">
        {isLoading && <p className="schedule-message">경기 일정을 불러오는 중입니다.</p>}
        {errorMessage && (
          <div className="schedule-message is-error">
            <p>{errorMessage}</p>
            <button type="button" onClick={() => void loadSchedules()}>다시 시도</button>
          </div>
        )}
        {!isLoading && !errorMessage && schedules.length === 0 && (
          <p className="schedule-message">등록된 경기 일정이 없습니다.</p>
        )}
        {!isLoading && !errorMessage && schedules.length > 0 && (
          <div className="schedule-table-wrap">
            <table className="schedule-table">
              <thead>
                <tr>
                  <th>일자</th>
                  <th>시간</th>
                  <th>상대팀</th>
                  <th>장소</th>
                  <th>매칭비</th>
                  <th>입금 여부</th>
                  <th>결제 인원</th>
                  <th>비고</th>
                  <th>진행 여부</th>
                  <th>상대팀 연락처</th>
                  {canWriteSchedules && <th>관리</th>}
                </tr>
              </thead>
              <tbody>
                {schedules.map((schedule) => (
                  <tr key={schedule.scheduleId}>
                    <td>{formatDate(schedule.startsAt)}</td>
                    <td>{formatTime(schedule.startsAt)}</td>
                    <td className="schedule-opponent">{schedule.opponentName}</td>
                    <td>{schedule.venueName}</td>
                    <td>{schedule.matchFee.toLocaleString('ko-KR')}원</td>
                    <td><StatusMark value={schedule.isMatchFeePaid} /></td>
                    <td>{schedule.payerName}</td>
                    <td className="schedule-notes">
                      {schedule.notes ? (
                        <button
                          type="button"
                          className={
                            expandedNoteScheduleId === schedule.scheduleId
                              ? 'schedule-notes-toggle is-expanded'
                              : 'schedule-notes-toggle'
                          }
                          aria-expanded={
                            expandedNoteScheduleId === schedule.scheduleId
                          }
                          onClick={() =>
                            setExpandedNoteScheduleId((currentScheduleId) =>
                              currentScheduleId === schedule.scheduleId
                                ? null
                                : schedule.scheduleId,
                            )
                          }
                        >
                          {schedule.notes}
                        </button>
                      ) : (
                        '-'
                      )}
                    </td>
                    <td><StatusMark value={schedule.isCompleted} /></td>
                    <td>{schedule.opponentContact || '-'}</td>
                    {canWriteSchedules && (
                      <td>
                        <div className="schedule-actions">
                          <button type="button" onClick={() => openEditModal(schedule)}>수정</button>
                          <button className="is-delete" type="button" onClick={() => void handleDelete(schedule)}>삭제</button>
                        </div>
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
        <div className="schedule-modal-backdrop" role="presentation">
          <div className="schedule-modal" role="dialog" aria-modal="true" aria-labelledby="schedule-modal-title">
            <header>
              <h2 id="schedule-modal-title">
                경기 일정 {editingSchedule ? '수정' : '추가'}
              </h2>
              <button type="button" aria-label="닫기" onClick={() => setIsModalOpen(false)}>×</button>
            </header>

            <form onSubmit={(event) => void handleSubmit(event)}>
              <div className="schedule-form-grid">
                <label>
                  <span>일자 및 시간 *</span>
                  <div className="schedule-date-time-fields">
                    <input
                      aria-label="경기 일자"
                      type="date"
                      value={formValues.startsAt.slice(0, 10)}
                      onChange={(event) => {
                        const time = formValues.startsAt.slice(11, 16)
                        setFormValues({
                          ...formValues,
                          startsAt: event.target.value
                            ? `${event.target.value}T${time || '00:00'}`
                            : '',
                        })
                      }}
                      required
                    />
                    <select
                      aria-label="경기 시간"
                      value={formValues.startsAt.slice(11, 16)}
                      onChange={(event) => {
                        const date = formValues.startsAt.slice(0, 10)
                        setFormValues({
                          ...formValues,
                          startsAt: date
                            ? `${date}T${event.target.value}`
                            : '',
                        })
                      }}
                      disabled={!formValues.startsAt.slice(0, 10)}
                      required
                    >
                      {hourlyTimeOptions.map((time) => (
                        <option key={time} value={time}>{time}</option>
                      ))}
                    </select>
                  </div>
                </label>
                <label>
                  <span>상대팀 *</span>
                  <input value={formValues.opponentName} onChange={(event) => setFormValues({ ...formValues, opponentName: event.target.value })} maxLength={100} required />
                </label>
                <label>
                  <span>장소 *</span>
                  <input value={formValues.venueName} onChange={(event) => setFormValues({ ...formValues, venueName: event.target.value })} maxLength={100} required />
                </label>
                <label>
                  <span>매칭비 *</span>
                  <input type="number" min="0" step="1" value={formValues.matchFee} onChange={(event) => setFormValues({ ...formValues, matchFee: event.target.value })} required />
                </label>
                <label>
                  <span>매칭비 입금 여부</span>
                  <select value={formValues.isMatchFeePaid ? 'true' : 'false'} onChange={(event) => setFormValues({ ...formValues, isMatchFeePaid: event.target.value === 'true' })}>
                    <option value="false">X - 미입금</option>
                    <option value="true">O - 입금</option>
                  </select>
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
                  <span>진행 여부</span>
                  <select value={formValues.isCompleted ? 'true' : 'false'} onChange={(event) => setFormValues({ ...formValues, isCompleted: event.target.value === 'true' })}>
                    <option value="false">X - 미진행</option>
                    <option value="true">O - 진행</option>
                  </select>
                </label>
                <label>
                  <span>상대팀 연락처</span>
                  <input value={formValues.opponentContact} onChange={(event) => setFormValues({ ...formValues, opponentContact: event.target.value })} maxLength={30} />
                </label>
                <label className="schedule-notes-field">
                  <span>비고</span>
                  <textarea value={formValues.notes} onChange={(event) => setFormValues({ ...formValues, notes: event.target.value })} maxLength={1000} rows={4} />
                </label>
              </div>

              <div className="schedule-modal-actions">
                <button type="button" onClick={() => setIsModalOpen(false)}>취소</button>
                <button className="is-primary" type="submit" disabled={isSaving}>
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

function StatusMark({ value }: { value: boolean }) {
  return (
    <span className={value ? 'schedule-status is-yes' : 'schedule-status is-no'}>
      {value ? 'O' : 'X'}
    </span>
  )
}

function formatDate(startsAt: string) {
  const date = new Date(startsAt)
  const weekdays = ['일', '월', '화', '수', '목', '금', '토']
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')

  return `${year}.${month}.${day}(${weekdays[date.getDay()]})`
}

function formatTime(startsAt: string) {
  return new Intl.DateTimeFormat('ko-KR', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
  }).format(new Date(startsAt))
}

function formatDateTime(startsAt: string) {
  return `${formatDate(startsAt)} ${formatTime(startsAt)}`
}

function getScheduleErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401) return '로그인이 만료되었습니다. 다시 로그인해 주세요.'
    if (error.status === 403) return '경기 일정을 변경할 권한이 없습니다.'
    if (error.status === 404) return '경기 일정을 찾을 수 없습니다.'
    return error.message
  }

  return '경기 일정 서버에 연결할 수 없습니다.'
}
