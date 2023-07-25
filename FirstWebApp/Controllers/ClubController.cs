using CloudinaryDotNet.Actions;
using FirstWebApp.Data;
using FirstWebApp.Interfaces;
using FirstWebApp.Models;
using FirstWebApp.Repository;
using FirstWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApp.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            CreateClubViewModel createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
            return View(createClubViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubVM)
        {
            if (ModelState.IsValid) 
            {
                var result = await _photoService.AddPhotoAsync(clubVM.Image);
                string defaultImage = "./images/No Image.png";
                string ImageUrlString = result.Url != null ? result.Url.ToString() : defaultImage;

                var club = new Club
                {
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    Image = ImageUrlString,
                    AppUserId = clubVM.AppUserId,
                    Address = new Address
                    {
                        Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State
                    },
                    ClubCategory = clubVM.ClubCategory
                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "photo upload failed.");
            }
            return View(clubVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            if (club == null) return View("Error");
            EditClubViewModel clubVM = new EditClubViewModel
            {
                Title = club.Title, 
                Description = club.Description, 
                URL = club.Image, 
                AddressId = club.AddressId,
                Address = club.Address,
                ClubCategory = club.ClubCategory
            };
            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club.");
                return View(clubVM);
            }

            Club club = await _clubRepository.GetByIdAsyncNoTracking(id);
            if (club != null)
            {
                string ImageUrlString = "./images/No Image.png";
                if (clubVM.Image != null)
                {
                    try
                    {
                        await _photoService.DetelePhotoAsync(club.Image);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Could not delete photo.");
                        return View(clubVM);
                    }
                    ImageUploadResult uploadResult = await _photoService.AddPhotoAsync(clubVM.Image);
                    ImageUrlString = uploadResult.Url.ToString();
                }
                else
                {
                    if (club.Image != null)
                    {
                        ImageUrlString = club.Image;
                    }

                }

                Club clubNew = new Club
                {
                    Id = id,
                    Title = clubVM.Title,
                    Description = clubVM.Description,
                    ClubCategory = clubVM.ClubCategory,
                    Address = new Address
                    {
                        Id = club.Address.Id,
                        Street = clubVM.Address.Street, 
                        State = clubVM.Address.State,
                        City = clubVM.Address.City,
                    },
                    AddressId = club.AddressId,
                    Image = ImageUrlString,
                };
                _clubRepository.Update(clubNew);
                return RedirectToAction("Index");
            }
            else
            {
                return View(clubVM);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            Club Club = await _clubRepository.GetByIdAsync(id);
            if (Club == null) return View("Error");
            return View(Club);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            Club Club = await _clubRepository.GetByIdAsync(id);
            if (Club == null) return View("Error");
            try
            {
                await _photoService.DetelePhotoAsync(Club.Image);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo.");
            }
            _clubRepository.Delete(Club);
            return RedirectToAction("Index");
        }

    }
}
