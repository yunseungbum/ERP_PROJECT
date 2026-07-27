import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ApiError } from '../../shared/api/apiClient'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import { MemberForm } from './MemberForm'
import {
  createMember,
  getMember,
  updateMember,
} from './memberApi'
import type { MemberResponse, MemberSaveRequest } from './memberTypes'
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
  memberStatus: 'Active',
  hasUniform: false,
  uniformNumber: null,
}

export function MemberFormPage({ userRoles }: MemberFormPageProps) {
  const navigate = useNavigate()
  const { memberId } = useParams()
  const canWriteMembers = hasPermission(userRoles, 'members', 'write')
  const isEditMode = Boolean(memberId)
  const parsedMemberId = Number(memberId)
  const [editingMember, setEditingMember] =
    useState<MemberResponse | null>(null)
  const [isLoading, setIsLoading] = useState(isEditMode)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    if (!isEditMode || !canWriteMembers) return

    if (!Number.isSafeInteger(parsedMemberId) || parsedMemberId <= 0) {
      setErrorMessage('올바르지 않은 회원 번호입니다.')
      setIsLoading(false)
      return
    }

    async function loadMember() {
      try {
        setEditingMember(await getMember(parsedMemberId))
      } catch (error) {
        setErrorMessage(getMemberFormErrorMessage(error))
      } finally {
        setIsLoading(false)
      }
    }

    void loadMember()
  }, [canWriteMembers, isEditMode, parsedMemberId])

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

  if (isLoading) {
    return (
      <main className="dashboard-main">
        <section className="member-form-message">
          <h1>회원정보를 불러오는 중입니다.</h1>
        </section>
      </main>
    )
  }

  if (isEditMode && (!editingMember || errorMessage)) {
    return (
      <main className="dashboard-main">
        <section className="member-form-message">
          <h1>회원을 찾을 수 없습니다.</h1>
          <p>{errorMessage ?? '존재하지 않는 회원 번호입니다.'}</p>
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
        memberStatus: editingMember.memberStatus,
        hasUniform: editingMember.hasUniform,
        uniformNumber: editingMember.uniformNumber,
      }
    : emptyMemberValues

  async function handleSubmit(values: MemberSaveRequest) {
    setIsSubmitting(true)
    setErrorMessage(null)

    try {
      if (isEditMode) {
        await updateMember(parsedMemberId, values)
      } else {
        await createMember(values)
      }

      navigate('/members')
    } catch (error) {
      const message = getMemberFormErrorMessage(error)
      setErrorMessage(message)
      window.alert(message)
    } finally {
      setIsSubmitting(false)
    }
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
          isSubmitting={isSubmitting}
          onSubmit={handleSubmit}
          onCancel={() => navigate('/members')}
        />
      </section>
    </main>
  )
}

function getMemberFormErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401) return '로그인이 만료되었습니다. 다시 로그인해 주세요.'
    if (error.status === 403) return '회원정보를 저장할 권한이 없습니다.'
    if (error.status === 404) return '수정할 회원을 찾을 수 없습니다.'
    return error.message
  }

  return '회원정보 서버에 연결할 수 없습니다.'
}
