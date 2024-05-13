using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;


namespace CiklometarBLL.AutoMapper
{
  public class AutoMapping :Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCyclistDTO>().ReverseMap();
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();
            CreateMap<User, AccessTokenDataDTO>().ReverseMap();

            CreateMap<Organization, OrganizationDTO>().ReverseMap();
            CreateMap<Organization, AddOrganizationDTO>().ReverseMap();

            CreateMap<Role, RoleDTO>().ReverseMap();

            CreateMap<Requests, RequestsDTO>().ReverseMap();
            CreateMap<Requests, RequestIdDTO>().ReverseMap();
            CreateMap<Requests, RequestsGetDTO>().ReverseMap();

            CreateMap<Location, LocationDTO>().ReverseMap();
            CreateMap<Location, LocationResponseDTO>()
                .ForMember(l => l.Lat, opt => opt.MapFrom(ld =>
                ld.Coordinates.Coordinate.X))
                .ForMember(l => l.Lng, opt => opt.MapFrom(ld =>
                ld.Coordinates.Coordinate.Y))
                .ReverseMap();

            CreateMap<Activity, ActivitiesDTO>().ReverseMap();
            CreateMap<Activity, ActivityOutputDTO>()
                .ForMember(l => l.Lat, opt => opt.MapFrom(ld =>
                ld.EndLocation.Coordinate.X))
                .ForMember(l => l.Lng, opt => opt.MapFrom(ld =>
                ld.EndLocation.Coordinate.Y));

            CreateMap<StravaTokens, StravaTokenDTO>().ReverseMap();

            CreateMap<CiklometarStatistics, HomepageDTO>().ReverseMap();
        }
    }
}
