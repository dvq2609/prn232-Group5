using backend.Models;
using Microsoft.EntityFrameworkCore;
namespace backend.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CloneEbayDbContext _context;
        public AccountRepository(CloneEbayDbContext context)
        {
            _context = context;
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

    }
}