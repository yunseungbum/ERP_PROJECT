import {
  apiDelete,
  apiGet,
  apiPost,
  apiPut,
} from '../../shared/api/apiClient'
import type {
  InventoryItemResponse,
  InventoryItemSaveRequest,
} from './inventoryTypes'

export function getInventoryItems(): Promise<InventoryItemResponse[]> {
  return apiGet<InventoryItemResponse[]>('/api/inventory')
}

export function createInventoryItem(
  request: InventoryItemSaveRequest,
): Promise<InventoryItemResponse> {
  return apiPost<InventoryItemResponse, InventoryItemSaveRequest>(
    '/api/inventory',
    request,
  )
}

export function updateInventoryItem(
  inventoryItemId: number,
  request: InventoryItemSaveRequest,
): Promise<InventoryItemResponse> {
  return apiPut<InventoryItemResponse, InventoryItemSaveRequest>(
    `/api/inventory/${inventoryItemId}`,
    request,
  )
}

export function deleteInventoryItem(
  inventoryItemId: number,
): Promise<void> {
  return apiDelete(`/api/inventory/${inventoryItemId}`)
}
