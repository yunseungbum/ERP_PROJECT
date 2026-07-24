export type FormationCode =
  | '4-2-3-1'
  | '4-1-2-3'
  | '4-5-1'
  | '4-3-3'

export type FormationBoardResponse = {
  scheduleId: number
  matchTitle: string
  startsAt: string
  participants: MatchParticipantResponse[]
  quarters: QuarterFormationResponse[]
}

export type MatchParticipantResponse = {
  participantId: number
  memberId: number | null
  participantName: string
  isGuest: boolean
  quarterParticipation: boolean[]
}

export type QuarterFormationResponse = {
  quarterNumber: number
  formationCode: FormationCode
  players: LineupPlayerResponse[]
  updatedAt: string | null
}

export type LineupPlayerResponse = {
  participantId: number
  slotCode: string
  positionOrder: number
}

export type AddMemberParticipantsRequest = {
  memberIds: number[]
}

export type AddGuestParticipantRequest = {
  guestName: string
}

export type SaveQuarterFormationRequest = {
  formationCode: FormationCode
  players: LineupPlayerResponse[]
}
