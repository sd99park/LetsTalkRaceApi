using LetsTalkRaceApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalkRaceApi.Controllers;

[ApiController]
[Route("[controller]")] // [controller] = Login, it strips "controller off of class name
public class LoginController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public LoginController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("v1/login")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // TODO: IsValid does nothing atm, maybe validate another way eventually
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.IsPersistent,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // TODO: Generate JWT
            // Example: var token = GenerateToken(model.Email);
            // return Ok(new { Token = token });
            return Ok(new { message = "Login successful" });
        }

        return Unauthorized(new { message = "Invalid login attempt" });
        
    }
    
    [HttpPost("v1/register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // TODO: IsValid does nothing atm, maybe validate another way eventually
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            Email = request.Email
            
        };

        // TODO: Validate Access Code
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // TODO: Return Valid JWT
            return Ok("User created successfully");
        }

        // TODO: Better error handling
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }
}   