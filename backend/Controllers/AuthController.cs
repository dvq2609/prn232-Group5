using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.DTOs;
using AutoMapper;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CloneEbayDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        public AuthController(CloneEbayDbContext context, IMapper mapper, IConfiguration configuration, IAccountService accountService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _accountService = accountService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var users = await _accountService.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            else
            {
                string roleName = user.Role;
                if (string.IsNullOrEmpty(roleName))
                {
                    return Unauthorized("Invalid email or password");
                }
                var token = GenerateJwtToken(user.Email ?? "", roleName, user.Id);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    Role = roleName,
                    AccountId = user.Id
                });
            }
        }
        private string GenerateJwtToken(string email, string role, int accountId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),

                new Claim("AccountId", accountId.ToString())
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}