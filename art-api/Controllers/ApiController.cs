using ArtsieApi.Models;
using ArtsieApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<Art>> Get(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return BadRequest();
        }
        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        return art;
    }
    [HttpGet("{id}/comments")]
    public async Task<ActionResult<List<Comment>>> GetComments(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return BadRequest();
        }

        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        return await _artService.GetCommentsOnArtAsync(id);
    }
    [HttpPost("{id}/comments")]
    public async Task<IActionResult> Post(Comment newComment, string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return BadRequest();
        }

        var art = await _artService.GetArtAsync(id);

        if (art is null)
        {
            return NotFound();
        }

        await _artService.CreateCommentAsync(newComment, id);

        return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);
    }

    // [HttpPost]
    // public async Task<IActionResult> Post(Art newArt)
    // {
    //     await _artService.CreateArtAsync(newArt);

    //     return CreatedAtAction(nameof(Get), new { id = newArt.Id }, newArt);
    // }

    // [HttpPut("{id:length(24)}")]
    // public async Task<IActionResult> Update(string id, Art updatedArt)
    // {
    //     var art = await _artService.GetArtAsync(id);

    //     if (art is null)
    //     {
    //         return NotFound();
    //     }

    //     updatedArt.Id = art.Id;

    //     await _artService.UpdateArtAsync(id, updatedArt);

    //     return NoContent();
    // }

    // [HttpDelete("{id:length(24)}")]
    // public async Task<IActionResult> Delete(string id)
    // {
    //     var art = await _artService.GetArtAsync(id);

    //     if (art is null)
    //     {
    //         return NotFound();
    //     }

    //     await _artService.RemoveArtAsync(id);

    //     return NoContent();
    // }
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Comment>> Get(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return BadRequest();
        }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out ObjectId objectId))
        {
            return BadRequest();
        }
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

    [HttpPatch("{username}")]
    public async Task<IActionResult> Update(string username, User updatedUser)
    {
        var user = await _usersService.GetUserAsync(username);

        if (user is null)
        {
            return NotFound();
        }

        user.Username = updatedUser.Username;

        await _usersService.UpdateUserAsync(username, user);

        return CreatedAtAction(nameof(Get), user);
    }

    [HttpDelete("{username}")]
    public async Task<IActionResult> Delete(string username)
    {
        var user = await _usersService.GetUserAsync(username);

        if (user is null)
        {
            return NotFound();
        }

        await _usersService.RemoveUserAsync(username);

        return NoContent();
    }
}