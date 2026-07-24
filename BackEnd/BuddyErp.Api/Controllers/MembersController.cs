using BuddyErp.Api.DTOs.Members;
using BuddyErp.Api.Services.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuddyErp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class MembersController(
    IMemberService memberService) : ControllerBase
{
    private const string MemberWriters =
        "President,Director,Coach,Treasurer,InventoryManager";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberResponse>>> GetMembers(
        CancellationToken cancellationToken)
    {
        var members = await memberService.GetMembersAsync(
            cancellationToken);

        return Ok(members);
    }

    [HttpGet("{memberId:long}")]
    public async Task<ActionResult<MemberResponse>> GetMember(
        long memberId,
        CancellationToken cancellationToken)
    {
        var member = await memberService.GetMemberAsync(
            memberId,
            cancellationToken);

        return member is null
            ? NotFound()
            : Ok(member);
    }

    [Authorize(Roles = MemberWriters)]
    [HttpPost]
    public async Task<ActionResult<MemberResponse>> CreateMember(
        MemberSaveRequest request,
        CancellationToken cancellationToken)
    {
        var member = await memberService.CreateMemberAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetMember),
            new { memberId = member.MemberId },
            member);
    }

    [Authorize(Roles = MemberWriters)]
    [HttpPut("{memberId:long}")]
    public async Task<ActionResult<MemberResponse>> UpdateMember(
        long memberId,
        MemberSaveRequest request,
        CancellationToken cancellationToken)
    {
        var member = await memberService.UpdateMemberAsync(
            memberId,
            request,
            cancellationToken);

        return member is null
            ? NotFound()
            : Ok(member);
    }

    [Authorize(Roles = "President")]
    [HttpDelete("{memberId:long}")]
    public async Task<IActionResult> DeleteMember(
        long memberId,
        CancellationToken cancellationToken)
    {
        var deactivated = await memberService.DeactivateMemberAsync(
            memberId,
            cancellationToken);

        return deactivated
            ? NoContent()
            : NotFound();
    }
}
