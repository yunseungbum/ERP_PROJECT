const accessTokenStorageKey = 'buddyErp.accessToken'

let accessToken: string | null =
  window.localStorage.getItem(accessTokenStorageKey)

export function getAccessToken(): string | null {
  return accessToken
}

export function setAccessToken(token: string): void {
  accessToken = token
  window.localStorage.setItem(accessTokenStorageKey, token)
}

export function clearAccessToken(): void {
  accessToken = null
  window.localStorage.removeItem(accessTokenStorageKey)
}
