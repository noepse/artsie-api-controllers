using ArtsieApi.Models;
using ArtsieApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace artStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtController : ControllerBase
{
    private readonly ArtsieService _artService;

    public ArtController(ArtsieService artService) =>
        _artService = artService;

    [HttpGet]
    public async Task<List<Art>> Get() =>
        await _artService.GetArtAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Art>> Get(string id)
    {
        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        return art;
    }
        [HttpGet("{id:length(24)}/comments")]
    public async Task<ActionResult<List<Comment>>> GetComments(string id)
    {
        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        return await _artService.GetCommentsOnArtAsync(id);
    }
        [HttpPost("{id:length(24)}/comments")]
    public async Task<IActionResult> Post(Comment newComment, string id)
    {
        await _artService.CreateCommentAsync(newComment, id);

        return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);
    }

    // [HttpPost]
    // public async Task<IActionResult> Post(Art newArt)
    // {
    //     await _artService.CreateArtAsync(newArt);

    //     return CreatedAtAction(nameof(Get), new { id = newArt.Id }, newArt);
    // }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Art updatedArt)
    {
        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        updatedArt.Id = art.Id;

        await _artService.UpdateArtAsync(id, updatedArt);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        await _artService.RemoveArtAsync(id);

        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ArtsieService _commentsService;

    public CommentsController(ArtsieService commentsService) =>
        _commentsService = commentsService;

    [HttpGet]
    public async Task<List<Comment>> Get() =>
        await _commentsService.GetCommentsAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Comment>> Get(string id)
    {
        var comment = await _commentsService.GetCommentAsync(id);

        if (comment is null)
        {
            return NotFound();
        }

        return comment;
    }

    [HttpPatch("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Likes likesUpdate)
    {
        var comment = await _commentsService.GetCommentAsync(id);

        if (comment is null)
        {
            return NotFound();
        }

        comment.Likes = comment.Likes + likesUpdate.IncLikes;

        await _commentsService.UpdateCommentAsync(id, comment);

        return CreatedAtAction(nameof(Get), comment);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var comment = await _commentsService.GetCommentAsync(id);

        if (comment is null)
        {
            return NotFound();
        }

        await _commentsService.RemoveCommentAsync(id);

        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ArtsieService _usersService;

    public UsersController(ArtsieService usersService) =>
        _usersService = usersService;

    [HttpGet]
    public async Task<List<User>> Get() =>
        await _usersService.GetUsersAsync();

    [HttpGet("{username}")]
    public async Task<ActionResult<User>> Get(string username)
    {
        var user = await _usersService.GetUserAsync(username);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        await _usersService.CreateUserAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _usersService.GetUserAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        updatedUser.Id = user.Id;

        await _usersService.UpdateUserAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _usersService.GetArtAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _usersService.RemoveArtAsync(id);

        return NoContent();
    }
}