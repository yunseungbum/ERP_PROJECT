export type AttendanceMatrixResponse = {
  schedules: AttendanceScheduleResponse[]
  members: AttendanceMemberResponse[]
}

export type AttendanceScheduleResponse = {
  scheduleId: number
  startsAt: string
}

export type AttendanceMemberResponse = {
  memberId: number
  memberName: string
  isPaused: boolean
  attendanceRate: number | null
  attendances: AttendanceCellResponse[]
}

export type AttendanceCellResponse = {
  scheduleId: number
  status: 'O' | 'X' | '-'
}
