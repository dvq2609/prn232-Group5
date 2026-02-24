using backend.Models;
namespace backend.Repositories
{
    public interface IAccountRepository
    {
        Task<List<User>> GetAllUsers();
    }
}