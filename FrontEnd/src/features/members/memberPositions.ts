import type { MemberPosition } from './memberTypes'

export const memberPositionLabels: Record<MemberPosition, string> = {
  Goalkeeper: '키퍼',
  WingBack: '윙백',
  CenterBack: '센터백',
  DefensiveMidfielder: '수미',
  CentralMidfielder: '중미',
  AttackingMidfielder: '공미',
  Winger: '윙어',
  Striker: '톱',
}

export const memberPositionOptions = Object.entries(memberPositionLabels).map(
  ([value, label]) => ({
    value: value as MemberPosition,
    label,
  }),
)
