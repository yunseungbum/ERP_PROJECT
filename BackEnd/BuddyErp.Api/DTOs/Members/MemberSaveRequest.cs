using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Members;

public sealed record MemberSaveRequest(
    [Required(ErrorMessage = "회원 이름을 입력해 주세요.")]
    [MaxLength(50)]
    string MemberName,

    [Required(ErrorMessage = "1순위 포지션을 선택해 주세요.")]
    [RegularExpression(
        MemberPositionCodes.ValidationPattern,
        ErrorMessage = "올바른 1순위 포지션을 선택해 주세요.")]
    string PrimaryPosition,

    [RegularExpression(
        MemberPositionCodes.ValidationPattern,
        ErrorMessage = "올바른 2순위 포지션을 선택해 주세요.")]
    string? SecondaryPosition,

    [Required(ErrorMessage = "연락처를 입력해 주세요.")]
    [MaxLength(20)]
    string PhoneNumber,

    [Range(1900, 2100, ErrorMessage = "올바른 출생연도를 입력해 주세요.")]
    int BirthYear,

    [MaxLength(1000)]
    string Notes,

    [Required(ErrorMessage = "회원 활동 상태를 선택해 주세요.")]
    [RegularExpression(
        MemberStatusCodes.ValidationPattern,
        ErrorMessage = "올바른 회원 활동 상태를 선택해 주세요.")]
    string MemberStatus);
