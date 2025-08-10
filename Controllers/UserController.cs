using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : BaseController
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
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
        [Authorize]
        public Task<IActionResult> GetAll() => ExecuteAsync(async () =>
        {
            var users = await _service.GetAllAsync();
            var response = new
            {
                status = "ok",
                data = users.Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role
                })
            };
            return Ok(response);
        });

        [HttpGet("{id}")]
        [Authorize]
        public Task<IActionResult> GetById(Guid id) => ExecuteAsync(async () =>
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { Error = "Usuario no encontrado." });

            var response = new
            {
                status = "ok",
                data = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role
                }
            };
            return Ok(response);
        });

        [HttpPut("{id}")]
        [Authorize]
        public Task<IActionResult> Update(Guid id, [FromBody] UserDto dto) => ExecuteAsync(async () =>
        {
            await _service.UpdateAsync(id, dto);
            var response = new { status = "ok", data = "Actualizado correctamente" };
            return Ok(response);
        });

        [HttpPost("{id}/change-password")]
        [Authorize]
        public Task<IActionResult> ChangePassword(Guid id, [FromBody] UserDto dto) => ExecuteAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { Error = "CurrentPassword y NewPassword son requeridos." });

            await _service.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);

            var response = new { status = "ok", data = "Actualizada correctamente" };
            return Ok(response);
        });

        [HttpDelete("{id}")]
        [Authorize]
        public Task<IActionResult> Delete(Guid id) => ExecuteAsync(async () =>
        {
            await _service.DeleteAsync(id);
            var response = new { status = "ok", data = "Eliminado correctamente" };
            return Ok(response);
        });
    }
}
