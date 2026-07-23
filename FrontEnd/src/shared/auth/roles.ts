export const USER_ROLES = {
  president: 'President',
  director: 'Director',
  coach: 'Coach',
  treasurer: 'Treasurer',
  inventoryManager: 'InventoryManager',
  member: 'Member',
} as const

export type UserRole = (typeof USER_ROLES)[keyof typeof USER_ROLES]

export const ALL_ROLES: readonly UserRole[] = Object.values(USER_ROLES)

export const OPERATOR_ROLES: readonly UserRole[] = [
  USER_ROLES.president,
  USER_ROLES.director,
  USER_ROLES.coach,
  USER_ROLES.treasurer,
  USER_ROLES.inventoryManager,
]
