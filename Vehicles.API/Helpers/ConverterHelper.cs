using AutoMapper;
using System;
using System.Threading.Tasks;
using Vehicles.API.Data;
using Vehicles.API.Data.Entities;
using Vehicles.API.Models;

namespace Vehicles.API.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IMapper _mapper;
        public ConverterHelper(DataContext context, ICombosHelper combosHelper, IMapper mapper)
        {
            _context = context;
            _combosHelper = combosHelper;
            _mapper = mapper;
        }

        public async Task<Detail> ToDetailAsync(DetailViewModel model)
        {
            Detail detail = _mapper.Map<Detail>(model);
            //detail.History = await _context.Histories.FindAsync(model.HistoryId);
            detail.Procedure = await _context.Procedures.FindAsync(model.ProcedureId);
            return detail;
        }

        public DetailViewModel ToDetailViewModel(Detail detail)
        {
            DetailViewModel detailV = _mapper.Map<DetailViewModel>(detail);
            detailV.Procedures = _combosHelper.GetComboProcedures();

            return detailV;
        }

        public async Task<User> ToUserAsync(UserViewModel model, Guid imageId)
        {

            User user = _mapper.Map<User>(model);
            user.DocumentType = await _context.DocumentTypes.FindAsync(model.DocumentTypeId);
            user.ImageId = imageId;
            user.Id = string.IsNullOrEmpty(user.Id) ? Guid.NewGuid().ToString() : model.Id;


            return user;
            //{
            //    Address = model.Address,
            //    Document = model.Document,
            //    DocumentType = await _context.DocumentTypes.FindAsync(model.DocumentTypeId),
            //    Email = model.Email,
            //    FirstName = model.FirstName,
            //    Id = isNew ? Guid.NewGuid().ToString() : model.Id,
            //    ImageId = imageId,
            //    LastName = model.LastName,
            //    PhoneNumber = model.PhoneNumber,
            //    UserName = model.Email,
            //    UserType = model.UserType,
            //};
        }

        public UserViewModel ToUserViewModel(User user)
        {

            UserViewModel model = _mapper.Map<UserViewModel>(user);
            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();

            return model;

            //return new UserViewModel
            //{
            //    Address = user.Address,
            //    Document = user.Document,
            //    DocumentTypeId = user.DocumentType.Id,
            //    DocumentTypes = _combosHelper.GetComboDocumentTypes(),
            //    Email = user.Email,
            //    FirstName = user.FirstName,
            //    Id = user.Id,
            //    ImageId = user.ImageId,
            //    LastName = user.LastName,
            //    PhoneNumber = user.PhoneNumber,
            //    UserType = user.UserType,
            //};
        }

        public async Task<Vehicle> ToVehicleAsync(VehicleViewModel model, bool isNew)
        {
            Vehicle vehicle = _mapper.Map<Vehicle>(model);

            vehicle.Brand = await _context.Brands.FindAsync(model.BrandId);

            vehicle.VehicleType = await _context.VehicleTypes.FindAsync(model.VehicleTypeId);

            return vehicle;

            //return new Vehicle
            //{
            //    Brand = await _context.Brands.FindAsync(model.BrandId),
            //    Color = model.Color,
            //    Id = isNew ? 0 : model.Id,
            //    Line = model.Line,
            //    Model = model.Model,
            //    Plaque = model.Plaque.ToUpper(),
            //    Remarks = model.Remarks,
            //    VehicleType = await _context.VehicleTypes.FindAsync(model.VehicleTypeId)
            //};
        }

        public VehicleViewModel ToVehicleViewModel(Vehicle vehicle)
        {
            return new VehicleViewModel
            {
                BrandId = vehicle.Brand.Id,
                Brands = _combosHelper.GetComboBrands(),
                Color = vehicle.Color,
                Id = vehicle.Id,
                Line = vehicle.Line,
                Model = vehicle.Model,
                Plaque = vehicle.Plaque.ToUpper(),
                Remarks = vehicle.Remarks,
                UserId = vehicle.User.Id,
                VehiclePhotos = vehicle.VehiclePhotos,
                VehicleTypeId = vehicle.VehicleType.Id,
                VehicleTypes = _combosHelper.GetComboVehicleTypes()
            };
        }
    }
}
