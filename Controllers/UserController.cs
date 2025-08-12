using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Api.Interface;
using System.Text.Json;


namespace Api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : BaseController
    {
        private readonly UserService _service;
        private readonly JwtService _jwtService;
        private readonly IUserRepository _userRepo;
        private readonly RedisService _redisService;

        public UserController(UserService service, JwtService jwtService, IUserRepository userRepo, RedisService redisService)
        {
            _service = service;
            _jwtService = jwtService;
            _userRepo = userRepo;
            _redisService = redisService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public Task<IActionResult> Register([FromBody] UserDto dto) => ExecuteAsync(async () =>
        {
            if (dto.Password == null || dto.Password.Length < 12)
                return BadRequest(new { Error = "La contraseña debe tener al menos 12 caracteres." });

            var user = await _service.RegisterAsync(dto);
            var response = new
            {
                status = "ok",
                data = new
                {
                    user.Name,
                    user.Email,
                    user.Role
                }
            };
            await _redisService.ClearCacheByPrefixAsync("users:");
            await _redisService.ClearCacheByPrefixAsync("user:");
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
        });

        [HttpPost("login")]
        [AllowAnonymous]
        public Task<IActionResult> Login([FromBody] UserDto dto) => ExecuteAsync(async () =>
        {
            var (user, jwtToken, refreshToken) = await _service.LoginAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized(new { Error = "Credenciales inválidas." });

            var response = new
            {
                status = "ok",
                data = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    Token = jwtToken,
                    RefreshToken = refreshToken
                }
            };
            return Ok(response);
        });

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            string cacheKey = "users:all";

            var cachedUsers = await _redisService.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUsers))
            {
                var usersDto = JsonSerializer.Deserialize<IEnumerable<object>>(cachedUsers);
                return Ok(new { status = "ok", data = usersDto });
            }

            var users = await _service.GetAllAsync();
            var usersData = users.Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.Role
            });

            await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(usersData));

            return Ok(new { status = "ok", data = usersData });
        });

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            string cacheKey = $"user:{id}";

            var cachedUser = await _redisService.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUser))
            {
                var userDto = JsonSerializer.Deserialize<object>(cachedUser);
                return Ok(new { status = "ok", data = userDto });
            }

            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Error = "Usuario no encontrado." });

            var userData = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role
            };

            await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(userData));

            return Ok(new { status = "ok", data = userData });
        });

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public Task<IActionResult> Update(Guid id, [FromBody] UserDto dto) => ExecuteAsync(async () =>
        {
            await _service.UpdateAsync(id, dto);
            var response = new { status = "ok", data = "Actualizado correctamente" };
            return Ok(response);
        });

        [HttpPost("{id}/change-password")]
        [AllowAnonymous]
        public Task<IActionResult> ChangePassword(Guid id, [FromBody] UserDto dto) => ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { Error = "CurrentPassword y NewPassword son requeridos." });

            await _service.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);

            var response = new { status = "ok", data = "Actualizada correctamente" };
            await _redisService.ClearCacheByPrefixAsync("users:");
            await _redisService.ClearCacheByPrefixAsync("user:");
            return Ok(response);
        });

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Delete(Guid id) => ExecuteAsync(async () =>
        {
            await _service.DeleteAsync(id);
            var response = new { status = "ok", data = "Eliminado correctamente" };
            return Ok(response);
        });

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {

            ClaimsPrincipal principal;
            principal = _jwtService.GetPrincipalFromExpiredToken(request.Token);


            var email = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value
                        ?? principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email == null) return BadRequest("Token inválido.");

            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return Unauthorized("Usuario no encontrado.");

            if (user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Refresh token ya usado o expirado.");
            }

            var newJwtToken = _jwtService.GenerateJwtToken(user.Id, user.Email, user.Role);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new
            {
                status = "ok",
                data = new
                {
                    token = newJwtToken,
                    refreshToken = newRefreshToken
                }
            });
        }
    }
}
