import {
  apiDelete,
  apiGet,
  apiPost,
  apiPut,
} from '../../shared/api/apiClient'
import type {
  ScheduleResponse,
  ScheduleSaveRequest,
} from './scheduleTypes'

export function getSchedules(): Promise<ScheduleResponse[]> {
  return apiGet<ScheduleResponse[]>('/api/schedules')
}

export function createSchedule(
  request: ScheduleSaveRequest,
): Promise<ScheduleResponse> {
  return apiPost<ScheduleResponse, ScheduleSaveRequest>(
    '/api/schedules',
    request,
  )
}

export function updateSchedule(
  scheduleId: number,
  request: ScheduleSaveRequest,
): Promise<ScheduleResponse> {
  return apiPut<ScheduleResponse, ScheduleSaveRequest>(
    `/api/schedules/${scheduleId}`,
    request,
  )
}

export function deleteSchedule(scheduleId: number): Promise<void> {
  return apiDelete(`/api/schedules/${scheduleId}`)
}
