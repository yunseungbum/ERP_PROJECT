import { apiGet, apiPut } from '../../shared/api/apiClient'
import type {
  DuesCellResponse,
  DuesMatrixResponse,
} from './duesTypes'

export function getDuesMatrix(
  year: number,
): Promise<DuesMatrixResponse> {
  return apiGet<DuesMatrixResponse>(`/api/dues?year=${year}`)
}

export function updateMemberDue(
  memberId: number,
  year: number,
  month: number,
  status: 'O' | 'X' | '-',
): Promise<DuesCellResponse> {
  return apiPut<DuesCellResponse, { status: 'O' | 'X' | '-' }>(
    `/api/dues/${year}/${month}/members/${memberId}`,
    { status },
  )
}

export function updateMemberDueNote(
  memberId: number,
  year: number,
  content: string,
): Promise<{ content: string }> {
  return apiPut<{ content: string }, { content: string }>(
    `/api/dues/${year}/members/${memberId}/note`,
    { content },
  )
}
