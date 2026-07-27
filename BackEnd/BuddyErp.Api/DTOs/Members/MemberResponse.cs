namespace BuddyErp.Api.DTOs.Members;

public sealed record MemberResponse(
    long MemberId,
    string MemberName,
    string PrimaryPosition,
    string? SecondaryPosition,
    string PhoneNumber,
    int BirthYear,
    string Notes,
    string MemberStatus,
    bool HasUniform,
    int? UniformNumber,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
