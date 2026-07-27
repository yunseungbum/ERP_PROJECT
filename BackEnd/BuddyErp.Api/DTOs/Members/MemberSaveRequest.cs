using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Members;

public sealed record MemberSaveRequest(
    [Required, MaxLength(50)]
    string MemberName,

    [Required]
    [RegularExpression(MemberPositionCodes.ValidationPattern)]
    string PrimaryPosition,

    [RegularExpression(MemberPositionCodes.ValidationPattern)]
    string? SecondaryPosition,

    [Required, MaxLength(20)]
    string PhoneNumber,

    [Range(1900, 2100)]
    int BirthYear,

    [MaxLength(1000)]
    string Notes,

    [Required]
    [RegularExpression(MemberStatusCodes.ValidationPattern)]
    string MemberStatus,

    bool HasUniform,

    [Range(1, 99, ErrorMessage = "등번호는 1부터 99까지 입력해 주세요.")]
    int? UniformNumber);
