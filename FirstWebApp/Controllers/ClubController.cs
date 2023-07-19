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

        public ClubController(IClubRepository clubRepository, IPhotoService photoService)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
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
            return View();
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
                    Address = new Address
                    {
                        Street = clubVM.Address.Street,
                        City = clubVM.Address.City,
                        State = clubVM.Address.State
                    },
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

        public async Task<IActionResult> Chert(string city)
        {
            IEnumerable<Club> clubs = await _clubRepository.GetClubByCity(city);
            return View(clubs);
        }
    }
}
