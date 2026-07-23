import {
  ALL_ROLES,
  OPERATOR_ROLES,
  USER_ROLES,
  type UserRole,
} from './roles'

export type PermissionAction = 'read' | 'write'

export type PermissionFeature =
  | 'members'
  | 'attendance'
  | 'attendanceRate'
  | 'schedules'
  | 'dues'
  | 'expenses'
  | 'uniforms'
  | 'inventory'
  | 'formations'

type FeaturePermissions = Record<
  PermissionFeature,
  Record<PermissionAction, readonly UserRole[]>
>

const presidentAndTreasurer: readonly UserRole[] = [
  USER_ROLES.president,
  USER_ROLES.treasurer,
]

const formationManagers: readonly UserRole[] = [
  USER_ROLES.president,
  USER_ROLES.director,
  USER_ROLES.coach,
]

export const FEATURE_PERMISSIONS: FeaturePermissions = {
  members: { read: ALL_ROLES, write: OPERATOR_ROLES },
  attendance: { read: ALL_ROLES, write: OPERATOR_ROLES },
  attendanceRate: { read: ALL_ROLES, write: OPERATOR_ROLES },
  schedules: { read: ALL_ROLES, write: OPERATOR_ROLES },
  dues: { read: ALL_ROLES, write: presidentAndTreasurer },
  expenses: { read: ALL_ROLES, write: presidentAndTreasurer },
  uniforms: { read: ALL_ROLES, write: OPERATOR_ROLES },
  inventory: { read: ALL_ROLES, write: OPERATOR_ROLES },
  formations: { read: ALL_ROLES, write: formationManagers },
}
