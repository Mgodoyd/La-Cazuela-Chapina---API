using Api.Data;
using Api.Interface;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task<Usuario?> GetByEmailAsync(string email) =>
            await _context.User.FirstOrDefaultAsync(u => u.Email == email);



        public async Task<Usuario?> GetByIdAsync(Guid id) =>
            await _context.User.FindAsync(id);
    }

}