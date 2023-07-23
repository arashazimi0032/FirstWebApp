using FirstWebApp.Data;
using FirstWebApp.Interfaces;
using FirstWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByIdNoTracking(string id)
        {
            return await _context.Users.Where(i => i.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public bool Update(AppUser user)
        {
            _context.Users.Update(user);
            return Save();
        }

        public bool Delete(AppUser user)
        {
            _context.Users.Remove(user);
            return Save();
        }
        public bool Save()
        {
            int saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
