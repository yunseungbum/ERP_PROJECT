using System.ComponentModel.DataAnnotations;

namespace BuddyErp.Api.DTOs.Auth;

public sealed record LoginRequest(
    [Required(ErrorMessage = "아이디를 입력해 주세요.")]
    [MaxLength(50)]
    string LoginId,

    [Required(ErrorMessage = "비밀번호를 입력해 주세요.")]
    [MaxLength(100)]
    string Password);
