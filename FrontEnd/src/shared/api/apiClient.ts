import { getAccessToken } from '../auth/accessTokenStore'

const apiBaseUrl =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5080'

type ApiProblem = {
  detail?: string
  title?: string
}

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

export async function apiGet<T>(path: string): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: createHeaders(),
  })

  await ensureSuccess(response)

  return response.json() as Promise<T>
}

export async function apiPost<TResponse, TRequest>(
  path: string,
  body: TRequest,
): Promise<TResponse> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: 'POST',
    headers: createHeaders(true),
    body: JSON.stringify(body),
  })

  await ensureSuccess(response)

  return response.json() as Promise<TResponse>
}

export async function apiPut<TResponse, TRequest>(
  path: string,
  body: TRequest,
): Promise<TResponse> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: 'PUT',
    headers: createHeaders(true),
    body: JSON.stringify(body),
  })

  await ensureSuccess(response)

  return response.json() as Promise<TResponse>
}

export async function apiDelete(path: string): Promise<void> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: 'DELETE',
    headers: createHeaders(),
  })

  await ensureSuccess(response)
}

export async function apiDeleteWithResponse<T>(path: string): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: 'DELETE',
    headers: createHeaders(),
  })

  await ensureSuccess(response)

  return response.json() as Promise<T>
}

export function getApiUrl(path: string): string {
  return `${apiBaseUrl}${path}`
}

function createHeaders(hasJsonBody = false): HeadersInit {
  const headers: Record<string, string> = {}
  const accessToken = getAccessToken()

  if (hasJsonBody) {
    headers['Content-Type'] = 'application/json'
  }

  if (accessToken) {
    headers.Authorization = `Bearer ${accessToken}`
  }

  return headers
}

async function ensureSuccess(response: Response): Promise<void> {
  if (response.ok) {
    return
  }

  const problem = await readProblem(response)
  const message =
    problem.detail ?? problem.title ?? `API 요청 실패 (${response.status})`

  throw new ApiError(response.status, message)
}

async function readProblem(response: Response): Promise<ApiProblem> {
  try {
    return (await response.json()) as ApiProblem
  } catch {
    return {}
  }
}
