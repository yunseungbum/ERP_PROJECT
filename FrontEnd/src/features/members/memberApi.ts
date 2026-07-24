import {
  apiDelete,
  apiGet,
  apiPost,
  apiPut,
} from '../../shared/api/apiClient'
import type {
  MemberResponse,
  MemberSaveRequest,
} from './memberTypes'

export function getMembers(): Promise<MemberResponse[]> {
  return apiGet<MemberResponse[]>('/api/members')
}

export function getMember(memberId: number): Promise<MemberResponse> {
  return apiGet<MemberResponse>(`/api/members/${memberId}`)
}

export function createMember(
  request: MemberSaveRequest,
): Promise<MemberResponse> {
  return apiPost<MemberResponse, MemberSaveRequest>(
    '/api/members',
    request,
  )
}

export function updateMember(
  memberId: number,
  request: MemberSaveRequest,
): Promise<MemberResponse> {
  return apiPut<MemberResponse, MemberSaveRequest>(
    `/api/members/${memberId}`,
    request,
  )
}

export function deleteMember(memberId: number): Promise<void> {
  return apiDelete(`/api/members/${memberId}`)
}
