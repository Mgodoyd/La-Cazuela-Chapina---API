using Api.Data;
using Api.Interface;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class UserRepository : EfRepository<Usuario>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Usuario?> GetByEmailAsync(string email) =>
            await _context.User.FirstOrDefaultAsync(u => u.Email == email);
    }

}