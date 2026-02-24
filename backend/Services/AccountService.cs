using backend.Models;
using backend.Repositories;
namespace backend.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<List<User>> GetAllUsers()
        {
            return await _accountRepository.GetAllUsers();
        }
    }
}