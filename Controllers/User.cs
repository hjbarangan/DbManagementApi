
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[ApiController]
[Route("/api/[controller]")]


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

        var response = new { message = "User Created" };
        return Ok(JsonConvert.SerializeObject(response));
    }

    [HttpGet()]
    public IActionResult GetUsers()
    {
        try
        {
            var users = _userService.GetUsers();
            var jsonUsers = JsonConvert.SerializeObject(users);
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
