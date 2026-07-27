import { useState, type FormEvent } from 'react'
import { memberPositionOptions } from './memberPositions'
import type { MemberPosition, MemberSaveRequest } from './memberTypes'

type MemberFormProps = {
  initialValues: MemberSaveRequest
  submitLabel: string
  isSubmitting: boolean
  onSubmit: (values: MemberSaveRequest) => Promise<void>
  onCancel: () => void
}

type MemberFormErrors = Partial<Record<keyof MemberSaveRequest, string>>

export function MemberForm({
  initialValues,
  submitLabel,
  isSubmitting,
  onSubmit,
  onCancel,
}: MemberFormProps) {
  const [values, setValues] = useState(initialValues)
  const [errors, setErrors] = useState<MemberFormErrors>({})

  function updateValue<Key extends keyof MemberSaveRequest>(
    key: Key,
    value: MemberSaveRequest[Key],
  ) {
    setValues((currentValues) => ({ ...currentValues, [key]: value }))
    setErrors((currentErrors) => ({ ...currentErrors, [key]: undefined }))
  }

  function validateForm() {
    const nextErrors: MemberFormErrors = {}

    if (!values.memberName.trim()) nextErrors.memberName = '이름을 입력해 주세요.'
    if (!values.phoneNumber.trim()) nextErrors.phoneNumber = '연락처를 입력해 주세요.'
    const currentYear = new Date().getFullYear()
    if (values.birthYear < 1900 || values.birthYear > currentYear) {
      nextErrors.birthYear = `출생연도는 1900년부터 ${currentYear}년 사이로 입력해 주세요.`
    }
    if (values.primaryPosition === values.secondaryPosition) {
      nextErrors.secondaryPosition = '2순위는 1순위와 다르게 선택해 주세요.'
    }
    if (values.notes.length > 1000) nextErrors.notes = '비고는 1,000자 이하로 입력해 주세요.'

    if (values.hasUniform &&
        (values.uniformNumber === null ||
          values.uniformNumber < 1 ||
          values.uniformNumber > 99)) {
      nextErrors.uniformNumber = '등번호는 1부터 99까지 입력해 주세요.'
    }

    setErrors(nextErrors)
    return Object.keys(nextErrors).length === 0
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!validateForm()) return

    await onSubmit({
      ...values,
      memberName: values.memberName.trim(),
      phoneNumber: values.phoneNumber.trim(),
      notes: values.notes.trim(),
    })
  }

  return (
    <form className="member-form" onSubmit={handleSubmit} noValidate>
      <div className="member-form-grid">
        <label className="member-form-field">
          <span>이름 <strong>*</strong></span>
          <input value={values.memberName} onChange={(event) => updateValue('memberName', event.target.value)} aria-invalid={Boolean(errors.memberName)} />
          {errors.memberName && <small>{errors.memberName}</small>}
        </label>

        <label className="member-form-field">
          <span>연락처 <strong>*</strong></span>
          <input type="tel" placeholder="010-0000-0000" value={values.phoneNumber} onChange={(event) => updateValue('phoneNumber', event.target.value)} aria-invalid={Boolean(errors.phoneNumber)} />
          {errors.phoneNumber && <small>{errors.phoneNumber}</small>}
        </label>

        <label className="member-form-field">
          <span>1순위 포지션 <strong>*</strong></span>
          <select value={values.primaryPosition} onChange={(event) => updateValue('primaryPosition', event.target.value as MemberPosition)}>
            {memberPositionOptions.map((position) => <option key={position.value} value={position.value}>{position.label}</option>)}
          </select>
        </label>

        <label className="member-form-field">
          <span>2순위 포지션</span>
          <select value={values.secondaryPosition ?? ''} onChange={(event) => updateValue('secondaryPosition', event.target.value ? event.target.value as MemberPosition : null)} aria-invalid={Boolean(errors.secondaryPosition)}>
            <option value="">선택 안 함</option>
            {memberPositionOptions.map((position) => <option key={position.value} value={position.value}>{position.label}</option>)}
          </select>
          {errors.secondaryPosition && <small>{errors.secondaryPosition}</small>}
        </label>

        <label className="member-form-field">
          <span>출생연도 <strong>*</strong></span>
          <input
            type="number"
            min="1900"
            max={new Date().getFullYear()}
            placeholder="예: 1998"
            value={values.birthYear || ''}
            onChange={(event) => updateValue('birthYear', Number(event.target.value))}
            aria-invalid={Boolean(errors.birthYear)}
          />
          {errors.birthYear && <small>{errors.birthYear}</small>}
        </label>

        <label className="member-form-field">
          <span>활동 상태 <strong>*</strong></span>
          <select
            value={values.memberStatus}
            onChange={(event) =>
              updateValue(
                'memberStatus',
                event.target.value as MemberSaveRequest['memberStatus'],
              )
            }
          >
            <option value="Active">활동</option>
            <option value="Paused">중단</option>
          </select>
          <small>중단 상태에서는 새로운 월 회비를 청구하지 않습니다.</small>
        </label>

        <label className="member-form-field">
          <span>유니폼 여부 <strong>*</strong></span>
          <select
            value={values.hasUniform ? 'true' : 'false'}
            onChange={(event) => {
              const hasUniform = event.target.value === 'true'
              setValues((currentValues) => ({
                ...currentValues,
                hasUniform,
                uniformNumber: hasUniform
                  ? currentValues.uniformNumber
                  : null,
              }))
            }}
          >
            <option value="false">없음</option>
            <option value="true">있음</option>
          </select>
        </label>

        <label className="member-form-field">
          <span>등번호</span>
          <input
            type="number"
            min="1"
            max="99"
            value={values.uniformNumber ?? ''}
            disabled={!values.hasUniform}
            onChange={(event) =>
              updateValue(
                'uniformNumber',
                event.target.value ? Number(event.target.value) : null,
              )
            }
            aria-invalid={Boolean(errors.uniformNumber)}
          />
          {errors.uniformNumber && <small>{errors.uniformNumber}</small>}
        </label>

        <label className="member-form-field member-notes-field">
          <span>비고</span>
          <textarea rows={5} maxLength={1000} placeholder="회원 관련 메모를 입력해 주세요." value={values.notes} onChange={(event) => updateValue('notes', event.target.value)} />
          <em>{values.notes.length} / 1000</em>
          {errors.notes && <small>{errors.notes}</small>}
        </label>
      </div>

      <div className="member-form-actions">
        <button type="button" className="form-cancel-button" onClick={onCancel} disabled={isSubmitting}>취소</button>
        <button type="submit" className="form-submit-button" disabled={isSubmitting}>
          {isSubmitting ? '저장 중...' : submitLabel}
        </button>
      </div>
    </form>
  )
}
