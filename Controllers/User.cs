
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/json")]

public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserDto userDto)
    {
        _userService.CreateUser(userDto.Username, userDto.Password);
        return Ok("User Created");
    }

    [HttpGet()]
    public IActionResult GetUsers()
    {
        try
        {
            var users = _userService.GetUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{username}")]

    public IActionResult DropUser(string username)
    {
        _userService.DropUser(username);
        return Ok("User Dropped");
    }



}

public class UserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
