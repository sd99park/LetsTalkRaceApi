using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LetsTalkRaceApi.Models.Requests;
using LetsTalkRaceApi.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LetsTalkRaceApi.Controllers;

[ApiController]
[Route("api/login/v1")]
public class LoginController : LtrControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginController(IConfiguration config, SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager) : base(config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.IsPersistent,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid login attempt" });
        }
        
        var user = await _userManager.FindByNameAsync(request.UserName);
        return Ok(CreateJwtToken(user));
    }
    
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new IdentityUser()
        {
            Id = request.Id ?? Guid.NewGuid().ToString(),
            UserName = request.Email,
            Email = request.Email
        };

        // TODO: Validate Access Code?
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception("There was an error creating identity");
        }
        
        var createdUser = await _userManager.FindByEmailAsync(user.UserName);
        return Ok(CreateJwtToken(createdUser));
    }
    
    [HttpGet, Authorize]
    [Route("user/{identityId}")]
    [ProducesResponseType(typeof(IdentityResponseDTO), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> GetIdentity(string identityId)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        return Ok(new IdentityResponseDTO(user));
    }
    
    [HttpPost("user/{identityId}/updateEmail"), Authorize]
    [ProducesResponseType(typeof(IdentityResponseDTO), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> UpdateEmail(string identityId, [FromBody] string newEmail)
    {
        // Idk if I actually want to send in previous email, added for now, can remove later
        
        var user = await _userManager.FindByIdAsync(identityId);
        
        var result = await _userManager.SetEmailAsync(user, newEmail);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error updating identity email");
        }
        
        user = await _userManager.FindByIdAsync(identityId);
        return Ok(new IdentityResponseDTO(user));
    }

    [HttpPost("user/{identityId}/updateUserName"), Authorize]
    [ProducesResponseType(typeof(IdentityResponseDTO), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> UpdateUserName(string identityId, [FromBody] string userName)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        var result = await _userManager.SetUserNameAsync(user, userName);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error updating identity UserName");
        }
        
        user = await _userManager.FindByIdAsync(identityId);
        return Ok(new IdentityResponseDTO(user));
    }

    [HttpPost("user/{identityId}/updatePassword"), Authorize]
    [ProducesResponseType(typeof(IdentityResponseDTO), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> UpdatePassword(string identityId, [FromBody] UpdatePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(identityId);

        var validOldPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);
        if (!validOldPassword)
        {
            throw new HttpRequestException("Invalid previous password");
        }

        if (request.OldPassword.Equals(request.NewPassword))
        {
            throw new HttpRequestException("New password is the same as previous password");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error updating identity Password");
        }
        
        user = await _userManager.FindByIdAsync(identityId);
        return Ok(new IdentityResponseDTO(user));
    }

    [HttpDelete("user/{identityId}/deleteUser"), Authorize]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> DeleteIdentity(string identityId)
    {
        var user = await _userManager.FindByIdAsync(identityId);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error updating identity Password");
        }
        
        return Ok("Identity successfully deleted");
    }
    
    private string CreateJwtToken(IdentityUser user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var tokenConfig = _config.GetSection("AppSettings:Token").Value;
        if (tokenConfig == null)
        {
            throw new Exception("Error getting AppSettings:Token");
        }

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenConfig));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(4),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}   