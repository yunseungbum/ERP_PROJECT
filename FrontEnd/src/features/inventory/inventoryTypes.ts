export type InventoryItemResponse = {
  inventoryItemId: number
  itemName: string
  quantity: number
}

export type InventoryItemSaveRequest = {
  itemName: string
  quantity: number
}
