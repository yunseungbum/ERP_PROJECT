using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Attendance;

public sealed record AttendanceUpdateRequest(
    [Required, MaxLength(1)] string Status);
