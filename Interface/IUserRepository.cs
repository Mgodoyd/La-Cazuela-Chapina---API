using Api.Models;

namespace Api.Interface

{
    public interface IUserRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByIdAsync(Guid id);
    }
}