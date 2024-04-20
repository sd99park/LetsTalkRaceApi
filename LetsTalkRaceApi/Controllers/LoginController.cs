using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LetsTalkRaceApi.Models;
using LetsTalkRaceApi.Models.Requests;
using LetsTalkRaceApi.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LetsTalkRaceApi.Controllers;

// TODO: Better error handling
// Throw 404's

[ApiController]
[Route("api/login/v1")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LoginController : LtrControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public LoginController(IConfiguration config, SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : base(config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
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
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count > 1)
        {
            throw new Exception("Multiple roles");
        }
        return Ok(CreateJwtToken(user, roles.FirstOrDefault() ?? "NONE"));
    }
    
    // TODO: This needs some sort of perms to only allow use from ProfilesController 
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // TODO: Test what happen if added with no role or non-existent role
        var user = new IdentityUser()
        {
            UserName = request.Email,
            Email = request.Email
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error creating identity");
        }

        var createdUser = await _userManager.FindByEmailAsync(user.UserName);
        
        result = await _userManager.AddToRoleAsync(createdUser, request.Role);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error adding identity role identity");
        }
        
        return Ok(CreateJwtToken(createdUser, request.Role));
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
        // TODO: IdentityId must match callerId OR have super_admin perms
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
        // TODO: IdentityId must match callerId OR have super_admin perms
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
        // TODO: IdentityId must match callerId OR have mega_admin perms
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

    [HttpDelete("user/{identityId}/deleteUser")]
    // [Authorize(Roles = PermissionConstants.ADMIN)]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> DeleteIdentity(string identityId)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        if (user == null)
        {
            throw new Exception($"User not found with id {identityId}");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error deleting identity");
        }
        
        return Ok("Identity successfully deleted");
    }

    [HttpPost, Authorize(Roles = PermissionConstants.SUPER_ADMIN)]
    [Route("role")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var role = new IdentityRole
        {
            Name = roleName
        };
        await _roleManager.CreateAsync(role);

        return Ok("Role Created");
    }
    
    [HttpDelete, Authorize(Roles = PermissionConstants.SUPER_ADMIN)]
    [Route("role")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> DeleteRole([FromBody] string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            throw new Exception($"Role not found with name {roleName}");
        }
        
        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error deleting role");
        }

        return Ok("Role Deleted");
    }

    [HttpPost, Authorize(Roles = PermissionConstants.ADMIN)]
    [Route("user/{identityId}/addRole")]
    [ProducesResponseType(typeof(IdentityUser), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> AddIdentityRole(string identityId, [FromBody] string roleName)
    {
        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        var user = await _userManager.FindByIdAsync(identityId); 
        if (user == null)
        {
            throw new Exception($"User not found with id {identityId}");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error adding role to identity");
        }
        
        var updatedUser = await _userManager.FindByIdAsync(identityId); 
        
        return Ok(updatedUser);
    }

    [HttpPost, Authorize(Roles = PermissionConstants.ADMIN)]
    [Route("user/{identityId}/removeRole")]
    [ProducesResponseType(typeof(IdentityUser), 201)]
    [ProducesResponseType(typeof(BadRequestResult), 400)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), 401)]
    public async Task<IActionResult> RemoveIdentityRole(string identityId, [FromBody] string roleName)
    {
        var user = await _userManager.FindByIdAsync(identityId); 
        if (user == null)
        {
            throw new Exception($"User not found with id {identityId}");
        }
        
        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            throw new Exception("There was an error removing role from identity");
        }
        
        var updatedUser = await _userManager.FindByIdAsync(identityId);
        return Ok(updatedUser);
    }
    
    
    private string CreateJwtToken(IdentityUser user, string role)
    {
        
        // TODO: Add user role somehow
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, role)
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