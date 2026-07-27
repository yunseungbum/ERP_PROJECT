import { useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { ApiError } from '../../shared/api/apiClient'
import {
  USER_ROLES,
  type UserRole,
} from '../../shared/auth/roles'
import {
  createAnnouncement,
  deleteAnnouncement,
  getAnnouncements,
  updateAnnouncement,
} from './announcementApi'
import type { AnnouncementResponse } from './announcementTypes'
import { dashboardSummaries } from './dashboardData'
import './dashboard.css'

type DashboardPageProps = {
  userRoles: readonly UserRole[]
}

type AnnouncementFormValues = {
  title: string
  content: string
}

const emptyAnnouncementForm: AnnouncementFormValues = {
  title: '',
  content: '',
}

export function DashboardPage({ userRoles }: DashboardPageProps) {
  const isPresident = userRoles.includes(USER_ROLES.president)
  const [announcements, setAnnouncements] =
    useState<AnnouncementResponse[]>([])
  const [announcementError, setAnnouncementError] = useState('')
  const [isAnnouncementLoading, setIsAnnouncementLoading] = useState(true)
  const [editingAnnouncement, setEditingAnnouncement] =
    useState<AnnouncementResponse | null>(null)
  const [announcementForm, setAnnouncementForm] =
    useState<AnnouncementFormValues>(emptyAnnouncementForm)
  const [isAnnouncementModalOpen, setIsAnnouncementModalOpen] =
    useState(false)
  const [isAnnouncementSaving, setIsAnnouncementSaving] = useState(false)
  const [viewingAnnouncement, setViewingAnnouncement] =
    useState<AnnouncementResponse | null>(null)

  useEffect(() => {
    void loadAnnouncements()
  }, [])

  async function loadAnnouncements() {
    setIsAnnouncementLoading(true)
    setAnnouncementError('')

    try {
      setAnnouncements(await getAnnouncements())
    } catch (error) {
      setAnnouncementError(getAnnouncementErrorMessage(error))
    } finally {
      setIsAnnouncementLoading(false)
    }
  }

  function openCreateAnnouncementModal() {
    setEditingAnnouncement(null)
    setAnnouncementForm(emptyAnnouncementForm)
    setIsAnnouncementModalOpen(true)
  }

  function openEditAnnouncementModal(
    announcement: AnnouncementResponse,
  ) {
    setEditingAnnouncement(announcement)
    setAnnouncementForm({
      title: announcement.title,
      content: announcement.content,
    })
    setIsAnnouncementModalOpen(true)
  }

  async function handleAnnouncementSubmit(
    event: FormEvent<HTMLFormElement>,
  ) {
    event.preventDefault()

    if (!announcementForm.title.trim() ||
        !announcementForm.content.trim()) {
      window.alert('공지 제목과 내용을 입력해 주세요.')
      return
    }

    setIsAnnouncementSaving(true)

    try {
      const request = {
        title: announcementForm.title.trim(),
        content: announcementForm.content.trim(),
      }

      if (editingAnnouncement) {
        await updateAnnouncement(
          editingAnnouncement.announcementId,
          request,
        )
      } else {
        await createAnnouncement(request)
      }

      setIsAnnouncementModalOpen(false)
      setAnnouncements(await getAnnouncements())
    } catch (error) {
      window.alert(getAnnouncementErrorMessage(error))
    } finally {
      setIsAnnouncementSaving(false)
    }
  }

  async function handleAnnouncementDelete(
    announcement: AnnouncementResponse,
  ) {
    if (!window.confirm(`"${announcement.title}" 공지를 삭제할까요?`)) {
      return
    }

    try {
      await deleteAnnouncement(announcement.announcementId)
      setAnnouncements(await getAnnouncements())
    } catch (error) {
      window.alert(getAnnouncementErrorMessage(error))
    }
  }

  return (
    <main className="dashboard-main dashboard-home">
      <header className="dashboard-header">
        <p>Buddy FC 통합 관리 시스템</p>
        <h1>팀 운영 대시보드</h1>
      </header>

      <section className="notice-panel" aria-labelledby="notice-title">
        <div className="section-heading">
          <div>
            <span className="section-icon" aria-hidden="true">!</span>
            <h2 id="notice-title">공지사항</h2>
          </div>
          {isPresident && (
            <button
              type="button"
              className="notice-add-button"
              onClick={openCreateAnnouncementModal}
            >
              + 공지 추가
            </button>
          )}
        </div>

        <div className="notice-list">
          {isAnnouncementLoading && (
            <p className="notice-message">공지사항을 불러오는 중입니다.</p>
          )}
          {announcementError && (
            <div className="notice-message is-error">
              <span>{announcementError}</span>
              <button
                type="button"
                onClick={() => void loadAnnouncements()}
              >
                다시 시도
              </button>
            </div>
          )}
          {!isAnnouncementLoading &&
            !announcementError &&
            announcements.length === 0 && (
            <p className="notice-message">등록된 공지사항이 없습니다.</p>
          )}
          {!isAnnouncementLoading &&
            !announcementError &&
            announcements.length > 0 && (
            <div className="notice-simple-list">
              {announcements.map((announcement) => (
                <article
                  className="notice-simple-item"
                  key={announcement.announcementId}
                >
                  <span className="notice-dot" aria-hidden="true" />
                  <button
                    type="button"
                    className="notice-title-button"
                    onClick={() =>
                      setViewingAnnouncement(announcement)
                    }
                  >
                    {announcement.title}
                  </button>
                  {isPresident && (
                    <div className="notice-actions">
                      <button
                        type="button"
                        onClick={() =>
                          openEditAnnouncementModal(announcement)
                        }
                      >
                        수정
                      </button>
                      <button
                        type="button"
                        className="is-delete"
                        onClick={() =>
                          void handleAnnouncementDelete(announcement)
                        }
                      >
                        삭제
                      </button>
                    </div>
                  )}
                </article>
              ))}
            </div>
          )}
        </div>
      </section>

      <section className="summary-grid" aria-label="ERP 주요 현황">
        {dashboardSummaries.map((summary) => (
          <Link
            className="summary-card"
            key={summary.title}
            to={summary.path}
          >
            <span className="summary-icon" aria-hidden="true">
              {summary.icon}
            </span>
            <strong>{summary.title}</strong>
          </Link>
        ))}
      </section>

      {isAnnouncementModalOpen && (
        <div className="announcement-modal-backdrop" role="presentation">
          <div
            className="announcement-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="announcement-modal-title"
          >
            <header>
              <h2 id="announcement-modal-title">
                공지 {editingAnnouncement ? '수정' : '추가'}
              </h2>
              <button
                type="button"
                aria-label="닫기"
                onClick={() => setIsAnnouncementModalOpen(false)}
              >
                ×
              </button>
            </header>

            <form
              onSubmit={(event) =>
                void handleAnnouncementSubmit(event)
              }
            >
              <label>
                <span>제목 *</span>
                <input
                  value={announcementForm.title}
                  maxLength={100}
                  onChange={(event) =>
                    setAnnouncementForm({
                      ...announcementForm,
                      title: event.target.value,
                    })
                  }
                  required
                />
              </label>
              <label>
                <span>내용 *</span>
                <textarea
                  value={announcementForm.content}
                  maxLength={1000}
                  rows={5}
                  onChange={(event) =>
                    setAnnouncementForm({
                      ...announcementForm,
                      content: event.target.value,
                    })
                  }
                  required
                />
              </label>
              <div className="announcement-modal-actions">
                <button
                  type="button"
                  onClick={() => setIsAnnouncementModalOpen(false)}
                >
                  취소
                </button>
                <button
                  type="submit"
                  className="is-primary"
                  disabled={isAnnouncementSaving}
                >
                  {isAnnouncementSaving ? '저장 중...' : '저장'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {viewingAnnouncement && (
        <div className="announcement-modal-backdrop" role="presentation">
          <div
            className="announcement-modal announcement-view-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="announcement-view-title"
          >
            <header>
              <h2 id="announcement-view-title">
                {viewingAnnouncement.title}
              </h2>
              <button
                type="button"
                aria-label="닫기"
                onClick={() => setViewingAnnouncement(null)}
              >
                ×
              </button>
            </header>
            <div className="announcement-view-content">
              <p>{viewingAnnouncement.content}</p>
            </div>
          </div>
        </div>
      )}
    </main>
  )
}

function getAnnouncementErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401) {
      return '로그인이 만료되었습니다. 다시 로그인해 주세요.'
    }
    if (error.status === 403) {
      return '공지사항을 변경할 권한이 없습니다.'
    }
    return error.message
  }

  return '공지사항 서버에 연결할 수 없습니다.'
}
