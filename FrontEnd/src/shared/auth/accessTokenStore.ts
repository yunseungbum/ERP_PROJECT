const accessTokenStorageKey = 'buddyErp.accessToken'
const accessTokenExpiresAtStorageKey = 'buddyErp.accessTokenExpiresAt'

let accessToken: string | null =
  window.localStorage.getItem(accessTokenStorageKey)
let accessTokenExpiresAt: string | null =
  window.localStorage.getItem(accessTokenExpiresAtStorageKey)

export function getAccessToken(): string | null {
  return accessToken
}

export function getAccessTokenExpiresAt(): string | null {
  return accessTokenExpiresAt ?? getJwtExpiresAt(accessToken)
}

export function setAccessToken(token: string, expiresAt: string): void {
  accessToken = token
  accessTokenExpiresAt = expiresAt
  window.localStorage.setItem(accessTokenStorageKey, token)
  window.localStorage.setItem(accessTokenExpiresAtStorageKey, expiresAt)
}

export function clearAccessToken(): void {
  accessToken = null
  accessTokenExpiresAt = null
  window.localStorage.removeItem(accessTokenStorageKey)
  window.localStorage.removeItem(accessTokenExpiresAtStorageKey)
}

function getJwtExpiresAt(token: string | null): string | null {
  if (!token) return null

  try {
    const payloadPart = token.split('.')[1]
    const normalizedPayload = payloadPart
      .replace(/-/g, '+')
      .replace(/_/g, '/')
      .padEnd(Math.ceil(payloadPart.length / 4) * 4, '=')
    const payload = JSON.parse(window.atob(normalizedPayload)) as {
      exp?: number
    }

    return payload.exp
      ? new Date(payload.exp * 1000).toISOString()
      : null
  } catch {
    return null
  }
}
