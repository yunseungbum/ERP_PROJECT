import { useEffect, useState } from 'react'
import { hasPermission } from '../../shared/auth/hasPermission'
import { USER_ROLES, type UserRole } from '../../shared/auth/roles'
import { ApiError } from '../../shared/api/apiClient'
import { Link } from 'react-router-dom'
import { deleteMember, getMembers } from './memberApi'
import { memberPositionLabels } from './memberPositions'
import type { MemberResponse } from './memberTypes'
import './members.css'

type MembersPageProps = {
  userRoles: readonly UserRole[]
}

function calculateKoreanAge(birthYear: number) {
  return new Date().getFullYear() - birthYear + 1
}

export function MembersPage({ userRoles }: MembersPageProps) {
  const canWriteMembers = hasPermission(userRoles, 'members', 'write')
  const canDeleteMembers = userRoles.includes(USER_ROLES.president)
  const [members, setMembers] = useState<MemberResponse[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [expandedNoteMemberId, setExpandedNoteMemberId] =
    useState<number | null>(null)

  useEffect(() => {
    void loadMembers()
  }, [])

  async function loadMembers() {
    setIsLoading(true)
    setErrorMessage(null)

    try {
      setMembers(await getMembers())
    } catch (error) {
      setErrorMessage(getMemberErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  async function handleDelete(member: MemberResponse) {
    const confirmed = window.confirm(
      `${member.memberName} 회원을 삭제하시겠습니까?\n기존 연결 기록은 유지되고 회원만 비활성화됩니다.`,
    )

    if (!confirmed) return

    try {
      await deleteMember(member.memberId)
      setMembers((currentMembers) =>
        currentMembers.filter(
          (currentMember) => currentMember.memberId !== member.memberId,
        ),
      )
    } catch (error) {
      window.alert(getMemberErrorMessage(error))
    }
  }

  return (
    <main className="dashboard-main members-page">
      <header className="members-header">
        <div>
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>회원정보</h1>
          <span>등록 회원 {members.length}명</span>
        </div>

        {canWriteMembers && (
          <Link to="/members/new" className="member-add-button">
            + 회원 추가
          </Link>
        )}
      </header>

      <section className="members-panel" aria-label="회원 목록">
        {isLoading && <p className="member-list-message">회원정보를 불러오는 중입니다.</p>}
        {errorMessage && (
          <div className="member-list-message is-error">
            <p>{errorMessage}</p>
            <button type="button" onClick={() => void loadMembers()}>다시 시도</button>
          </div>
        )}
        {!isLoading && !errorMessage && members.length === 0 && (
          <p className="member-list-message">등록된 회원이 없습니다.</p>
        )}
        {!isLoading && !errorMessage && members.length > 0 && (
        <div className="members-table-wrap">
          <table className="members-table">
            <thead>
              <tr>
                <th>이름</th>
                <th>1순위</th>
                <th>2순위</th>
                <th>연락처</th>
                <th>나이</th>
                <th>비고</th>
                <th>상태</th>
                <th>유니폼</th>
                <th>등번호</th>
                {(canWriteMembers || canDeleteMembers) && <th>관리</th>}
              </tr>
            </thead>
            <tbody>
              {members.map((member) => (
                <tr key={member.memberId}>
                  <td className="member-name">{member.memberName}</td>
                  <td><span className="position-badge">{memberPositionLabels[member.primaryPosition]}</span></td>
                  <td>{member.secondaryPosition ? memberPositionLabels[member.secondaryPosition] : '-'}</td>
                  <td>{member.phoneNumber}</td>
                  <td>{calculateKoreanAge(member.birthYear)}살</td>
                  <td className="member-notes">
                    {member.notes ? (
                      <button
                        type="button"
                        className={
                          expandedNoteMemberId === member.memberId
                            ? 'member-notes-toggle is-expanded'
                            : 'member-notes-toggle'
                        }
                        aria-expanded={
                          expandedNoteMemberId === member.memberId
                        }
                        onClick={() =>
                          setExpandedNoteMemberId((currentMemberId) =>
                            currentMemberId === member.memberId
                              ? null
                              : member.memberId,
                          )
                        }
                      >
                        {member.notes}
                      </button>
                    ) : (
                      '-'
                    )}
                  </td>
                  <td>
                    <span className={
                      member.memberStatus === 'Active'
                        ? 'status-badge is-active'
                        : 'status-badge is-paused'
                    }>
                      {member.memberStatus === 'Active' ? '활동' : '중단'}
                    </span>
                  </td>
                  <td>{member.hasUniform ? 'O' : 'X'}</td>
                  <td>
                    {member.hasUniform
                      ? member.uniformNumber ?? '-'
                      : '-'}
                  </td>
                  {(canWriteMembers || canDeleteMembers) && (
                    <td>
                      <div className="member-actions">
                        {canWriteMembers && (
                          <Link to={`/members/${member.memberId}/edit`} className="edit-button">수정</Link>
                        )}
                        {canDeleteMembers && (
                          <button type="button" className="delete-button" onClick={() => void handleDelete(member)}>삭제</button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        )}
      </section>
    </main>
  )
}

function getMemberErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401) return '로그인이 만료되었습니다. 다시 로그인해 주세요.'
    if (error.status === 403) return '회원정보를 처리할 권한이 없습니다.'
    if (error.status === 404) return '회원을 찾을 수 없습니다.'
    return error.message
  }

  return '회원정보 서버에 연결할 수 없습니다.'
}
