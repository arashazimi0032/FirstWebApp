using FirstWebApp.Models;

namespace FirstWebApp.Interfaces
{
    public interface IRaceRepository
    {
        bool Add(Race race);
        bool Delete(Race race);
        bool Update(Race race);
        bool Save();
        Task<IEnumerable<Race>> GetAll();
        Task<Race> GetRaceByIdAsync(int id);
        Task<IEnumerable<Race>> GetRaceByCity(string city);
    }
}
