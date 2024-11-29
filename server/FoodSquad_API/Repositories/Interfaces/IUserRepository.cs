using FoodSquad_API.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodSquad_API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllPaginatedAsync(int page, int size);
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task AddUserAsync(User user);
    }
}
