import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import { Link } from 'react-router-dom'
import { memberMockData } from './memberMockData'
import { memberPositionLabels } from './memberPositions'
import './members.css'

type MembersPageProps = {
  userRoles: readonly UserRole[]
}

function calculateKoreanAge(birthYear: number) {
  return new Date().getFullYear() - birthYear + 1
}

export function MembersPage({ userRoles }: MembersPageProps) {
  const canWriteMembers = hasPermission(userRoles, 'members', 'write')

  return (
    <main className="dashboard-main members-page">
      <header className="members-header">
        <div>
          <p>Buddy FC 통합 관리 시스템</p>
          <h1>회원정보</h1>
          <span>등록 회원 {memberMockData.length}명</span>
        </div>

        {canWriteMembers && (
          <Link to="/members/new" className="member-add-button">
            + 회원 추가
          </Link>
        )}
      </header>

      <section className="members-panel" aria-label="회원 목록">
        <div className="members-table-wrap">
          <table className="members-table">
            <thead>
              <tr>
                <th>이름</th>
                <th>1순위</th>
                <th>2순위</th>
                <th>연락처</th>
                <th>한국식 나이</th>
                <th>비고</th>
                <th>상태</th>
                {canWriteMembers && <th>관리</th>}
              </tr>
            </thead>
            <tbody>
              {memberMockData.map((member) => (
                <tr key={member.memberId}>
                  <td className="member-name">{member.memberName}</td>
                  <td><span className="position-badge">{memberPositionLabels[member.primaryPosition]}</span></td>
                  <td>{member.secondaryPosition ? memberPositionLabels[member.secondaryPosition] : '-'}</td>
                  <td>{member.phoneNumber}</td>
                  <td>{calculateKoreanAge(member.birthYear)}살</td>
                  <td className="member-notes">{member.notes || '-'}</td>
                  <td><span className={member.isActive ? 'status-badge is-active' : 'status-badge'}>{member.isActive ? '활동' : '비활동'}</span></td>
                  {canWriteMembers && (
                    <td>
                      <div className="member-actions">
                        <Link to={`/members/${member.memberId}/edit`} className="edit-button">수정</Link>
                        <button type="button" className="delete-button">삭제</button>
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </main>
  )
}
