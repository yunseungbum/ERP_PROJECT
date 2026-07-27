import {
  apiDelete,
  apiGet,
  apiPost,
  apiPut,
} from '../../shared/api/apiClient'
import type {
  AnnouncementResponse,
  AnnouncementSaveRequest,
} from './announcementTypes'

export function getAnnouncements(): Promise<AnnouncementResponse[]> {
  return apiGet<AnnouncementResponse[]>('/api/announcements')
}

export function createAnnouncement(
  request: AnnouncementSaveRequest,
): Promise<AnnouncementResponse> {
  return apiPost<AnnouncementResponse, AnnouncementSaveRequest>(
    '/api/announcements',
    request,
  )
}

export function updateAnnouncement(
  announcementId: number,
  request: AnnouncementSaveRequest,
): Promise<AnnouncementResponse> {
  return apiPut<AnnouncementResponse, AnnouncementSaveRequest>(
    `/api/announcements/${announcementId}`,
    request,
  )
}

export function deleteAnnouncement(
  announcementId: number,
): Promise<void> {
  return apiDelete(`/api/announcements/${announcementId}`)
}
