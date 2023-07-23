using FirstWebApp.Models;

namespace FirstWebApp.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Race>> GetAllUserRacesAsync();
        Task<List<Club>> GetAllUserClubsAsync();
        Task<AppUser> GetUserById(string id);
        Task<AppUser> GetUserByIdNoTracking(string id);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
    }
}
