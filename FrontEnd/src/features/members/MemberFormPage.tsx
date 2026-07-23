import { Link, useNavigate, useParams } from 'react-router-dom'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import { MemberForm } from './MemberForm'
import { memberMockData } from './memberMockData'
import type { MemberSaveRequest } from './memberTypes'
import './members.css'

type MemberFormPageProps = {
  userRoles: readonly UserRole[]
}

const emptyMemberValues: MemberSaveRequest = {
  memberName: '',
  primaryPosition: 'Goalkeeper',
  secondaryPosition: null,
  phoneNumber: '',
  birthYear: 0,
  notes: '',
}

export function MemberFormPage({ userRoles }: MemberFormPageProps) {
  const navigate = useNavigate()
  const { memberId } = useParams()
  const canWriteMembers = hasPermission(userRoles, 'members', 'write')
  const editingMember = memberId
    ? memberMockData.find((member) => member.memberId === Number(memberId))
    : undefined
  const isEditMode = Boolean(memberId)

  if (!canWriteMembers) {
    return (
      <main className="dashboard-main">
        <section className="member-form-message">
          <h1>접근 권한이 없습니다.</h1>
          <p>회원정보를 추가하거나 수정할 수 없는 계정입니다.</p>
          <Link to="/members">회원 목록으로 돌아가기</Link>
        </section>
      </main>
    )
  }

  if (isEditMode && !editingMember) {
    return (
      <main className="dashboard-main">
        <section className="member-form-message">
          <h1>회원을 찾을 수 없습니다.</h1>
          <p>존재하지 않는 회원 번호입니다.</p>
          <Link to="/members">회원 목록으로 돌아가기</Link>
        </section>
      </main>
    )
  }

  const initialValues: MemberSaveRequest = editingMember
    ? {
        memberName: editingMember.memberName,
        primaryPosition: editingMember.primaryPosition,
        secondaryPosition: editingMember.secondaryPosition,
        phoneNumber: editingMember.phoneNumber,
        birthYear: editingMember.birthYear,
        notes: editingMember.notes,
      }
    : emptyMemberValues

  function handleSubmit(values: MemberSaveRequest) {
    console.info('백엔드 연결 전 회원 입력값:', values)
    window.alert('입력값 검사를 통과했습니다. DB 연결 후 실제로 저장됩니다.')
  }

  return (
    <main className="dashboard-main">
      <header className="member-form-header">
        <p>회원정보 / {isEditMode ? '수정' : '추가'}</p>
        <h1>{isEditMode ? '회원정보 수정' : '회원 추가'}</h1>
        <span>필수 항목을 입력한 후 저장해 주세요.</span>
      </header>

      <section className="member-form-panel">
        <MemberForm
          initialValues={initialValues}
          submitLabel={isEditMode ? '수정 내용 저장' : '회원 추가'}
          onSubmit={handleSubmit}
          onCancel={() => navigate('/members')}
        />
      </section>
    </main>
  )
}
