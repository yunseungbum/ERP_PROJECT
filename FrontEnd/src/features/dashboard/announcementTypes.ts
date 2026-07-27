export type AnnouncementResponse = {
  announcementId: number
  title: string
  content: string
  authorName: string
  createdAt: string
  updatedAt: string
}

export type AnnouncementSaveRequest = {
  title: string
  content: string
}
