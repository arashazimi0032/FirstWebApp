﻿using CloudinaryDotNet.Actions;
using FirstWebApp.Interfaces;
using FirstWebApp.Models;
using FirstWebApp.Repository;
using FirstWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FirstWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepository, IPhotoService photoService)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetRaceByIdAsync(id);
            return View(race);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(raceVM.Image);
                string defaultImage = "./images/No Image.png";
                string ImageUrlString = result.Url != null ? result.Url.ToString() : defaultImage;

                var race = new Race
                {
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = ImageUrlString,
                    Address = new Address
                    {
                        Street = raceVM.Address.Street,
                        City = raceVM.Address.City,
                        State = raceVM.Address.State
                    },
                };
                _raceRepository.Add(race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "photo upload failed.");
            }
            return View(raceVM);
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            Race race = await _raceRepository.GetRaceByIdAsync(id);
            if (race == null) return View("Error");
            EditRaceViewModel raceVM = new EditRaceViewModel
            {
                Title = race.Title, 
                Description = race.Description,
                Address = race.Address,
                URL = race.Image,
                AddressId = race.AddressId,
                RaceCategory = race.RaceCategory
            };
            return View(raceVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit race.");
                return View(raceVM);
            }

            Race race = await _raceRepository.GetRaceByIdAsyncNoTracking(id);
            if (race != null)
            {
                try
                {
                    await _photoService.DetelePhotoAsync(race.Image);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo.");
                    return View(raceVM);
                }

                ImageUploadResult uploadResult = await _photoService.AddPhotoAsync(raceVM.Image);
                string defaultImage = "./images/No Image.png";
                string ImageUrlString = uploadResult.Url != null ? uploadResult.Url.ToString() : defaultImage;
                Race raceNew = new Race
                {
                    Id = id,
                    Title = raceVM.Title,
                    Description = raceVM.Description,
                    Image = ImageUrlString,
                    Address = raceVM.Address,
                    AddressId = race.AddressId,
                    RaceCategory = raceVM.RaceCategory
                };
                _raceRepository.Update(raceNew);
                return RedirectToAction("Index");
            }
            else
            {
                return View(raceVM);
            }
        }
    }
}
