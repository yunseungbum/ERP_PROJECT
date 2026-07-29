export const sessionExpiredEventName = 'buddyErp:session-expired'

export function notifySessionExpired(): void {
  window.dispatchEvent(new Event(sessionExpiredEventName))
}
