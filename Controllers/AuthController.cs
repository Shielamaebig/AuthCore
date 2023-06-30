using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthCore.Models;
using AuthCore.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using AuthCore.Services;

namespace AuthCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        public AuthController(UserManager<IdentityUser> userManager, ApplicationDbContext context,
             IConfiguration configuration, TokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userManager.CreateAsync(
                new IdentityUser { UserName = request.Username, Email = request.Email },
                request.Password
            );
            if (result.Succeeded)
            {
                request.Password = "";
                return CreatedAtAction(nameof(Register), new { email = request.Email }, request);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<string>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(request.Email);
            if (managedUser == null)
            {
                return BadRequest("Bad credentials");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var userInDb = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (userInDb is null)
                return Unauthorized();

            var accessToken = CreateToken(userInDb);

            return Ok(new
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken,
            });
        }

        private string CreateToken(IdentityUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        //WithServicesTokenService
        // [HttpPost]
        // [Route("login")]
        // public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     var managedUser = await _userManager.FindByEmailAsync(request.Email);
        //     if (managedUser == null)
        //     {
        //         return BadRequest("Bad credentials");
        //     }
        //     var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
        //     if (!isPasswordValid)
        //     {
        //         return BadRequest("Bad credentials");
        //     }
        //     var userInDb = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        //     if (userInDb is null)
        //         return Unauthorized();
        //     var accessToken = _tokenService.CreateToken(userInDb);
        //     await _context.SaveChangesAsync();
        //     return Ok(new AuthResponse
        //     {
        //         Username = userInDb.UserName,
        //         Email = userInDb.Email,
        //         Token = accessToken,
        //     });
        // }
    }
}