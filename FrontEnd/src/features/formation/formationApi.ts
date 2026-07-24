import {
  apiDeleteWithResponse,
  apiGet,
  apiPost,
  apiPut,
} from '../../shared/api/apiClient'
import type {
  AddGuestParticipantRequest,
  AddMemberParticipantsRequest,
  FormationBoardResponse,
  SaveQuarterFormationRequest,
} from './formationTypes'

export function getUpcomingFormationBoard(): Promise<FormationBoardResponse> {
  return apiGet<FormationBoardResponse>('/api/formations/upcoming')
}

export function addMemberParticipants(
  scheduleId: number,
  request: AddMemberParticipantsRequest,
): Promise<FormationBoardResponse> {
  return apiPost<FormationBoardResponse, AddMemberParticipantsRequest>(
    `/api/formations/${scheduleId}/participants/members`,
    request,
  )
}

export function addGuestParticipant(
  scheduleId: number,
  request: AddGuestParticipantRequest,
): Promise<FormationBoardResponse> {
  return apiPost<FormationBoardResponse, AddGuestParticipantRequest>(
    `/api/formations/${scheduleId}/participants/guests`,
    request,
  )
}

export function removeParticipant(
  scheduleId: number,
  participantId: number,
): Promise<FormationBoardResponse> {
  return apiDeleteWithResponse<FormationBoardResponse>(
    `/api/formations/${scheduleId}/participants/${participantId}`,
  )
}

export function saveQuarterFormation(
  scheduleId: number,
  quarterNumber: number,
  request: SaveQuarterFormationRequest,
): Promise<FormationBoardResponse> {
  return apiPut<FormationBoardResponse, SaveQuarterFormationRequest>(
    `/api/formations/${scheduleId}/quarters/${quarterNumber}`,
    request,
  )
}
