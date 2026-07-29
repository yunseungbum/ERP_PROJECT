import { useEffect, useMemo, useState } from 'react'
import {
  addGuestParticipant,
  addMemberParticipants,
  getUpcomingFormationBoard,
  removeParticipant,
  saveQuarterFormation,
} from './formationApi'
import type {
  FormationBoardResponse,
  FormationCode,
} from './formationTypes'
import { getMembers } from '../members/memberApi'
import type { MemberResponse } from '../members/memberTypes'
import { hasPermission } from '../../shared/auth/hasPermission'
import type { UserRole } from '../../shared/auth/roles'
import './formation.css'

type Quarter = 1 | 2 | 3 | 4

type Participant = {
  participantId: number
  memberId: number | null
  participantName: string
  isGuest: boolean
  quarterParticipation: boolean[]
}

type FormationSlot = {
  slotId: string
  positionCode: string
  left: number
  top: number
}

type QuarterPlan = {
  formationCode: FormationCode
  lineup: Record<string, number>
}

type QuarterPlans = Record<Quarter, QuarterPlan>

const formationTemplates: Record<FormationCode, FormationSlot[]> = {
  '4-2-3-1': [
    { slotId: 'goalkeeper', positionCode: 'GK', left: 50, top: 89 },
    { slotId: 'leftBack', positionCode: 'LB', left: 16, top: 72 },
    { slotId: 'leftCenterBack', positionCode: 'CB', left: 38, top: 76 },
    { slotId: 'rightCenterBack', positionCode: 'CB', left: 62, top: 76 },
    { slotId: 'rightBack', positionCode: 'RB', left: 84, top: 72 },
    { slotId: 'leftDefensiveMidfielder', positionCode: 'DM', left: 37, top: 55 },
    { slotId: 'rightDefensiveMidfielder', positionCode: 'DM', left: 63, top: 55 },
    { slotId: 'leftWinger', positionCode: 'LW', left: 18, top: 37 },
    { slotId: 'attackingMidfielder', positionCode: 'AM', left: 50, top: 39 },
    { slotId: 'rightWinger', positionCode: 'RW', left: 82, top: 37 },
    { slotId: 'striker', positionCode: 'ST', left: 50, top: 20 },
  ],
  '4-3-3': [
    { slotId: 'goalkeeper', positionCode: 'GK', left: 50, top: 89 },
    { slotId: 'leftBack', positionCode: 'LB', left: 16, top: 72 },
    { slotId: 'leftCenterBack', positionCode: 'CB', left: 38, top: 76 },
    { slotId: 'rightCenterBack', positionCode: 'CB', left: 62, top: 76 },
    { slotId: 'rightBack', positionCode: 'RB', left: 84, top: 72 },
    { slotId: 'leftMidfielder', positionCode: 'CM', left: 28, top: 52 },
    { slotId: 'centerMidfielder', positionCode: 'CM', left: 50, top: 57 },
    { slotId: 'rightMidfielder', positionCode: 'CM', left: 72, top: 52 },
    { slotId: 'leftForward', positionCode: 'LW', left: 22, top: 27 },
    { slotId: 'centerForward', positionCode: 'ST', left: 50, top: 20 },
    { slotId: 'rightForward', positionCode: 'RW', left: 78, top: 27 },
  ],
  '4-1-2-3': [
    { slotId: 'goalkeeper', positionCode: 'GK', left: 50, top: 89 },
    { slotId: 'leftBack', positionCode: 'LB', left: 16, top: 72 },
    { slotId: 'leftCenterBack', positionCode: 'CB', left: 38, top: 76 },
    { slotId: 'rightCenterBack', positionCode: 'CB', left: 62, top: 76 },
    { slotId: 'rightBack', positionCode: 'RB', left: 84, top: 72 },
    { slotId: 'defensiveMidfielder', positionCode: 'DM', left: 50, top: 58 },
    { slotId: 'leftCentralMidfielder', positionCode: 'CM', left: 36, top: 44 },
    { slotId: 'rightCentralMidfielder', positionCode: 'CM', left: 64, top: 44 },
    { slotId: 'leftForward', positionCode: 'LW', left: 22, top: 25 },
    { slotId: 'centerForward', positionCode: 'ST', left: 50, top: 19 },
    { slotId: 'rightForward', positionCode: 'RW', left: 78, top: 25 },
  ],
  '4-5-1': [
    { slotId: 'goalkeeper', positionCode: 'GK', left: 50, top: 89 },
    { slotId: 'leftBack', positionCode: 'LB', left: 16, top: 72 },
    { slotId: 'leftCenterBack', positionCode: 'CB', left: 38, top: 76 },
    { slotId: 'rightCenterBack', positionCode: 'CB', left: 62, top: 76 },
    { slotId: 'rightBack', positionCode: 'RB', left: 84, top: 72 },
    { slotId: 'leftMidfielder', positionCode: 'LM', left: 16, top: 48 },
    { slotId: 'leftCentralMidfielder', positionCode: 'CM', left: 35, top: 54 },
    { slotId: 'centerMidfielder', positionCode: 'CM', left: 50, top: 48 },
    { slotId: 'rightCentralMidfielder', positionCode: 'CM', left: 65, top: 54 },
    { slotId: 'rightMidfielder', positionCode: 'RM', left: 84, top: 48 },
    { slotId: 'striker', positionCode: 'ST', left: 50, top: 21 },
  ],
}

