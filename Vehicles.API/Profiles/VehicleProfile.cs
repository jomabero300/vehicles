using AutoMapper;
using Vehicles.API.Data.Entities;
using Vehicles.API.Models;

namespace Vehicles.API.Profiles
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<Vehicle, VehicleViewModel>().ReverseMap();
        }
    }
}
