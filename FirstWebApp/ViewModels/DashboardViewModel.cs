using FirstWebApp.Models;

namespace FirstWebApp.ViewModels
{
    public class DashboardViewModel
    {
        public string Id { get; set; }
        public List<Race> Races { get; set; }
        public List<Club> Clubs { get; set; }
    }
}
