using FirstWebApp.Data;
using FirstWebApp.Interfaces;
using FirstWebApp.Models;

namespace FirstWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Club>> GetAllUserClubsAsync()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            return _context.Clubs.Where(r => r.AppUser.Id == curUser).ToList();
        }

        public async Task<List<Race>> GetAllUserRacesAsync()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            return _context.Races.Where(r => r.AppUser.Id == curUser).ToList();
        }
    }
}
