using backend.Models;
namespace backend.Services
{
    public interface IAccountService
    {
        Task<List<User>> GetAllUsers();
    }
}