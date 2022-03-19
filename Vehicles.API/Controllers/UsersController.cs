using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.API.Data;
using Vehicles.API.Data.Entities;
using Vehicles.API.Helpers;
using Vehicles.API.Models;
using Vehicles.Common.Enums;

namespace Vehicles.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        public UsersController(IUserHelper userHelper,
            DataContext context,
            IMapper mapper,
            ICombosHelper combosHelper,
            IImageHelper imageHelper, IConverterHelper converterHelper)
        {
            _userHelper = userHelper;
            _context = context;
            _mapper = mapper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(x => x.DocumentType)
                .Include(x => x.Vehicles)
                .Where(x => x.UserType == UserType.User)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            UserViewModel model = new UserViewModel
            {
                DocumentTypes = _combosHelper.GetComboDocumentTypes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _converterHelper.ToUserAsync(model, imageId);
                user.UserType = UserType.User;

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, user.UserType.ToString());

                //string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                //string tokenLink = Url.Action("ConfirmEmail", "Account", new
                //{
                //    userid = user.Id,
                //    token = myToken
                //}, protocol: HttpContext.Request.Scheme);

                //Response response = _mailHelper.SendMail(model.Email, "Vehicles - Confirmación de cuenta", $"<h1>Vehicles - Confirmación de cuenta</h1>" +
                //    $"Para habilitar el usuario, " +
                //    $"por favor hacer clic en el siguiente enlace: </br></br><a href = \"{tokenLink}\">Confirmar Email</a>");

                return RedirectToAction(nameof(Index));
            }

            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel model = _converterHelper.ToUserViewModel(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;
                if (model.ImageFile != null)
                {
                    string result= await _imageHelper.DeleteImageAsync(model.ImageFullPath, "Images\\users");
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _converterHelper.ToUserAsync(model, imageId);

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }

            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _context.Users
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehiclePhotos)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Histories)
                .ThenInclude(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.ImageId != Guid.Empty)
            {
                string result = await _imageHelper.DeleteImageAsync(user.ImageFullPath, "Images\\users");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _context.Users
                .Include(x => x.DocumentType)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Brand)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehicleType)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.VehiclePhotos)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Histories)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> AddVehicle(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _context.Users
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            VehicleViewModel model = new VehicleViewModel
            {
                Brands = _combosHelper.GetComboBrands(),
                UserId = user.Id,
                VehicleTypes = _combosHelper.GetComboVehicleTypes()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(VehicleViewModel vehicleViewModel)
        {
            User user = await _context.Users
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == vehicleViewModel.UserId);
            if (user == null)
            {
                return NotFound();
            }

            Guid imageId = Guid.Empty;

            if (vehicleViewModel.ImageFile != null)
            {
                imageId = await _imageHelper.UploadImageAsync(vehicleViewModel.ImageFile, "vehiclephotos");
            }

            Vehicle vehicle = await _converterHelper.ToVehicleAsync(vehicleViewModel, true);

            if (vehicle.VehiclePhotos == null)
            {
                vehicle.VehiclePhotos = new List<VehiclePhoto>();
            }

            vehicle.VehiclePhotos.Add(new VehiclePhoto
            {
                ImageId = imageId
            });

            try
            {
                user.Vehicles.Add(vehicle);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = user.Id });
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esa placa.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }

            vehicleViewModel.Brands = _combosHelper.GetComboBrands();
            vehicleViewModel.VehicleTypes = _combosHelper.GetComboVehicleTypes();
            return View(vehicleViewModel);
        }

        public async Task<IActionResult> EditVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .Include(x => x.User)
                .Include(x => x.Brand)
                .Include(x => x.VehicleType)
                .Include(x => x.VehiclePhotos)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            VehicleViewModel model = _mapper.Map<VehicleViewModel>(vehicle);
            model.Brands = _combosHelper.GetComboBrands();
            model.VehicleTypes = _combosHelper.GetComboVehicleTypes();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(int id, VehicleViewModel vehicleViewModel)
        {
            if (id != vehicleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Vehicle vehicle = _mapper.Map<Vehicle>(vehicleViewModel);
                    vehicle.VehicleType = await _context.VehicleTypes.FindAsync(vehicleViewModel.VehicleTypeId);

                    _context.Vehicles.Update(vehicle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = vehicleViewModel.UserId });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un vehículo con esta placa.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            vehicleViewModel.Brands = _combosHelper.GetComboBrands();
            vehicleViewModel.VehicleTypes = _combosHelper.GetComboVehicleTypes();
            return View(vehicleViewModel);
        }

        public async Task<IActionResult> DeleteVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .Include(x => x.User)
                .Include(x => x.VehiclePhotos)
                .Include(x => x.Histories)
                .ThenInclude(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(vehicle);

            await _context.SaveChangesAsync();

            foreach (var item in vehicle.VehiclePhotos)
            {
                string response=await _imageHelper.DeleteImageAsync(item.ImageFullPath, "Images\\vehiclephotos");
            }

            return RedirectToAction(nameof(Details), new { id = vehicle.User.Id });
        }

        public async Task<IActionResult> DeleteImageVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VehiclePhoto vehiclePhoto = await _context.VehiclePhotos
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehiclePhoto == null)
            {
                return NotFound();
            }

            try
            {
                await _imageHelper.DeleteImageAsync(vehiclePhoto.ImageFullPath, "Images\\vehiclephotos");
                
            }
            catch { }

            _context.VehiclePhotos.Remove(vehiclePhoto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditVehicle), new { id = vehiclePhoto.Vehicle.Id });
        }

        public async Task<IActionResult> AddVehicleImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            VehiclePhotoViewModel model = new()
            {
                VehicleId = vehicle.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicleImage(VehiclePhotoViewModel model)
        {
            if (ModelState.IsValid)
            {

                Guid imageId = await _imageHelper.UploadImageAsync(model.ImageFile, "vehiclephotos");
                Vehicle vehicle = await _context.Vehicles
                    .Include(x => x.VehiclePhotos)
                    .FirstOrDefaultAsync(x => x.Id == model.VehicleId);
                if (vehicle.VehiclePhotos == null)
                {
                    vehicle.VehiclePhotos = new List<VehiclePhoto>();
                }

                vehicle.VehiclePhotos.Add(new VehiclePhoto
                {
                    ImageId = imageId
                });

                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditVehicle), new { id = vehicle.Id });
            }

            return View(model);

        }

        public async Task<IActionResult> DetailsVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles
                .Include(x => x.User)
                .Include(x => x.VehicleType)
                .Include(x => x.Brand)
                .Include(x => x.VehiclePhotos)
                .Include(x => x.Histories)
                .ThenInclude(x => x.Details)
                .ThenInclude(x => x.Procedure)
                .Include(x => x.Histories)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        public async Task<IActionResult> AddHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vehicle vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            HistoryViewModel model = new HistoryViewModel
            {
                VehicleId = vehicle.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHistory(HistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Vehicle vehicle = await _context.Vehicles
                    .Include(x => x.Histories)
                    .FirstOrDefaultAsync(x => x.Id == model.VehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                History history = new History
                {
                    Date = DateTime.UtcNow,
                    Mileage = model.Mileage,
                    Remarks = model.Remarks,
                    User = user
                };

                if (vehicle.Histories == null)
                {
                    vehicle.Histories = new List<History>();
                }

                vehicle.Histories.Add(history);
                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsVehicle), new { id = vehicle.Id });
            }

            return View(model);
        }

        public async Task<IActionResult> EditHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            History history = await _context.Histories
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            HistoryViewModel model = new HistoryViewModel
            {
                Mileage = history.Mileage,
                Remarks = history.Remarks,
                VehicleId = history.Vehicle.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHistory(int id, HistoryViewModel historyViewModel)
        {
            if (ModelState.IsValid)
            {
                History history = await _context.Histories.FindAsync(id);
                history.Mileage = historyViewModel.Mileage;
                history.Remarks = historyViewModel.Remarks;
                _context.Histories.Update(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsVehicle), new { id = historyViewModel.VehicleId });
            }

            return View(historyViewModel);
        }

        public async Task<IActionResult> DeleteHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            History history = await _context.Histories
                .Include(x => x.Details)
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            _context.Histories.Remove(history);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsVehicle), new { id = history.Vehicle.Id });
        }

        public async Task<IActionResult> DetailsHistory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            History history = await _context.Histories
                .Include(x => x.Details)
                .ThenInclude(x => x.Procedure)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.VehiclePhotos)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        public async Task<IActionResult> AddDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            History history = await _context.Histories.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            DetailViewModel model = new DetailViewModel
            {
                HistoryId = history.Id,
                Procedures = _combosHelper.GetComboProcedures()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail(DetailViewModel detailViewModel)
        {
            if (ModelState.IsValid)
            {
                History history = await _context.Histories
                    .Include(x => x.Details)
                    .Include(x=>x.Vehicle)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == detailViewModel.HistoryId);

                if (history == null)
                {
                    return NotFound();
                }

                if (history.Details == null)
                {
                    history.Details = new List<Detail>();
                }

                Detail detail =await _converterHelper.ToDetailAsync(detailViewModel);

                try
                {
                    history.Details.Add(detail);
                    _context.Histories.Update(history);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(DetailsHistory), new { id = detailViewModel.HistoryId });

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }            
            }

            detailViewModel.Procedures = _combosHelper.GetComboProcedures();
            return View(detailViewModel);
        }

        public async Task<IActionResult> EditDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Detail detail = await _context.Details
                .Include(x => x.History)
                .Include(x => x.Procedure)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (detail == null)
            {
                return NotFound();
            }

            DetailViewModel model = _converterHelper.ToDetailViewModel(detail);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetail(int id, DetailViewModel detailViewModel)
        {
            if (ModelState.IsValid)
            {
                Detail detail = await _converterHelper.ToDetailAsync(detailViewModel);
                _context.Details.Update(detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetailsHistory), new { id = detailViewModel.HistoryId });
            }

            detailViewModel.Procedures = _combosHelper.GetComboProcedures();
            return View(detailViewModel);
        }

        public async Task<IActionResult> DeleteDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Detail detail = await _context.Details
                .Include(x => x.History)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.Details.Remove(detail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DetailsHistory), new { id = detail.History.Id });
        }
    }
}
