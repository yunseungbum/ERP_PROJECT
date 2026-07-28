import { apiGet, apiPut } from '../../shared/api/apiClient'
import type {
  AttendanceCellResponse,
  AttendanceMatrixResponse,
} from './attendanceTypes'

export function getAttendanceMatrix(): Promise<AttendanceMatrixResponse> {
  return apiGet<AttendanceMatrixResponse>('/api/attendance/matrix')
}

export function updateAttendance(
  scheduleId: number,
  memberId: number,
  status: 'O' | 'X' | '-',
): Promise<AttendanceCellResponse> {
  return apiPut<AttendanceCellResponse, { status: 'O' | 'X' | '-' }>(
    `/api/attendance/schedules/${scheduleId}/members/${memberId}`,
    { status },
  )
}
