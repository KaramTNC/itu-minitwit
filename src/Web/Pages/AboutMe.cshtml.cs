using System.Security.Claims;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages;
/// <summary>
/// Handles logic for the about me page 
/// </summary>
/// <param name="service"></param>
public class AboutMeModel(ICheepService service) : PageModel
{
    public required List<CheepViewModel> UserCheepsVm { get; set; }
    public required AuthorViewModel UserAuthorVm { get; set; }
    public required List<AuthorViewModel> FollowingVm { get; set; }
    public required List<CheepViewModel> LikedCheepsVm { get; set; }

    /// <summary>
    /// Performs on Page Load
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        UserCheepsVm = await service.GetUserCheeps(User.FindFirst(ClaimTypes.Email)?.Value!, page);
        UserAuthorVm =  await service.GetAuthorViewModel(User.FindFirst(ClaimTypes.Email)?.Value!);
        FollowingVm = await service.GetFollowerViewModel(User.FindFirst(ClaimTypes.Email)?.Value!);
        LikedCheepsVm = await service.GetLikedCheepsForAuthor(User.FindFirst(ClaimTypes.Email)?.Value!);
            
        return Page();
    }

    /// <summary>
    /// Performs when pressing "Forget Me"
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnPostForget()
    {
        var identity = User.FindFirst(ClaimTypes.Email)?.Value;
        await service.DeleteAuthor(identity!);
        
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        Response.Cookies.Delete("Seq-Session");
        Response.Cookies.Delete(".AspNetCore.Antiforgery.xYiNViD5USA");
        
        return RedirectToPage("Public");
    }
    
    [BindProperty] public required string Email { get; set; }

    /// <summary>
    /// Performs when pressing "Follow" on a Cheep Post
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostFollow([FromQuery] int page = 0)
    {
        await service.UpdateFollower(User.FindFirst(ClaimTypes.Email)?.Value!, Email);
        return RedirectToPage("AboutMe");
    }

    [BindProperty]
    public int CheepId { get; set; }
    
    /// <summary>
    /// Performs when pressing "Like" on a Cheep
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostLike([FromQuery] int page = 0)
    {
        await service.UpdateCheepLikes(CheepId, User.FindFirst(ClaimTypes.Email)?.Value!);
        return RedirectToPage("AboutMe");
    }
}
