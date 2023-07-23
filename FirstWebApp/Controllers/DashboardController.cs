using CloudinaryDotNet.Actions;
using FirstWebApp.Interfaces;
using FirstWebApp.Models;
using FirstWebApp.Repository;
using FirstWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FirstWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(IDashboardRepository dashboardRepository, 
            IHttpContextAccessor httpContextAccessor, IPhotoService photoService, UserManager<AppUser> userManager)
        {
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
            _userManager = userManager;
        }
        public void MapUserEdit(AppUser user, EditUserDashboardViewModel editVM, ImageUploadResult photoResult)
        {
            string defaultImage = "./images/No Image.png";
            string ImageUrlString = photoResult.Url != null ? photoResult.Url.ToString() : defaultImage;

            user.Id = editVM.Id;
            user.Pace = editVM.Pace;
            user.Mileage = editVM.Mileage;
            user.ProfileImageUrl = ImageUrlString;
            user.State = editVM.State;
            user.City = editVM.City;
        }
        public async Task<IActionResult> Index()
        {
            var userRaces = await _dashboardRepository.GetAllUserRacesAsync();
            var userClubs = await _dashboardRepository.GetAllUserClubsAsync();
            string curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                Id = curUserId,
                Races = userRaces,
                Clubs = userClubs
            };
            return View(dashboardViewModel);
        }

        public async Task<IActionResult> EditUserProfile(string id)
        {

            AppUser user = await _dashboardRepository.GetUserById(id);
            if (user == null) return View("Error");
            EditUserDashboardViewModel editUserDashboardViewModel = new EditUserDashboardViewModel
            {
                Id = id, 
                Pace = user.Pace, 
                Mileage = user.Mileage,
                ProfileImageUrl = user.ProfileImageUrl, 
                City = user.City,
                State = user.State,
            };
            return View(editUserDashboardViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View(editVM);
            }

            AppUser user = await _dashboardRepository.GetUserByIdNoTracking(editVM.Id);
            if (user.ProfileImageUrl == null || user.ProfileImageUrl == "")
            {
                ImageUploadResult photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _photoService.DetelePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", "Could not delete photo");
                    return View(editVM);
                }

                ImageUploadResult photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _dashboardRepository.Update(user);
                return RedirectToAction("Index");
            }

        }

        public async Task<IActionResult> DeleteUserProfile(string id)
        {
            AppUser user = await _dashboardRepository.GetUserById(id);
            if (user == null) return View("Error");
            UserViewModel userViewModel = new UserViewModel
            {
                Id = id,
                Pace = user.Pace, 
                Mileage = user.Mileage,
            };
            return View(userViewModel);
        }

        [HttpPost, ActionName("DeleteUserProfile")]
        public async Task<IActionResult> DeleteUserProfileAction(string id)
        {
            AppUser user = await _dashboardRepository.GetUserById(id);
            
            var role = await _userManager.GetRolesAsync(user);
            string rl = role[0];
            if (rl != "admin")
            {
                if (user == null) return View("Error");
                try
                {
                    await _photoService.DetelePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo.");
                }
                _dashboardRepository.Delete(user);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Could not delete photo.");
                return View("Error");
            }
            
        }
    }
}
