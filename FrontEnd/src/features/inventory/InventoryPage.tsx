import { useEffect, useState, type FormEvent } from 'react'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import {
  createInventoryItem,
  deleteInventoryItem,
  getInventoryItems,
  updateInventoryItem,
} from './inventoryApi'
import type { InventoryItemResponse } from './inventoryTypes'
import './inventory.css'

type InventoryPageProps = {
  userRoles: readonly UserRole[]
}

export function InventoryPage({ userRoles }: InventoryPageProps) {
  const canWriteInventory = hasPermission(
    userRoles,
    'inventory',
    'write',
  )
  const [items, setItems] = useState<InventoryItemResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [editingItem, setEditingItem] =
    useState<InventoryItemResponse | null>(null)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [itemName, setItemName] = useState('')
  const [quantity, setQuantity] = useState('')
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    async function loadItems() {
      try {
        setItems(await getInventoryItems())
      } catch {
        setErrorMessage('보유 물품을 불러오지 못했습니다.')
      } finally {
        setIsLoading(false)
      }
    }

    void loadItems()
  }, [])

  function openCreateModal() {
    setEditingItem(null)
    setItemName('')
    setQuantity('')
    setIsModalOpen(true)
  }

  function openEditModal(item: InventoryItemResponse) {
    setEditingItem(item)
    setItemName(item.itemName)
    setQuantity(String(item.quantity))
    setIsModalOpen(true)
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!itemName.trim() || Number(quantity) < 0) {
      window.alert('품명과 수량을 올바르게 입력해 주세요.')
      return
    }

    setIsSaving(true)
    try {
      const savedItem = editingItem
        ? await updateInventoryItem(editingItem.inventoryItemId, {
            itemName: itemName.trim(),
            quantity: Number(quantity),
          })
        : await createInventoryItem({
            itemName: itemName.trim(),
            quantity: Number(quantity),
          })

      setItems((currentItems) => editingItem
        ? currentItems.map((item) =>
            item.inventoryItemId === savedItem.inventoryItemId
              ? savedItem
              : item)
        : [...currentItems, savedItem])
      setIsModalOpen(false)
    } catch (error) {
      window.alert(
        error instanceof Error
          ? error.message
          : '물품을 저장하지 못했습니다.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete(item: InventoryItemResponse) {
    if (!window.confirm(`${item.itemName} 물품을 삭제할까요?`)) return

    try {
      await deleteInventoryItem(item.inventoryItemId)
      setItems((currentItems) =>
        currentItems.filter(
          (currentItem) =>
            currentItem.inventoryItemId !== item.inventoryItemId,
        ),
      )
    } catch {
      window.alert('물품을 삭제하지 못했습니다.')
    }
  }

  return (
    <main className="dashboard-main inventory-page">
      <header className="inventory-header">
        <div>
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>보유 물품 리스트</h1>
          <span>보유 물품 {items.length}종</span>
        </div>
        {canWriteInventory && (
          <button type="button" onClick={openCreateModal}>
            + 물품 추가
          </button>
        )}
      </header>

      <section className="inventory-panel">
        {isLoading && <p className="inventory-page-message">보유 물품을 불러오는 중입니다.</p>}
        {errorMessage && <p className="inventory-page-message is-error">{errorMessage}</p>}
        {!isLoading && !errorMessage && (
          <div className="inventory-table-wrap">
            <table className="inventory-table">
              <thead>
                <tr>
                  <th>No.</th>
                  <th>품명</th>
                  <th>수량</th>
                  {canWriteInventory && <th>관리</th>}
                </tr>
              </thead>
              <tbody>
                {items.length === 0 ? (
                  <tr>
                    <td colSpan={canWriteInventory ? 4 : 3}>
                      등록된 보유 물품이 없습니다.
                    </td>
                  </tr>
                ) : (
                  items.map((item, index) => (
                    <tr key={item.inventoryItemId}>
                      <td>{index + 1}</td>
                      <td className="inventory-item-name">{item.itemName}</td>
                      <td>{item.quantity}</td>
                      {canWriteInventory && (
                        <td>
                          <div className="inventory-actions">
                            <button type="button" onClick={() => openEditModal(item)}>수정</button>
                            <button className="is-delete" type="button" onClick={() => void handleDelete(item)}>삭제</button>
                          </div>
                        </td>
                      )}
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        )}
      </section>

      {isModalOpen && (
        <div className="inventory-modal-backdrop">
          <div className="inventory-modal" role="dialog" aria-modal="true">
            <h2>물품 {editingItem ? '수정' : '추가'}</h2>
            <form onSubmit={(event) => void handleSubmit(event)}>
              <label>
                품명
                <input value={itemName} onChange={(event) => setItemName(event.target.value)} maxLength={100} required />
              </label>
              <label>
                수량
                <input type="number" min="0" value={quantity} onChange={(event) => setQuantity(event.target.value)} required />
              </label>
              <div className="inventory-modal-actions">
                <button type="button" onClick={() => setIsModalOpen(false)}>취소</button>
                <button className="is-primary" type="submit" disabled={isSaving}>
                  {isSaving ? '저장 중...' : '저장'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </main>
  )
}
