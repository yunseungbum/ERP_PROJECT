import { useEffect, useState } from 'react'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import { getAttendanceMatrix, updateAttendance } from './attendanceApi'
import type { AttendanceMatrixResponse } from './attendanceTypes'
import './attendance.css'

function formatScheduleDate(startsAt: string) {
  const date = new Date(startsAt)
  return `${date.getMonth() + 1}/${date.getDate()}`
}

type AttendancePageProps = {
  userRoles: readonly UserRole[]
}

export function AttendancePage({ userRoles }: AttendancePageProps) {
  const canWriteAttendance = hasPermission(
    userRoles,
    'attendance',
    'write',
  )
  const [matrix, setMatrix] = useState<AttendanceMatrixResponse>({
    schedules: [],
    members: [],
  })
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [savingCellKey, setSavingCellKey] = useState('')

  useEffect(() => {
    async function loadAttendanceMatrix() {
      try {
        setMatrix(await getAttendanceMatrix())
      } catch {
        setErrorMessage('참석 현황을 불러오지 못했습니다.')
      } finally {
        setIsLoading(false)
      }
    }

    void loadAttendanceMatrix()
  }, [])

  function getScheduleTotal(scheduleId: number) {
    return matrix.members.filter((member) =>
      member.attendances.some((attendance) =>
        attendance.scheduleId === scheduleId &&
        attendance.status === 'O',
      ),
    ).length
  }

  async function handleAttendanceChange(
    memberId: number,
    scheduleId: number,
    currentStatus: 'O' | 'X',
  ) {
    const nextStatus: 'O' | 'X' =
      currentStatus === 'O' ? 'X' : 'O'
    const cellKey = `${scheduleId}-${memberId}`
    setSavingCellKey(cellKey)

    try {
      await updateAttendance(scheduleId, memberId, nextStatus)
      setMatrix((currentMatrix) => ({
        ...currentMatrix,
        members: currentMatrix.members.map((member) => {
          if (member.memberId !== memberId) return member

          const attendances = member.attendances.map((attendance) =>
            attendance.scheduleId === scheduleId
              ? { ...attendance, status: nextStatus }
              : attendance,
          )
          const applicableAttendances = attendances.filter(
            (attendance) => attendance.status !== '-',
          )
          const attendanceRate = applicableAttendances.length === 0
            ? null
            : Math.round(
                applicableAttendances.filter(
                  (attendance) => attendance.status === 'O',
                ).length *
                1000 /
                applicableAttendances.length,
              ) / 10

          return { ...member, attendances, attendanceRate }
        }),
      }))
    } catch {
      window.alert('참석 여부를 저장하지 못했습니다.')
    } finally {
      setSavingCellKey('')
    }
  }

  return (
    <section className="attendance-page">
      <header className="attendance-header">
        <div>
          <p className="attendance-eyebrow">ATTENDANCE</p>
          <h1>참석 현황</h1>
          <p>포메이션 참여 인원을 기준으로 경기별 참석 여부를 확인합니다.</p>
        </div>
      </header>

      <div className="attendance-table-card">
        {isLoading && <p className="attendance-state">참석 현황을 불러오는 중입니다.</p>}
        {!isLoading && errorMessage && (
          <p className="attendance-state attendance-state-error">{errorMessage}</p>
        )}
        {!isLoading && !errorMessage && matrix.members.length === 0 && (
          <p className="attendance-state">등록된 회원이 없습니다.</p>
        )}
        {!isLoading && !errorMessage && matrix.members.length > 0 && (
          <div className="attendance-table-scroll">
            <table className="attendance-table">
              <thead>
                <tr>
                  <th className="attendance-number-column attendance-fixed-column" rowSpan={2} scope="col">No.</th>
                  <th className="attendance-name-column attendance-fixed-column" rowSpan={2} scope="col">이름</th>
                  <th className="attendance-rate-column attendance-fixed-column" rowSpan={2} scope="col">참석률</th>
                  <th className="attendance-date-group" colSpan={Math.max(matrix.schedules.length, 1)} scope="colgroup">
                    경기 진행 일시
                  </th>
                </tr>
                <tr>
                  {matrix.schedules.length > 0 ? (
                    matrix.schedules.map((schedule) => (
                      <th className="attendance-date-column" key={schedule.scheduleId} scope="col">
                        {formatScheduleDate(schedule.startsAt)}
                      </th>
                    ))
                  ) : (
                    <th className="attendance-date-column" scope="col">-</th>
                  )}
                </tr>
              </thead>
              <tbody>
                {matrix.members.map((member, index) => (
                  <tr key={member.memberId}>
                    <td className="attendance-number-cell">{index + 1}</td>
                    <td className="attendance-member-name">{member.memberName}</td>
                    <td className="attendance-rate-cell">
                      {member.attendanceRate === null ? '-' : `${member.attendanceRate}%`}
                    </td>
                    {matrix.schedules.length > 0 ? (
                      matrix.schedules.map((schedule) => {
                        const attendance = member.attendances.find(
                          (item) => item.scheduleId === schedule.scheduleId,
                        )
                        const status = attendance?.status ?? '-'
                        const statusClass =
                          status === 'O'
                            ? 'is-present'
                            : status === 'X'
                              ? 'is-absent'
                              : 'is-excluded'

                        return (
                          <td className={`attendance-value-cell ${statusClass}`} key={schedule.scheduleId}>
                            {canWriteAttendance && status !== '-' ? (
                              <button
                                type="button"
                                className="attendance-status-button"
                                disabled={
                                  savingCellKey ===
                                  `${schedule.scheduleId}-${member.memberId}`
                                }
                                title="클릭하여 O/X 변경"
                                onClick={() => void handleAttendanceChange(
                                  member.memberId,
                                  schedule.scheduleId,
                                  status,
                                )}
                              >
                                {status}
                              </button>
                            ) : (
                              status
                            )}
                          </td>
                        )
                      })
                    ) : (
                      <td className="attendance-value-cell">-</td>
                    )}
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr>
                  <th colSpan={3}>Total</th>
                  {matrix.schedules.length > 0 ? (
                    matrix.schedules.map((schedule) => (
                      <td key={schedule.scheduleId}>{getScheduleTotal(schedule.scheduleId)}</td>
                    ))
                  ) : (
                    <td>0</td>
                  )}
                </tr>
              </tfoot>
            </table>
          </div>
        )}
      </div>
    </section>
  )
}
