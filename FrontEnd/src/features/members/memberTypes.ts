export type MemberPosition =
  | 'Goalkeeper'
  | 'WingBack'
  | 'CenterBack'
  | 'DefensiveMidfielder'
  | 'CentralMidfielder'
  | 'AttackingMidfielder'
  | 'Winger'
  | 'Striker'

export type MemberSaveRequest = {
  memberName: string
  primaryPosition: MemberPosition
  secondaryPosition: MemberPosition | null
  phoneNumber: string
  birthYear: number
  notes: string
}

export type MemberResponse = MemberSaveRequest & {
  memberId: number
  isActive: boolean
  createdAt: string
  updatedAt: string
}
