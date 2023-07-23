using FirstWebApp.Interfaces;
using FirstWebApp.Models;
using FirstWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FirstWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AppUser> users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach (AppUser user in users)
            {
                result.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Pace = user.Pace,
                    Mileage = user.Mileage,
                    ProfileImageUrl = user.ProfileImageUrl
                });
            }

            return View(result);
        }

        public async Task<IActionResult> Detail(string id)
        {
            AppUser user = await _userRepository.GetUserById(id);
            UserDetailViewModel userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id, 
                UserName = user.UserName,
                Pace = user.Pace, 
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl
            };
            return View(userDetailViewModel);
        }
    }
}
