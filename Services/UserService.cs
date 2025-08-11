using Api.DTOs;
using Api.Interface;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Api.Services
{
    public class UserService
    {
        private readonly IRepository<Usuario> _repository;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();
        private readonly JwtService _jwtService;

        public UserService(IRepository<Usuario> repository, JwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<Usuario> RegisterAsync(UserDto dto)
        {
            if (!new EmailAddressAttribute().IsValid(dto.Email))
                throw new ValidationException("Email inválido.");

            var existingUsers = await _repository.GetAllAsync();
            if (existingUsers.Any(u => u.Email.ToLower() == dto.Email.ToLower()))
                throw new ValidationException("Email ya registrado.");

            if (dto.Password == null || dto.Password.Length == 0)
                throw new ValidationException("Password es requerido.");

            var passwordString = dto.Password;
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{12,}$");
            if (!regex.IsMatch(passwordString))
                throw new ValidationException("La contraseña debe incluir mayúsculas, minúsculas, números y caracteres especiales.");

            var user = new Usuario
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role
            };

            var hashed = _passwordHasher.HashPassword(user, passwordString);
            user.PasswordHash = hashed;

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return user;
        }

        public async Task<(Usuario? user, string? JwtToken, string? RefreshToken)> LoginAsync(string email, string? password)
        {
            var user = (await _repository.GetAllAsync())
                .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (IsInvalidUserOrPassword(user, password))
                return (null, null, null);

            var passwordString = password!;
            var storedHashString = user!.PasswordHash;

            var result = _passwordHasher.VerifyHashedPassword(user, storedHashString, passwordString);
            if (result == PasswordVerificationResult.Failed)
                return (null, null, null);

            var jwtToken = _jwtService.GenerateJwtToken(user.Id, user.Email, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _repository.Update(user);
            await _repository.SaveChangesAsync();

            return (user, jwtToken, refreshToken);
        }

        private bool IsInvalidUserOrPassword(Usuario? user, string? password)
        {
            return user == null || password == null || password.Length == 0;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync() =>
            await _repository.GetAllAsync();
        public async Task<Usuario?> GetByIdAsync(Guid id) =>
            await _repository.GetByIdAsync(id);

        public async Task UpdateAsync(Guid id, UserDto dto)
        {
            var user = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Usuario no encontrado.");

            if (user.Email != dto.Email)
            {
                var users = await _repository.GetAllAsync();
                if (users.Any(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != id))
                    throw new ValidationException("Email ya en uso.");

                user.Email = dto.Email;
            }

            user.Name = dto.Name;
            user.Role = dto.Role;

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(Guid id, string currentPassword, string newPassword)
        {
            var user = await _repository.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var storedHashString = user.PasswordHash;
            var result = _passwordHasher.VerifyHashedPassword(user, storedHashString, currentPassword);

            if (result == PasswordVerificationResult.Failed)
                throw new ValidationException("La contraseña actual es incorrecta.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 12)
                throw new ValidationException("La nueva contraseña debe tener al menos 12 caracteres.");

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Usuario no encontrado.");

            _repository.Delete(user);
            await _repository.SaveChangesAsync();
        }
    }
}
