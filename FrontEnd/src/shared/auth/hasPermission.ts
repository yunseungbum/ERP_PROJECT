import {
  FEATURE_PERMISSIONS,
  type PermissionAction,
  type PermissionFeature,
} from './permissions'
import type { UserRole } from './roles'

export function hasPermission(
  userRoles: readonly UserRole[],
  feature: PermissionFeature,
  action: PermissionAction,
) {
  const allowedRoles = FEATURE_PERMISSIONS[feature][action]

  return userRoles.some((role) => allowedRoles.includes(role))
}