function createEmptyQuarterPlans(): QuarterPlans {
  return {
    1: { formationCode: '4-2-3-1', lineup: {} },
    2: { formationCode: '4-2-3-1', lineup: {} },
    3: { formationCode: '4-2-3-1', lineup: {} },
    4: { formationCode: '4-2-3-1', lineup: {} },
  }
}

function copyQuarterPlan(plan: QuarterPlan): QuarterPlan {
  return {
    formationCode: plan.formationCode,
    lineup: { ...plan.lineup },
  }
}

function createQuarterPlansFromBoard(
  board: FormationBoardResponse,
): QuarterPlans {
  const plans = createEmptyQuarterPlans()

  for (const quarter of board.quarters) {
    if (quarter.quarterNumber < 1 || quarter.quarterNumber > 4) {
      continue
    }

    plans[quarter.quarterNumber as Quarter] = {
      formationCode: quarter.formationCode,
      lineup: Object.fromEntries(
        quarter.players.map((player) => [
          player.slotCode,
          player.participantId,
        ]),
      ),
    }
  }

  return plans
}

type FormationPageProps = {
  userRoles: readonly UserRole[]
}

export function FormationPage({ userRoles }: FormationPageProps) {
  const canManageFormations = hasPermission(
    userRoles,
    'formations',
    'write',
  )
  const [scheduleId, setScheduleId] = useState<number | null>(null)
  const [matchTitle, setMatchTitle] = useState('')
  const [matchStartsAt, setMatchStartsAt] = useState('')
  const [members, setMembers] = useState<MemberResponse[]>([])
  const [isLoadingMembers, setIsLoadingMembers] = useState(true)
  const [memberLoadError, setMemberLoadError] = useState('')
  const [selectedMemberIds, setSelectedMemberIds] = useState<number[]>([])
  const [guestName, setGuestName] = useState('')
  const [isMemberSelectorOpen, setIsMemberSelectorOpen] = useState(true)
  const [participants, setParticipants] = useState<Participant[]>([])
  const [activeQuarter, setActiveQuarter] = useState<Quarter>(1)
  const [selectedParticipantId, setSelectedParticipantId] = useState<number | null>(null)
  const [draggedParticipantId, setDraggedParticipantId] = useState<number | null>(null)
  const [savedQuarterPlans, setSavedQuarterPlans] = useState<QuarterPlans>(createEmptyQuarterPlans)
  const [draftPlan, setDraftPlan] = useState<QuarterPlan>(() => (
    copyQuarterPlan(createEmptyQuarterPlans()[1])
  ))
  const [pendingQuarter, setPendingQuarter] = useState<Quarter | null>(null)
  const [isUnsavedModalOpen, setIsUnsavedModalOpen] = useState(false)
  const [saveMessage, setSaveMessage] = useState('')
  const [apiError, setApiError] = useState('')
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    let isMounted = true

    async function loadPage() {
      try {
        const [memberResponse, boardResponse] = await Promise.all([
          getMembers(),
          getUpcomingFormationBoard(),
        ])

        if (isMounted) {
          setMembers(memberResponse.filter((member) => member.isActive))
          applyBoardResponse(boardResponse, 1)
        }
      } catch (error) {
        if (isMounted) {
          setMemberLoadError(
            error instanceof Error
              ? error.message
              : '회원 목록을 불러오지 못했습니다.',
          )
        }
      } finally {
        if (isMounted) {
          setIsLoadingMembers(false)
        }
      }
    }

    void loadPage()

    return () => {
      isMounted = false
    }
  }, [canManageFormations])

  useEffect(() => {
    const now = new Date()
    const nextMidnight = new Date(
      now.getFullYear(),
      now.getMonth(),
      now.getDate() + 1,
      0,
      0,
      0,
      500,
    )
    const millisecondsUntilNextDay =
      nextMidnight.getTime() - now.getTime()
    const midnightTimer = window.setTimeout(() => {
      window.location.reload()
    }, millisecondsUntilNextDay)

    return () => window.clearTimeout(midnightTimer)
  }, [])

  const participantMap = useMemo(
    () => new Map(
      participants.map((participant) => [
        participant.participantId,
        participant,
      ]),
    ),
    [participants],
  )

  const memberParticipants = participants.filter(
    (participant) => !participant.isGuest,
  )
  const guestParticipants = participants.filter(
    (participant) => participant.isGuest,
  )

  const memberMap = useMemo(
    () => new Map(
      members.map((member) => [member.memberId, member]),
    ),
    [members],
  )

  function getUniformLabel(participant: Participant) {
    if (participant.isGuest) return '용병'
    if (participant.memberId === null) return '-'

    const member = memberMap.get(participant.memberId)
    return member?.hasUniform && member.uniformNumber !== null
      ? String(member.uniformNumber)
      : '-'
  }

  const isDraftChanged = useMemo(
    () => JSON.stringify(draftPlan) !== JSON.stringify(savedQuarterPlans[activeQuarter]),
    [activeQuarter, draftPlan, savedQuarterPlans],
  )

  const formationSlots = formationTemplates[draftPlan.formationCode]

  const duplicateParticipantIds = useMemo(() => {
    const participantCounts = new Map<number, number>()

    for (const participantId of Object.values(draftPlan.lineup)) {
      participantCounts.set(
        participantId,
        (participantCounts.get(participantId) ?? 0) + 1,
      )
    }

    return new Set(
      [...participantCounts.entries()]
        .filter(([, count]) => count > 1)
        .map(([participantId]) => participantId),
    )
  }, [draftPlan.lineup])

  function applyParticipants(board: FormationBoardResponse) {
    setParticipants(board.participants.map((participant) => ({
      participantId: participant.participantId,
      memberId: participant.memberId,
      participantName: participant.participantName,
      isGuest: participant.isGuest,
      quarterParticipation: participant.quarterParticipation,
    })))
  }

  function applyBoardResponse(
    board: FormationBoardResponse,
    displayedQuarter: Quarter,
  ) {
    const plans = createQuarterPlansFromBoard(board)

    setScheduleId(board.scheduleId)
    setMatchTitle(board.matchTitle)
    setMatchStartsAt(board.startsAt)
    applyParticipants(board)
    setSavedQuarterPlans(plans)
    setActiveQuarter(displayedQuarter)
    setDraftPlan(copyQuarterPlan(plans[displayedQuarter]))
    setSelectedParticipantId(null)
    setApiError('')
  }

  function toggleMember(memberId: number) {
    setSelectedMemberIds((currentIds) => (
      currentIds.includes(memberId)
        ? currentIds.filter((currentId) => currentId !== memberId)
        : [...currentIds, memberId]
    ))
  }

  async function addSelectedMembers() {
    if (scheduleId === null || selectedMemberIds.length === 0) {
      return
    }

    setIsSaving(true)
    setApiError('')

    try {
      const board = await addMemberParticipants(scheduleId, {
        memberIds: selectedMemberIds,
      })
      applyParticipants(board)
      setSelectedMemberIds([])
    } catch (error) {
      setApiError(
        error instanceof Error
          ? error.message
          : '참여 인원을 저장하지 못했습니다.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  async function addGuest() {
    const trimmedGuestName = guestName.trim()

    if (!trimmedGuestName || scheduleId === null) {
      return
    }

    setIsSaving(true)
    setApiError('')

    try {
      const board = await addGuestParticipant(scheduleId, {
        guestName: trimmedGuestName,
      })
      applyParticipants(board)
      setGuestName('')
    } catch (error) {
      setApiError(
        error instanceof Error
          ? error.message
          : '용병을 저장하지 못했습니다.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  async function removeParticipation(participant: Participant) {
    if (scheduleId === null) {
      return
    }

    const shouldRemove = window.confirm(
      `${participant.participantName} 선수를 이번 경기 참여 인원에서 제외할까요?\n` +
      '저장된 모든 쿼터의 스쿼드 배치에서도 함께 빠집니다.',
    )

    if (!shouldRemove) {
      return
    }

    setIsSaving(true)
    setApiError('')

    try {
      const board = await removeParticipant(
        scheduleId,
        participant.participantId,
      )
      applyBoardResponse(board, activeQuarter)
      setSaveMessage(
        `${participant.participantName} 선수를 참여 인원과 스쿼드에서 제외했습니다.`,
      )
    } catch (error) {
      setApiError(
        error instanceof Error
          ? error.message
          : '참여 인원을 제외하지 못했습니다.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  function assignParticipant(
    slotId: string,
    participantId = selectedParticipantId,
  ) {
    if (!participantId) {
      setDraftPlan((currentPlan) => {
        if (!currentPlan.lineup[slotId]) {
          return currentPlan
        }

        const nextLineup = { ...currentPlan.lineup }
        delete nextLineup[slotId]

        return {
          ...currentPlan,
          lineup: nextLineup,
        }
      })
      setSaveMessage('')
      return
    }

    setDraftPlan((currentPlan) => ({
      ...currentPlan,
      lineup: {
        ...currentPlan.lineup,
        [slotId]: participantId,
      },
    }))
    setSelectedParticipantId(null)
    setDraggedParticipantId(null)
    setSaveMessage('')
  }

  function clearActiveQuarter() {
    setDraftPlan((currentPlan) => ({
      ...currentPlan,
      lineup: {},
    }))
    setSelectedParticipantId(null)
    setSaveMessage('')
  }

  async function saveQuarterToDatabase(
    quarter: Quarter,
  ): Promise<FormationBoardResponse | null> {
    if (scheduleId === null) {
      setApiError('저장할 경기 일정이 없습니다.')
      return null
    }

    if (duplicateParticipantIds.size > 0) {
      setApiError('중복 배치된 선수를 정리한 후 저장해 주세요.')
      return null
    }

    setIsSaving(true)
    setApiError('')

    try {
      return await saveQuarterFormation(scheduleId, quarter, {
        formationCode: draftPlan.formationCode,
        players: formationSlots
          .map((slot, index) => ({
            participantId: draftPlan.lineup[slot.slotId],
            slotCode: slot.slotId,
            positionOrder: index + 1,
          }))
          .filter((player) => player.participantId !== undefined),
      })
    } catch (error) {
      setApiError(
        error instanceof Error
          ? error.message
          : '포메이션을 저장하지 못했습니다.',
      )
      return null
    } finally {
      setIsSaving(false)
    }
  }

  async function saveActiveQuarter() {
    const board = await saveQuarterToDatabase(activeQuarter)

    if (board === null) {
      return
    }

    applyBoardResponse(board, activeQuarter)
    setSaveMessage(`${activeQuarter}쿼터 포메이션을 DB에 저장했습니다.`)
  }

  function moveToQuarter(nextQuarter: Quarter) {
    setActiveQuarter(nextQuarter)
    setDraftPlan(copyQuarterPlan(savedQuarterPlans[nextQuarter]))
    setSelectedParticipantId(null)
    setPendingQuarter(null)
    setIsUnsavedModalOpen(false)
    setSaveMessage('')
  }

  function requestQuarterChange(nextQuarter: Quarter) {
    if (nextQuarter === activeQuarter) {
      return
    }

    if (isDraftChanged) {
      setPendingQuarter(nextQuarter)
      setIsUnsavedModalOpen(true)
      return
    }

    moveToQuarter(nextQuarter)
  }

  async function saveAndMoveQuarter() {
    if (pendingQuarter === null) {
      return
    }

    const nextQuarter = pendingQuarter
    const board = await saveQuarterToDatabase(activeQuarter)

    if (board === null) {
      return
    }

    applyBoardResponse(board, nextQuarter)
    setPendingQuarter(null)
    setIsUnsavedModalOpen(false)
  }

  function discardAndMoveQuarter() {
    if (pendingQuarter !== null) {
      moveToQuarter(pendingQuarter)
    }
  }

  function cancelQuarterChange() {
    setPendingQuarter(null)
    setIsUnsavedModalOpen(false)
  }

  return (
    <main
      className={[
        'dashboard-main',
        'formation-page',
        canManageFormations ? '' : 'is-read-only',
      ].join(' ')}
    >
      <header className="formation-header">
        <div>
          <h1>포메이션 관리</h1>
        </div>
        <span>DB 저장 연결</span>
      </header>

      <div className="formation-layout">
        <section className="formation-board-section">
          <div className="formation-board-content">
              <div className="formation-toolbar">
                <div className="formation-select-group">
                  <label>
                    쿼터 선택
                    <select
                      value={activeQuarter}
                      onChange={(event) => requestQuarterChange(
                        Number(event.target.value) as Quarter,
                      )}
                    >
                      <option value={1}>1쿼터</option>
                      <option value={2}>2쿼터</option>
                      <option value={3}>3쿼터</option>
                      <option value={4}>4쿼터</option>
                    </select>
                  </label>
                  <label>
                    포메이션
                    <select
                      value={draftPlan.formationCode}
                      onChange={(event) => {
                        setDraftPlan({
                          formationCode: event.target.value as FormationCode,
                          lineup: {},
                        })
                        setSelectedParticipantId(null)
                        setSaveMessage('')
                      }}
                    >
                      <option value="4-2-3-1">4-2-3-1</option>
                      <option value="4-1-2-3">4-1-2-3</option>
                      <option value="4-5-1">4-5-1</option>
                      <option value="4-3-3">4-3-3</option>
                    </select>
                  </label>
                  <p className="formation-toolbar-match">
                    {matchTitle || '경기 정보를 불러오는 중입니다.'}
                  </p>
                </div>
                <div className="formation-toolbar-actions">
                  <button type="button" onClick={clearActiveQuarter}>
                    초기화
                  </button>
                  <button
                    type="button"
                    className="formation-save-button"
                    disabled={!isDraftChanged || isSaving}
                    onClick={() => void saveActiveQuarter()}
                  >
                    {isSaving ? '저장 중...' : '저장'}
                  </button>
                </div>
              </div>

              <div className="soccer-field">
                <div className="field-half-line" />
                <div className="field-center-circle" />
                <div className="field-penalty-box field-penalty-box-top" />
                <div className="field-penalty-box field-penalty-box-bottom" />

                {formationSlots.map((slot) => {
                  const participantId = draftPlan.lineup[slot.slotId]
                  const participant = participantId
                    ? participantMap.get(participantId)
                    : null
                  const isDuplicated = participantId
                    ? duplicateParticipantIds.has(participantId)
                    : false

                  return (
                    <button
                      key={slot.slotId}
                      type="button"
                      className={[
                        'formation-player',
                        participant ? '' : 'is-empty',
                        isDuplicated ? 'is-duplicated' : '',
                      ].join(' ')}
                      style={{ left: `${slot.left}%`, top: `${slot.top}%` }}
                      disabled={!canManageFormations}
                      onClick={() => assignParticipant(slot.slotId)}
                      draggable={canManageFormations && Boolean(participant)}
                      onDragStart={() => {
                        if (participantId) {
                          setDraggedParticipantId(participantId)
                        }
                      }}
                      onDragEnd={() => setDraggedParticipantId(null)}
                      onDragOver={(event) => event.preventDefault()}
                      onDrop={(event) => {
                        event.preventDefault()
                        assignParticipant(slot.slotId, draggedParticipantId)
                      }}
                    >
                      <span className="formation-shirt">
                        {participant ? getUniformLabel(participant) : '+'}
                      </span>
                      <span className="formation-player-name">
                        {participant?.participantName ?? '선택'}
                      </span>
                      {isDuplicated && (
                        <span className="formation-player-warning">중복</span>
                      )}
                    </button>
                  )
                })}
              </div>

              {duplicateParticipantIds.size > 0 && (
                <p className="formation-duplicate-warning" role="alert">
                  같은 선수가 현재 쿼터의 포메이션에 두 번 이상 배치되어 있습니다.
                </p>
              )}

              <div className="formation-candidates">
                {participants.length === 0 ? (
                  <p>오른쪽에서 참여 인원을 추가하면 선수 아이콘이 생성됩니다.</p>
                ) : (
                  participants.map((participant) => (
                    <button
                      key={participant.participantId}
                      type="button"
                      draggable
                      className={
                        selectedParticipantId === participant.participantId
                          ? 'is-selected'
                          : ''
                      }
                      onClick={() => setSelectedParticipantId((currentId) => (
                        currentId === participant.participantId
                          ? null
                          : participant.participantId
                      ))}
                      onDragStart={() => setDraggedParticipantId(
                        participant.participantId,
                      )}
                      onDragEnd={() => setDraggedParticipantId(null)}
                    >
                      <span>{getUniformLabel(participant)}</span>
                      {participant.participantName}
                    </button>
                  ))
                )}
              </div>

              <div className="formation-save-status">
                <span className={isDraftChanged ? 'is-unsaved' : ''}>
                  {isDraftChanged ? '저장하지 않은 변경사항이 있습니다.' : '저장된 상태입니다.'}
                </span>
                {saveMessage && <strong>{saveMessage}</strong>}
              </div>
              {apiError && (
                <p className="formation-error" role="alert">{apiError}</p>
              )}
          </div>
        </section>

        <aside className="formation-side-panel">
          <section className="formation-panel">
            <div className="formation-panel-heading">
              <h2>회원 및 용병 선택</h2>
              <div className="formation-panel-heading-actions">
                <span>{selectedMemberIds.length}명 선택</span>
                <button
                  type="button"
                  className={[
                    'formation-panel-toggle',
                    isMemberSelectorOpen ? 'is-open' : '',
                  ].join(' ')}
                  aria-label={
                    isMemberSelectorOpen
                      ? '회원 및 용병 선택 닫기'
                      : '회원 및 용병 선택 열기'
                  }
                  aria-expanded={isMemberSelectorOpen}
                  onClick={() => setIsMemberSelectorOpen((isOpen) => !isOpen)}
                >
                  ▼
                </button>
              </div>
            </div>

            {isMemberSelectorOpen && (
              <div className="formation-member-selector-content">
                {isLoadingMembers && <p className="formation-message">회원 목록을 불러오는 중입니다.</p>}
                {memberLoadError && <p className="formation-error">{memberLoadError}</p>}

                {!isLoadingMembers && !memberLoadError && (
                  <div className="formation-member-list">
                    {members.map((member) => (
                      <label key={member.memberId}>
                        <input
                          type="checkbox"
                          checked={selectedMemberIds.includes(member.memberId)}
                          onChange={() => toggleMember(member.memberId)}
                        />
                        <span>{member.memberName}</span>
                        <small>
                          {member.hasUniform && member.uniformNumber !== null
                            ? member.uniformNumber
                            : '-'}
                        </small>
                      </label>
                    ))}
                  </div>
                )}

                <button
                  type="button"
                  className="formation-primary-button"
                  disabled={selectedMemberIds.length === 0 || isSaving}
                  onClick={() => void addSelectedMembers()}
                >
                  {isSaving ? '저장 중...' : '선택 인원 추가'}
                </button>

                <div className="formation-guest-form">
                  <input
                    value={guestName}
                    maxLength={20}
                    placeholder="용병 이름"
                    onChange={(event) => setGuestName(event.target.value)}
                    onKeyDown={(event) => {
                      if (event.key === 'Enter') {
                        void addGuest()
                      }
                    }}
                  />
                  <button
                    type="button"
                    disabled={isSaving}
                    onClick={() => void addGuest()}
                  >
                    용병 추가
                  </button>
                </div>
                {apiError && (
                  <p className="formation-error" role="alert">{apiError}</p>
                )}
              </div>
            )}
          </section>

          <section className="formation-participation-section">
            <div className="formation-panel-heading">
              <div>
                <h2>이번 주 참여 인원</h2>
                <p className="formation-participation-match">
                  {matchStartsAt
                    ? `${new Date(matchStartsAt).toLocaleDateString('ko-KR')} · ${matchTitle}`
                    : '가장 가까운 경기 정보를 불러오는 중입니다.'}
                </p>
              </div>
              <span>{participants.length}명</span>
            </div>

            {participants.length === 0 ? (
              <p className="formation-message">참여 인원을 추가해 주세요.</p>
            ) : (
              <div className="participation-table-wrap">
                <table className="participation-table">
                  <thead>
                    <tr>
                      <th>선수</th>
                      <th>1Q</th>
                      <th>2Q</th>
                      <th>3Q</th>
                      <th>4Q</th>
                      <th>합계</th>
                      <th>관리</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr className="participation-group-row">
                      <th colSpan={7}>
                        회원
                        <span>{memberParticipants.length}명</span>
                      </th>
                    </tr>
                    {memberParticipants.map((participant) => (
                      <tr key={participant.participantId}>
                        <td>
                          {participant.participantName}
                          <small>
                            {`등번호 ${getUniformLabel(participant)}`}
                          </small>
                        </td>
                        {participant.quarterParticipation.map((isPlaying, index) => (
                          <td key={`${participant.participantId}-${index}`}>
                            <span className={isPlaying ? 'is-playing' : ''}>
                              {isPlaying ? 'O' : 'X'}
                            </span>
                          </td>
                        ))}
                        <td>
                          {participant.quarterParticipation.filter(Boolean).length}쿼터
                        </td>
                        <td>
                          <button
                            type="button"
                            className="participation-remove-button"
                            disabled={isSaving}
                            onClick={() => void removeParticipation(participant)}
                          >
                            빼기
                          </button>
                        </td>
                      </tr>
                    ))}
                    {guestParticipants.length > 0 && (
                      <>
                        <tr className="participation-group-row is-guest">
                          <th colSpan={7}>
                            용병
                            <span>{guestParticipants.length}명</span>
                          </th>
                        </tr>
                        {guestParticipants.map((participant) => (
                          <tr key={participant.participantId}>
                            <td>
                              {participant.participantName}
                              <small>용병</small>
                            </td>
                            {participant.quarterParticipation.map((isPlaying, index) => (
                              <td key={`${participant.participantId}-${index}`}>
                                <span className={isPlaying ? 'is-playing' : ''}>
                                  {isPlaying ? 'O' : 'X'}
                                </span>
                              </td>
                            ))}
                            <td>
                              {participant.quarterParticipation.filter(Boolean).length}쿼터
                            </td>
                            <td>
                              <button
                                type="button"
                                className="participation-remove-button"
                                disabled={isSaving}
                                onClick={() => void removeParticipation(participant)}
                              >
                                빼기
                              </button>
                            </td>
                          </tr>
                        ))}
                      </>
                    )}
                  </tbody>
                </table>
              </div>
            )}

            <p className="formation-reset-guide">
              경기 당일 23:59 이후에는 다음 경기 기준의 빈 포메이션으로 전환됩니다.
            </p>
          </section>
        </aside>
      </div>

      {isUnsavedModalOpen && (
        <div className="formation-modal-backdrop" role="presentation">
          <section
            className="formation-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="formation-unsaved-title"
          >
            <h2 id="formation-unsaved-title">변경사항을 저장할까요?</h2>
            <p>
              {activeQuarter}쿼터의 포메이션이 변경되었습니다.
              저장하지 않고 이동하면 현재 변경사항이 사라집니다.
            </p>
            <div className="formation-modal-actions">
              <button type="button" onClick={cancelQuarterChange}>취소</button>
              <button type="button" onClick={discardAndMoveQuarter}>
                저장하지 않고 이동
              </button>
              <button
                type="button"
                className="formation-save-button"
                disabled={isSaving}
                onClick={() => void saveAndMoveQuarter()}
              >
                {isSaving ? '저장 중...' : '저장 후 이동'}
              </button>
            </div>
          </section>
        </div>
      )}
    </main>
  )
}
