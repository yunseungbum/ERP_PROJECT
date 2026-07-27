export type ScheduleResponse = {
  scheduleId: number
  venueName: string
  opponentName: string
  startsAt: string
  matchFee: number
  isMatchFeePaid: boolean
  payerName: string
  notes: string
  isCompleted: boolean
  opponentContact: string | null
}

export type ScheduleSaveRequest = Omit<ScheduleResponse, 'scheduleId'>
