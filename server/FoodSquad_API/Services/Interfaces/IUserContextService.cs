using FoodSquad_API.Models.Entity;
using System.Threading.Tasks;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IUserContextService
    { 
        Task<User> GetCurrentUserAsync();
        Task CheckOwnershipAsync(User resourceOwner);
    }
}
