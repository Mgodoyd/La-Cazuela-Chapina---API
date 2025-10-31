using Api.Models;

namespace Api.Interface

{
    public interface IUserRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
    }
}